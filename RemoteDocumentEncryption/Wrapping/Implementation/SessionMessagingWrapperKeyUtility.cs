using System;
using System.Linq;
using Org.BouncyCastle.Crypto.Digests;

namespace NL.Rijksoverheid.RDW.RemoteDocumentEncryption.Wrapping.Implementation;

public static class SessionMessagingWrapperKeyUtility
{
    public const int ENC_MODE = 1;
    public const int MAC_MODE = 2;

    public static byte[] DeriveKey(byte[] keySeed, ChipAuthenticationCipherInfo cipherInfo, int mode)
        => DeriveKey(keySeed, cipherInfo.Algorithm, cipherInfo.KeyLength, mode);
    private static byte[] DeriveKey(byte[] keySeed, CipherAlgorithm cipherAlg, int keyLength, int mode)
    {
        var digest = GetDigest(cipherAlg, keyLength);
        digest.BlockUpdate(keySeed, 0, keySeed.Length);
        digest.BlockUpdate(new byte[] { 0x00, 0x00, 0x00, (byte)mode }, 0, 4);
        var hashResult = new byte[digest.GetDigestSize()];
        digest.DoFinal(hashResult, 0);

        if (cipherAlg == CipherAlgorithm.DESede)
        {
            switch (keyLength)
            {
                case 112:
                case 128:
                    var keyBytes = new byte[24];
                    Array.Copy(hashResult, 0, keyBytes, 0, 8); /* E  (octets 1 to 8) */
                    Array.Copy(hashResult, 8, keyBytes, 8, 8); /* D  (octets 9 to 16) */
                    Array.Copy(hashResult, 0, keyBytes, 16, 8); /* E (again octets 1 to 8, i.e. 112-bit 3DES key) */
                    return keyBytes;
                default:
                    throw new InvalidOperationException("DESede with 128 bit key length only");
            }
        }

        if (cipherAlg == CipherAlgorithm.Aes)
        {
            switch (keyLength)
            {
                case 128:
                case 192:
                case 256:
                    var keyBytes = new byte[keyLength / 8]; /* NOTE: 256 = 32 * 8 */
                    Array.Copy(hashResult, 0, keyBytes, 0, keyBytes.Length);
                    return keyBytes;
                default:
                    throw new InvalidOperationException("AES with 128, 192 or 256 bit key length");
            }
        }

        throw new InvalidOperationException();
    }

    private static GeneralDigest GetDigest(CipherAlgorithm cipherAlg, int keyLength)
    {
        if (CipherAlgorithm.DESede == cipherAlg || 
            CipherAlgorithm.Aes == cipherAlg && keyLength == 128)
            return new Sha1Digest();
        
        if (CipherAlgorithm.Aes == cipherAlg && keyLength is 192 or 256)
            return new Sha256Digest();

        throw new InvalidOperationException("Unsupported cipher algorithm or key length.");
    }
}