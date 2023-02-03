using System;
using System.Linq;

namespace CaSessionUtilitiesTest;

/// <summary>
/// All current tests based on DG14 from SPEC2014
/// </summary>
public class TestCase
{
    public string PcdPrivateKey { get; init; }
    public string PcdPublicKey { get; init; }
    public string SharedSecret { get; init; }
    public string KsEnc { get; init; }
    public string KsMac { get; init; }
    public int File { get; init; }
    public int Length { get; init; }
    public string CommandApdu { get; init; }
    public string WrappedCommandApdu { get; init; }
    public string EncryptedPaddedResponse { get; init; }
    public string WrappedResponse{ get; init; }
    public string MessageEncryptionKey { get; init; }
    public string EncodedMessage { get; init; }
}