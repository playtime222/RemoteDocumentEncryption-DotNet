using System;
using System.Linq;
using NL.Rijksoverheid.RDW.RDE.CaSessionUtilities.Wrapping;

namespace NL.Rijksoverheid.RDW.RDE.CaSessionUtilities;

public record RdeEACCAResult
{
    public byte[] PcdPublicKey { get; }
    public SecureMessagingWrapper Wrapper { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pcdPublicKey">Something that can be encoded that Java understands</param>
    /// <param name="wrapper"></param>
    /// <param name="debugInfo"></param>
    public RdeEACCAResult(byte[] pcdPublicKey, SecureMessagingWrapper wrapper)
    {
        PcdPublicKey = pcdPublicKey;
        Wrapper = wrapper;
    }
}