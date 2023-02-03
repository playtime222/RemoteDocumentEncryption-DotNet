using System;
using System.Linq;

namespace NL.Rijksoverheid.RDW.RemoteDocumentEncryption;

public record RdeMessageParameters
{
    /// <summary>
    /// CA Session PCD Public Key
    /// Used during decrypt
    /// </summary>
    public byte[] EphemeralPublicKey { get; set; }

    /// <summary>
    /// Read binary file command wrapped using a CA Session.
    /// Note a wrapped command is not encrypted - it is encoded and has a MAC appended.
    /// </summary>
    public byte[] WrappedCommand { get; set; }
    
    /// <summary>
    /// Wrapped result of sending the Wrapped Command to the MRTD.
    /// Seed for generating message cipher key
    /// </summary>
    public byte[] WrappedResponse { get; set; }
}