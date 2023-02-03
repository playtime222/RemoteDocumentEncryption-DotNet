using System;
using System.Diagnostics;
using System.Linq;
using NL.Rijksoverheid.RDW.RDE.CaSessionUtilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace CaSessionUtilitiesTest;

public class Step5_EncryptionKeyFromRbResponse
{
    [InlineData("J 1")]
    [InlineData("Case 5")]
    [InlineData("Case 6")]
    [InlineData("Case 7")]
    [InlineData("Case 8")]
    [Theory]
    public void Test(string caseName)
    {
        var testCase = TestCases.Items[caseName];
        Convert(testCase.WrappedResponse, testCase.MessageEncryptionKey);
    }

    private void Convert(string responseHex, string expectedKeyHex) 
    {
        var key = Crypto.GetAes256SecretKeyFromResponse(Hex.Decode(responseHex));
        Trace.WriteLine("RB Result   : " + responseHex);
        Trace.WriteLine("Actual   : " + Hex.ToHexString(key));
        Trace.WriteLine("Expected : " + expectedKeyHex);
        Assert.Equal(Hex.Decode(expectedKeyHex), key);
    }
}