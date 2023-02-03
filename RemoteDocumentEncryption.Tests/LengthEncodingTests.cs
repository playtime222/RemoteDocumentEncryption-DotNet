using System;
using System.Linq;
using NL.Rijksoverheid.RDW.RemoteDocumentEncryption.Wrapping;
using Org.BouncyCastle.Utilities.Encoders;

namespace CaSessionUtilitiesTest;

public class LengthEncodingTests
{

    [InlineData(1, "0201")]
    [InlineData(64, "4101")]
    [InlineData(65, "4201")]
    [InlineData(254, "81ff01")]
    [InlineData(255, "82010001")]
    [InlineData(256, "82010101")]
    [InlineData(1024, "82040101")]
    [Theory]
    public void Do87LengthEncoding(int length, string expected)
    {
        Assert.Equal(expected, Hex.ToHexString(ResponseEncoder.GetEncodedDo87Size(length)));
    }
}