using System;
using System.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace NL.Rijksoverheid.RDW.RemoteDocumentEncryption.Wrapping.Implementation;

public class EACCAProtocol
{

    /// <summary>
    /// Cut down the result to the correct wrapper and the session ephemeral public key Z
    /// This function create the correct flavour of DH key pair and passes the shared secret into the correct flavour of SecureMessageWrapper
    /// Problem here is the format of the piccPublicKeyDer which is read from the DG14. Java returns a PublicKey object.
    /// </summary>
    /// <param name="keyId"></param>
    /// <param name="protocolOid"></param>
    /// <param name="publicKeyOid"></param>
    /// <param name="piccPublicKeyDer">DG14/param>
    /// <returns></returns>
    public static RdeEACCAResult doCA(BigInteger keyId, string protocolOid, string publicKeyOid, /*May not want to use this type cos it might not be portable*/ byte[] piccPublicKeyDer)
    {
        if (piccPublicKeyDer == null)
            throw new ArgumentException("PICC public key is null");

        //TODO and the rest...

        var agreementAlg = ChipAuthenticationInfo.GetKeyAgreementAlgorithm(protocolOid);

        //TODO?
        //protocolOid == protocolOid ?? inferChipAuthenticationOIDfromPublicKeyOID(publicKeyOid);

        //..and pray it's an ASN1 object...
        var piccPublicKey = PublicKeyFactory.CreateKey(piccPublicKeyDer);
        var pcdEphemeralKeyPair = CreateKeyPair(agreementAlg, piccPublicKey);
        var sharedSecret = ComputeSharedSecret(agreementAlg, piccPublicKey, pcdEphemeralKeyPair.Private);
        var wrapper = RestartSecureMessaging(protocolOid, sharedSecret); //And we are done.

        return new RdeEACCAResult(SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(pcdEphemeralKeyPair.Public).GetDerEncoded(), wrapper);
    }
    
    public static AsymmetricCipherKeyPair CreateKeyPair(KeyAgreementAlgorithm agreementAlg, AsymmetricKeyParameter piccPublicKeyAsCipherParameter)
    {
        if (KeyAgreementAlgorithm.DH == agreementAlg)
        {
            var p = ((DHPublicKeyParameters)piccPublicKeyAsCipherParameter).Parameters;
            var keyPairGenerator = new DHKeyPairGenerator();
            keyPairGenerator.Init(new DHKeyGenerationParameters(new(), p));
            return keyPairGenerator.GenerateKeyPair();
        }

        if (KeyAgreementAlgorithm.ECDH == agreementAlg)
        {
            var p = ((ECPublicKeyParameters)piccPublicKeyAsCipherParameter).Parameters;

            ////var algorithm = "brainpoolP256t1";
            ////var algorithm = "brainpoolP160t1";
            ////var algorithm = "brainpoolP384t1";
            ////var algorithm = "brainpoolP320t1";
            ////var algorithm = "brainpoolP192t1";
            ////var algorithm = "brainpoolP256r1";
            ////var algorithm = "brainpoolP192r1";
            ////var algorithm = "brainpoolP512t1";
            ////var algorithm = "brainpoolP160r1";
            ////var algorithm = "brainpoolP512r1";
            ////var algorithm = "brainpoolP384r1";
            ////var algorithm = "brainpoolP224r1";
            //var algorithm = "brainpoolP320r1"; // <----- SPEC 2014 DG14
            ////var algorithm = "brainpoolP224t1";

            //var table = ECNamedCurveTable.GetByName(algorithm);
            //var p = new ECDomainParameters(curve: table.Curve,g: table.G,n: table.N,h: table.H);

            var keyPairGenerator = new ECKeyPairGenerator();
            keyPairGenerator.Init(new ECKeyGenerationParameters(p, new()));
            return keyPairGenerator.GenerateKeyPair();
        }

        throw new InvalidOperationException("Unsupported agreement algorithm.");
    }

    public static byte[] ComputeSharedSecret(KeyAgreementAlgorithm alg, ICipherParameters piccPublicKey, ICipherParameters pcdPrivateKey)
    {
        var agreement = AgreementUtilities.GetBasicAgreement(alg == KeyAgreementAlgorithm.DH ? "DH" : "ECDH");
        agreement.Init(pcdPrivateKey);
        return agreement.CalculateAgreement(piccPublicKey).ToByteArrayUnsigned();
    }

    public static SecureMessagingWrapper RestartSecureMessaging(string oid, byte[] sharedSecret)
    {
        var cipherInfo = ChipAuthenticationInfo.GetCipher(oid);

        /* Start secure messaging. */
        var ksEnc = SessionMessagingWrapperKeyUtility.DeriveKey(sharedSecret, cipherInfo, SessionMessagingWrapperKeyUtility.ENC_MODE);
        var ksMac = SessionMessagingWrapperKeyUtility.DeriveKey(sharedSecret, cipherInfo, SessionMessagingWrapperKeyUtility.MAC_MODE);

        if (cipherInfo.Algorithm == CipherAlgorithm.DESede)
            return new DESedeSecureMessagingWrapper(ksEnc, ksMac);

        if (cipherInfo.Algorithm == CipherAlgorithm.Aes)
            return new AesSecureMessagingWrapper(ksEnc, ksMac);

        throw new InvalidOperationException("Unsupported cipher algorithm.");
    }
}