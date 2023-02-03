//using System;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Text;
//using NL.Rijksoverheid.RDW.RDE.CaSessionUtilities.Messaging;
//using NL.Rijksoverheid.RDW.RDE.CaSessionUtilities.Messaging.ZipV2;
//using Org.BouncyCastle.Crypto.Paddings;
//using Org.BouncyCastle.Security;
//using Org.BouncyCastle.Utilities.Encoders;

//namespace CaSessionUtilitiesTest;

//public class Step6_EncodeMessage
//{
//    //[InlineData(Step5_EncryptionKeyFromRbResponse.Case1_Length10_SecretKey)]
//    //[InlineData(Step5_EncryptionKeyFromRbResponse.Case1_Length100_SecretKey)]
//    //[InlineData(Step5_EncryptionKeyFromRbResponse.Case2_Length10_SecretKey)]
//    //[InlineData(Step5_EncryptionKeyFromRbResponse.Case2_Length100_SecretKey)]
//    //[InlineData(Step5_EncryptionKeyFromRbResponse.Case3_Length10_SecretKey)]
//    //[InlineData(Step5_EncryptionKeyFromRbResponse.Case3_Length100_SecretKey)]
//    [InlineData("J 1")]
//    [InlineData("Case 5")]
//    [Theory]
//    public void RoundTrip(string caseName)
//    {
//        var testCase = TestCases.Items[caseName];
//        Test(testCase.MessageEncryptionKey, testCase.EncodedMessage);
//    }

//    private byte[] Test(string secretKeyHex, string expectedHex)
//    {
//        var messageContentArgs = new MessageContentArgs();
//        messageContentArgs.Add(new FileArgs("argle", Encoding.UTF8.GetBytes("argle...")));
//        messageContentArgs.UnencryptedNote = "note";

//        var rdeMessageDecryptionInfo = new RdeMessageDecryptionInfo
//        {
//            Command = "Now!",
//            PcdPublicKey = "01"
//        };

//        var secretKey = Hex.Decode(secretKeyHex);
//        var encoder = new ZipMessageEncoder();
//        var actual = encoder.Encode(messageContentArgs, rdeMessageDecryptionInfo, secretKey);

//        var actualHex = Hex.ToHexString(actual);
//        Trace.WriteLine($"Expected: {expectedHex}");
//        Trace.WriteLine($"Actual  : {actualHex}");

//        //Do not compare the files - this is due to differences in date/time stamps and minor but compatible differences between Java and .NET archives.
//        Step7_DecodeMessage.Decode(secretKeyHex, actualHex);
//        return actual;
//    }


//    [Fact]
//    public void Generate()
//    {
//        var secretKey = new byte[32];
//        var r = new SecureRandom();
//        r.NextBytes(secretKey);
//        Trace.WriteLine($"SecretKey  : {Hex.ToHexString(secretKey)}");
//        var actual = Test(Hex.ToHexString(secretKey), "Generating...");
//        //File.WriteAllBytes("D:\\CSharpMessage.zip", actual);
//    }
//}