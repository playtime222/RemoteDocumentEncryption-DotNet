//using System;
//using System.Linq;
//using System.Text;
//using NL.Rijksoverheid.RDW.RDE.CaSessionUtilities.Messaging.ZipV2;
//using Org.BouncyCastle.Utilities.Encoders;

//namespace CaSessionUtilitiesTest;

//public class Step7_DecodeMessage
//{
//    [InlineData("J 1")]
//    [InlineData("Java Message Case 1 Length 10")]
//    [InlineData("Java Message Case 1 Length 100")]
//    [InlineData("Java Message Case 2 Length 10")]
//    [InlineData("Java Message Case 2 Length 100")]
//    [InlineData("Java Message Case 3 Length 10")]
//    [InlineData("Java Message Case 3 Length 100")]
//    [Theory]
//    public void Test(string caseName)
//    {
//        var testCase = TestCases.Items[caseName];
//        Decode(testCase.MessageEncryptionKey, testCase.EncodedMessage);
//    }

//    public static void Decode(string secretKeyHex, string messageHex)
//    {
//        var secretKey = Hex.Decode(secretKeyHex);
//        var decoder = new ZipMessageDecoder();
//        var actualRdeSessionArgs = decoder.DecodeRdeSessionArgs(Hex.Decode(messageHex));
//        Assert.Equal(16, Hex.Decode(actualRdeSessionArgs.Iv).Length);
//        var message = decoder.Decode(secretKey);
//        Assert.Equal("note", message.Note);
//        Assert.Equal("argle", message.Files[0].Filename);
//        Assert.Equal("argle...", Encoding.UTF8.GetString(message.Files[0].Content));
//    }
//}