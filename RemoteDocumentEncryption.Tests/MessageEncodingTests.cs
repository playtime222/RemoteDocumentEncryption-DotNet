//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Text;
//using NL.Rijksoverheid.RDW.RDE.CaSessionUtilities.Messaging;
//using NL.Rijksoverheid.RDW.RDE.CaSessionUtilities.Messaging.ZipV2;
//using Org.BouncyCastle.Security;
//using Org.BouncyCastle.Utilities.Encoders;

//namespace CaSessionUtilitiesTest;

////Early debugging tests
//public class MessageEncodingTests
//{
//    /// <summary>
//    /// Use to generate tests for Java decoder
//    /// </summary>
//    [Fact]
//    public void RoundTrip()
//    {
//        var messageContentArgs = new MessageContentArgs();
//        messageContentArgs.Add(new FileArgs("argle", Encoding.UTF8.GetBytes("argle...")));
//        messageContentArgs.UnencryptedNote = "note";

//        var rdeMessageDecryptionInfo = new RdeMessageDecryptionInfo
//        {
//            Command = "Now!",
//            PcdPublicKey = "01"
//        };

//        var secretKey = new byte[32];
//        new SecureRandom().NextBytes(secretKey);

//        var encoder = new ZipMessageEncoder();
//        var encoded = encoder.Encode(messageContentArgs, rdeMessageDecryptionInfo, secretKey);

//        Trace.WriteLine("Secret Key      : " + Hex.ToHexString(secretKey));
//        Trace.WriteLine("Encoded message : " + Hex.ToHexString(encoded));
//        Trace.WriteLine("Length :        " + encoded.Length);

//        var decoder = new ZipMessageDecoder();
//        var actualRdeSessionArgs = decoder.DecodeRdeSessionArgs(encoded);
//        Assert.Equal(rdeMessageDecryptionInfo.Command, actualRdeSessionArgs.RdeInfo.Command);
//        Assert.Equal(rdeMessageDecryptionInfo.PcdPublicKey, actualRdeSessionArgs.RdeInfo.PcdPublicKey);
//        Assert.Equal(16, Hex.Decode(actualRdeSessionArgs.Iv).Length);

//        //Pretend we threw the actualRdeSessionArgs at a phone and the MRTD

//        var message = decoder.Decode(secretKey);
//        Assert.Equal("note", message.Note);
//        Assert.Equal("argle", message.Files[0].Filename);
//        Assert.Equal("argle...", Encoding.UTF8.GetString(message.Files[0].Content));
//    }
//}