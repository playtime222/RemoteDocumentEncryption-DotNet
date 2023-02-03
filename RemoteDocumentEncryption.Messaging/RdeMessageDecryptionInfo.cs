using System;
using System.Linq;

namespace NL.Rijksoverheid.RDW.RemoteDocumentEncryption.Messaging;

public class RdeMessageDecryptionInfo
{
    /// <summary>
    /// Display name for the receiver's document used during enrolment
    /// </summary>
    public string DocumentDisplayName { get; set; }

    /// <summary>
    /// From the DG14 info.
    /// </summary>
    public string CaProtocolOid { get; set; }

    /// <summary>
    /// Hex encoded
    /// AKA Ephemeral Key Z
    /// </summary>
    public string PcdPublicKey { get; set; } //from EAC CA session

    /// <summary>
    /// Hex encoded
    /// Encrypted RB command 
    /// </summary>
    public string Command { get; set; } //from EAC CA session
}