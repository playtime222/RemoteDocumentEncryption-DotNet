using System;
using System.Linq;

namespace NL.Rijksoverheid.RDW.RemoteDocumentEncryption;

/// <summary>
/// From enrolment, specifically DG14
/// </summary>
public record CaSessionArgs
{
    //caSessionArgs.getCaPublicKeyInfo().getKeyId(),
    //caSessionArgs.getCaPublicKeyInfo().getObjectIdentifier(),
    //caSessionArgs.getCaPublicKeyInfo().getSubjectPublicKey());
    //caSessionArgs.getCaInfo().getObjectIdentifier(),
    public string ProtocolOid { get; set; }
    public ChipAuthenticationPublicKeyInfo PublicKeyInfo { get; set; }
}