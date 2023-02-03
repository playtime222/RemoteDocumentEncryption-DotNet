using System;
using System.Linq;
using Org.BouncyCastle.Math;

namespace NL.Rijksoverheid.RDW.RemoteDocumentEncryption;

/// <summary>
/// All from DG14. This is the dto.
/// </summary>
public record ChipAuthenticationPublicKeyInfo
{
    [Obsolete("Fallback only?")]
    public string Oid { get; set; }

    //EC or ECDH, ASN.1 DER?
    public byte[] PublicKey { get; set; }


    [Obsolete("Fallback only?")]
    //String? Key OID???
    public BigInteger KeyId { get; set; }
}