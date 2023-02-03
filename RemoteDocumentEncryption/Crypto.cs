using System;
using System.Linq;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace NL.Rijksoverheid.RDW.RDE.CaSessionUtilities
{
    public static class Crypto
    {
        private static byte[] GetCipherText(string alg, byte[] key, byte[] iv, byte[] plainText)
        {
            var cipher = CipherUtilities.GetCipher(alg);
            cipher.Init(true, new ParametersWithIV(new KeyParameter(key), iv));
            return cipher.DoFinal(plainText);
        }

        /// <summary>
        /// Encrypting messages
        /// </summary>
        public static byte[] GetAesCbcPkcs5PaddingCipherText(byte[] key, byte[] iv, byte[] plainText)
            => GetCipherText("AES/CBC/PKCS5Padding", key, iv, plainText);

        /// <summary>
        /// Used for AES response encoding
        /// </summary>
        public static byte[] GetAesCbcNoPaddingCipherText(byte[] key, byte[] iv, byte[] plainText)
            => GetCipherText("AES/CBC/NoPadding", key, iv, plainText);

        public static byte[] GetDeSedeCbcNoPaddingCipherText(byte[] key, byte[] iv, byte[] plainText)
            => GetCipherText("DESede/CBC/NoPadding", key, iv, plainText);


        /// <summary>
        /// Decrypting messages
        /// </summary>
        public static byte[] GetAesCbcPkcs5PaddingPlainText(byte[] key, byte[] iv, byte[] cipherText)
        {
            var cipher = CipherUtilities.GetCipher("AES/CBC/PKCS5Padding");
            cipher.Init(false, new ParametersWithIV(new KeyParameter(key), iv));
            return cipher.DoFinal(cipherText);
        }

        /// <summary>
        /// Generating IV for AES response encoding.
        /// </summary>
        public static byte[] GetAesEcbNoPaddingCipherText(byte[] key, byte[] data)
        {
            var cipher = CipherUtilities.GetCipher("AES/ECB/NoPadding");
            cipher.Init(true, new KeyParameter(key));
            return cipher.DoFinal(data);
        }

        /// <summary>
        /// AES command and response encoding
        /// </summary>
        public static byte[] GetAesCMac(/*string alg,*/ byte[] key, byte[] data)
        {
            var mac = new CMac(new AesEngine(), 128);
            var macKey = new KeyParameter(key);

            mac.Init(macKey);
            mac.BlockUpdate(data, 0, data.Length);

            var result = new byte[mac.GetMacSize()];
            mac.DoFinal(result, 0);
            return result;
        }

        /// <summary>
        /// Used for verifying message files.
        /// </summary>
        public static byte[] GetAesGMac(/*string alg,*/ byte[] key, byte[] iv, byte[] data)
        {
            var mac = new GMac(new GcmBlockCipher(new AesEngine())); //TODO specify size?
            mac.Init(new ParametersWithIV(new KeyParameter(key), iv));
            mac.BlockUpdate(data, 0, data.Length);
            var result = new byte[mac.GetMacSize()];
            mac.DoFinal(result, 0);
            return result;
        }

        /// <summary>
        /// Used for verifying message files.
        /// TODO change to return bool?
        /// </summary>
        public static void VerifyAesGMac(/*string alg,*/ byte[] key, byte[] iv, byte[] data, byte[] value)
        {
            var cipher = new GcmBlockCipher(new AesEngine());
            cipher.Init(false, new ParametersWithIV(new KeyParameter(key), iv));
            cipher.ProcessAadBytes(data, 0, data.Length);
            var unusedBuffer = new byte[2048];
            cipher.ProcessBytes(value, 0, value.Length, unusedBuffer, 0);
            cipher.DoFinal(unusedBuffer, 0); //Throws if bad gmac
        }

        public static byte[] GetIso9797Alg3Mac(byte[] key, byte[] data)
        {
            var mac = new ISO9797Alg3Mac(new DesEngine()); //TODO specify size?
            mac.Init(new KeyParameter(key));
            mac.BlockUpdate(data, 0, data.Length);
            var result = new byte[mac.GetMacSize()];
            mac.DoFinal(result, 0);
            return result;
        }

        /// <summary>
        /// TODO use a proper KDF, preferably without salt unless response needs stretching (but why cos it already uses DH - possible with short read lengths?)
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static byte[] GetAes256SecretKeyFromResponse(byte[] response)
        {
            var digest = new Sha256Digest();
            digest.BlockUpdate(response, 0, response.Length);
            var result = new byte[digest.GetDigestSize()];
            digest.DoFinal(result, 0);
            return result;
        }
    }
}
