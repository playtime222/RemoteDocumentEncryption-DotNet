using System;
using System.Collections.Generic;
using System.Linq;

namespace NL.Rijksoverheid.RDW.RemoteDocumentEncryption.Wrapping.Implementation;

public static class ChipAuthenticationInfo
{
    public static ChipAuthenticationCipherInfo GetCipher(string oid)
        => _CipherInfo.TryGetValue(oid, out var result) ? result : throw new InvalidOperationException("Unsupported OID.");

    private static readonly Dictionary<string, ChipAuthenticationCipherInfo> _CipherInfo = new()
    {
        {ID_CA_DH_3DES_CBC_CBC, new(CipherAlgorithm.DESede,128) },
        {ID_CA_ECDH_3DES_CBC_CBC , new(CipherAlgorithm.DESede, 128) },
        {ID_CA_DH_AES_CBC_CMAC_128 ,  new(CipherAlgorithm.Aes,128) },
        {ID_CA_DH_AES_CBC_CMAC_192 ,  new(CipherAlgorithm.Aes,192) },
        {ID_CA_DH_AES_CBC_CMAC_256 ,  new(CipherAlgorithm.Aes,256) },
        {ID_CA_ECDH_AES_CBC_CMAC_128, new(CipherAlgorithm.Aes,128) },
        {ID_CA_ECDH_AES_CBC_CMAC_192, new(CipherAlgorithm.Aes,192) },
        {ID_CA_ECDH_AES_CBC_CMAC_256, new(CipherAlgorithm.Aes,256) },
    };

    private const string ID_CA_DH_3DES_CBC_CBC =       "0.4.0.127.0.7.2.2.2";
    private const string ID_CA_ECDH_3DES_CBC_CBC =     "0.4.0.127.0.7.2.2.3.2.1";
    private const string ID_CA_DH_AES_CBC_CMAC_128 =   "0.4.0.127.0.7.2.2.3.1.2";
    private const string ID_CA_DH_AES_CBC_CMAC_192 =   "0.4.0.127.0.7.2.2.3.1.3";
    private const string ID_CA_DH_AES_CBC_CMAC_256 =   "0.4.0.127.0.7.2.2.3.1.4";
    private const string ID_CA_ECDH_AES_CBC_CMAC_128 = "0.4.0.127.0.7.2.2.3.2.2";
    private const string ID_CA_ECDH_AES_CBC_CMAC_192 = "0.4.0.127.0.7.2.2.3.2.3";
    private const string ID_CA_ECDH_AES_CBC_CMAC_256 = "0.4.0.127.0.7.2.2.3.2.4";


    /// <summary>
    /// OID of DG14 CA Session Public Key
    /// </summary>
    public static KeyAgreementAlgorithm GetKeyAgreementAlgorithm(string oid)
        => _CipherInfo2.TryGetValue(oid, out var result) ? result : throw new InvalidOperationException("Unsupported OID.");

    private static readonly Dictionary<string, KeyAgreementAlgorithm> _CipherInfo2 = new()
    {
        {ID_CA_DH_3DES_CBC_CBC,       KeyAgreementAlgorithm.DH },
        {ID_CA_ECDH_3DES_CBC_CBC ,    KeyAgreementAlgorithm.ECDH },
        {ID_CA_DH_AES_CBC_CMAC_128 ,  KeyAgreementAlgorithm.DH },
        {ID_CA_DH_AES_CBC_CMAC_192 ,  KeyAgreementAlgorithm.DH },
        {ID_CA_DH_AES_CBC_CMAC_256 ,  KeyAgreementAlgorithm.DH },
        {ID_CA_ECDH_AES_CBC_CMAC_128, KeyAgreementAlgorithm.ECDH },
        {ID_CA_ECDH_AES_CBC_CMAC_192, KeyAgreementAlgorithm.ECDH },
        {ID_CA_ECDH_AES_CBC_CMAC_256, KeyAgreementAlgorithm.ECDH },
    };
}