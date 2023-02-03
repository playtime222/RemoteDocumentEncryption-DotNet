using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using NL.Rijksoverheid.RDW.RDE.CaSessionUtilities.Wrapping.Implementation;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.X509;

namespace CaSessionUtilitiesTest;

public class Step1_SharedSecrets
{
    [InlineData("J 1")]
    [InlineData("Case 2")]
    [InlineData("Case 3")]
    [InlineData("Case 4")]
    [InlineData("Case 5")]
    [InlineData("Case 6")]
    [InlineData("Case 7")]
    [InlineData("Case 8")]
    [Theory]
    public void Test(string caseName)
    {
        var testCase = TestCases.Items[caseName];
        var agreementAlg = ChipAuthenticationInfo.GetKeyAgreementAlgorithm(Spec2014Content.DG14_CA_ProtocolOID);
        Assert.Equal(KeyAgreementAlgorithm.ECDH, agreementAlg);

        var sharedSecret = EACCAProtocol.ComputeSharedSecret(agreementAlg, PublicKeyFactory.CreateKey(Hex.Decode(Spec2014Content.DG14_PubKeyHex)), PrivateKeyFactory.CreateKey(Hex.Decode(testCase.PcdPrivateKey)));

        Trace.WriteLine("Actual   : " + Hex.ToHexString(sharedSecret));
        Trace.WriteLine("Expected : " + testCase.SharedSecret);

        Assert.Equal(Hex.Decode(testCase.SharedSecret), sharedSecret);
    }

    //Generate tests for Java
    [Fact]
    public void Generate()
    {
        var agreementAlg = ChipAuthenticationInfo.GetKeyAgreementAlgorithm(Spec2014Content.DG14_CA_ProtocolOID);
        Trace.WriteLine("Alg:" + agreementAlg);
        var piccPublicKey = PublicKeyFactory.CreateKey(Hex.Decode(Spec2014Content.DG14_PubKeyHex));
        var keyPair = EACCAProtocol.CreateKeyPair(agreementAlg, piccPublicKey);
        var sharedSecret = EACCAProtocol.ComputeSharedSecret(agreementAlg, PublicKeyFactory.CreateKey(Hex.Decode(Spec2014Content.DG14_PubKeyHex)), keyPair.Private);
        var publicDer = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keyPair.Public).ToAsn1Object().GetDerEncoded();

        var privateDer = PrivateKeyInfoFactory.CreatePrivateKeyInfo(keyPair.Private).ToAsn1Object().GetDerEncoded();

        //var pem = "MIIBmwIBAQQo01s1RjPWie9b12U3C5jKZtHsEODA9BI8CZLbBP+V8cgcOiVIIuVk\r\n1KCCARQwggEQAgEBMDQGByqGSM49AQECKQDTXkcgNrxPt+E8eF7SAeBl+Y/Ppvb0\r\nDe9PkrnseJPsKPzUErHxsy4nMFQEKD7jC1aPurD4g8zr1G0/O7iipzUT9et52mYZ\r\nDrCF/6n0kvN1qX2GDrQEKFIIg5Sd/bxC060ZhkBoim/hP0E0lVS0mswx3M2IRTmB\r\nb160rI+x8aYEUQRDvX6a+1PYuFKJvMSO5b/m8gE30QoIfrbnhx4qEKWZxxCvjQ05\r\n4gYRFP3QVUXsHMirQJMkf3cnXgdD/+0RcYLqqcd4d6qsasfTUkXRaS6O4QIpANNe\r\nRyA2vE+34Tx4XtIB4GX5j8+lto8Soy1ILsfuhljphpFVW0TFkxECAQGhVANSAASe\r\nKRyflLgRGE6kwhKICMLXE6FM6kgIVLoo3Lf7gAt2Q5zTCT173XZHk65RGdnj9kG9\r\ni7s5Dw73YVtPoy3MGOpphr7Fm7uHNThUI5P55hFB6A==";
        //Trace.WriteLine(Hex.ToHexString(Base64.Decode(pem.Replace("\r\n", ""))));
        //var wtf = keyPair.Private.EncodePrivateKeyDerBase64();
        //Trace.WriteLine("PCD Public Key  : " + wtf);
        Trace.WriteLine("PCD Private Key : " + Hex.ToHexString(privateDer));
        Trace.WriteLine("PCD Public Key  : " + Hex.ToHexString(publicDer));
        Trace.WriteLine("Shared Secret   : " + Hex.ToHexString(sharedSecret));
       
    }
}

//public static class Crypto2
//{
//    public static string EncodeDerBase64(this AsymmetricKeyParameter key) => Convert.ToBase64String(SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(key).GetDerEncoded());
//    public static string EncodePrivateKeyDerBase64(this AsymmetricCipherKeyPair keys)
//    {
//        var textWriter = new StringWriter();
//        var pemWriter = new PemWriter(textWriter);
//        pemWriter.WriteObject(keys.Private);
//        pemWriter.Writer.Flush();
//        return textWriter.ToString();
//    }
//    public static string EncodePrivateKeyDerBase64(this AsymmetricKeyParameter asymmetricKeyParameter)
//    {
//        var textWriter = new StringWriter();
//        var pemWriter = new PemWriter(textWriter);
//        pemWriter.WriteObject(asymmetricKeyParameter);
//        pemWriter.Writer.Flush();
//        return textWriter.ToString();
//    }
//}