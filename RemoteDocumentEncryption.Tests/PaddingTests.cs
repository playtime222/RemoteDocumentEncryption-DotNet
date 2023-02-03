using System;
using System.Diagnostics;
using System.Linq;
using NL.Rijksoverheid.RDW.RemoteDocumentEncryption.Wrapping.Implementation;
using Org.BouncyCastle.Utilities.Encoders;

namespace CaSessionUtilitiesTest;

public class PaddingTests
{
    [InlineData("6E", 4, "6E800000")]
    [InlineData("1122", 2, "1122")]
    [InlineData("1122", 4, "11228000")]
    [InlineData("6E", 16, "6E800000000000000000000000000000")]
    [InlineData("00112233445566778899AABBCCDDEEFF", 16, "00112233445566778899AABBCCDDEEFF")]
    [Theory]
    //Pad if not a multiple of blocksize
    public void Method1(string response, int blockSize, string expected)
    {
        var result = PaddingIso9797.GetPaddedArrayMethod1(Hex.Decode(response), blockSize);
        var resultHex = Hex.ToHexString(result);

        Trace.WriteLine("Actual  : " + resultHex);
        Trace.WriteLine("Expected: " + expected);
        Trace.WriteLine("Size: " + result.Length);
        Assert.Equal(expected, resultHex, true);
    }

    [InlineData("1122", 2, "11228000")]
    [InlineData("1122", 4, "11228000")]
    [InlineData("6E", 4, "6E800000")]
    [InlineData("11223344", 4, "1122334480000000")]
    [InlineData("6E8201D9318201D5300D060804007F0007020202020101300F060A04007F000702020302040201013012060A04007F0007020204020402010202010E30170606", 16, "6E8201D9318201D5300D060804007F0007020202020101300F060A04007F000702020302040201013012060A04007F0007020204020402010202010E3017060680000000000000000000000000000000")]
    [Theory]
    ///Always pad
    public void Method2(string response, int blockSize, string expected)
    {
        var result = PaddingIso9797.GetPaddedArrayMethod2(Hex.Decode(response), blockSize);
        var resultHex = Hex.ToHexString(result);

        Trace.WriteLine("Actual  : " + resultHex);
        Trace.WriteLine("Expected: " + expected);
        Trace.WriteLine("Size: " + result.Length);
        Assert.Equal(expected, resultHex, true);
    }
}