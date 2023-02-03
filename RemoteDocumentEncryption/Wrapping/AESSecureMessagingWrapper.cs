using System;
using System.Diagnostics;
using System.Linq;
using Org.BouncyCastle.Utilities.Encoders;

namespace NL.Rijksoverheid.RDW.RemoteDocumentEncryption.Wrapping;

public class AesSecureMessagingWrapper : SecureMessagingWrapper
{
    public AesSecureMessagingWrapper(byte[] ksEnc, byte[] ksMac)
    : base(ksEnc, ksMac, "AES/CBC/NoPadding", "AESCMAC")
    {
    }

    public override int BlockSize => 16;

    public override byte[] CalculateMac(byte[] data)
        => Crypto.GetAesCMac(KsMac, data);

    public override byte[] GetEncodedDataForResponse(byte[] paddedResponse)
    {
        var iv = Crypto.GetAesEcbNoPaddingCipherText(KsEnc, GetEncodedSendSequenceCounter(2)); //Contains state -> 2 the SSC counter...
        Trace.WriteLine($"IV: {Hex.ToHexString(iv)}");

        var result = Crypto.GetAesCbcNoPaddingCipherText(KsEnc, iv, paddedResponse);
        Trace.WriteLine($"Response ciphertext: {Hex.ToHexString(result)}");
        return result;
    }

    public override byte[] GetEncodedSendSequenceCounter(long ssc) 
        => new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, (byte)ssc };

}
