using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NL.Rijksoverheid.RDW.RemoteDocumentEncryption.Wrapping.Implementation;
using Org.BouncyCastle.Utilities.Encoders;

namespace CaSessionUtilitiesTest;

//Seed:  8f54c4f830fdd59f0d20f5118d19d9f03295319180da36ba812353b108a9fb43ad8ddb4505da89df111db04ede91934d026a404999fea9d81fabfdcb5df34cc00c6e7f16a7bb50d93ea6461ebbd9ad94
//alg:  AES
//format:  RAW
//raw:  e71666984de8c71293a4b8363850128f9f9350078ae2faaa6dc0da2c464e9ea1
public class Step2_KsEncAndKsMacFromSharedSecret
{
    [InlineData("J 1")]
    [InlineData("Case 2")]
    [InlineData("Case 3")]
    [InlineData("Case 4")]
    [InlineData("Case 5")]
    [InlineData("Case 6")]
    [InlineData("Case 7")]
    [InlineData("Case 8")]
    [Theory]
    public void GenerateSharedSecretTests2(string caseName)
    {
        var testCase = TestCases.Items[caseName];

        var ksEnc = SessionMessagingWrapperKeyUtility.DeriveKey(Hex.Decode(testCase.SharedSecret), new(CipherAlgorithm.Aes, 256), SessionMessagingWrapperKeyUtility.ENC_MODE);
        var ksMac = SessionMessagingWrapperKeyUtility.DeriveKey(Hex.Decode(testCase.SharedSecret), new(CipherAlgorithm.Aes, 256), SessionMessagingWrapperKeyUtility.MAC_MODE);

        Trace.WriteLine("Seed         : " + testCase.SharedSecret);
        Trace.WriteLine("KsEnc Actual : " + Hex.ToHexString(ksEnc));
        Trace.WriteLine("    Expected : " + testCase.KsEnc);
        Trace.WriteLine("KsMac Actual : " + Hex.ToHexString(ksMac));
        Trace.WriteLine("    Expected : " + testCase.KsMac);

        Assert.Equal(Hex.Decode(testCase.KsEnc), ksEnc);
        Assert.Equal(Hex.Decode(testCase.KsMac), ksMac);
    }

}