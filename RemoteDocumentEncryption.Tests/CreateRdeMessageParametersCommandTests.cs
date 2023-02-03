//using System;
//using System.Diagnostics;
//using System.Text;
//using Newtonsoft.Json;
//using NL.Rijksoverheid.RDW.RDE.CaSessionUtilities;
//using NL.Rijksoverheid.RDW.RDE.CaSessionUtilities.Messaging;
//using NL.Rijksoverheid.RDW.RDE.CaSessionUtilities.Messaging.ZipV2;
//using Org.BouncyCastle.Asn1.X9;
//using Org.BouncyCastle.Security;
//using Org.BouncyCastle.Utilities;
//using Org.BouncyCastle.Utilities.Encoders;

//namespace CaSessionUtilitiesTest;


////TODO Need test data
//public class CreateRdeMessageParametersCommandTests
//{
//    //[Fact]
//    //public void DumpCurveNames()
//    //{
//    //    foreach (string i in ECNamedCurveTable.Names)
//    //    {
//    //        if (i.Contains("brain"))
//    //            Trace.WriteLine(i);
//    //    }
//    //}


//    //From SPECI2014 passport
//    private const string HexEncodedDg14 = "6E8201D9318201D5300D060804007F0007020202020101300F060A04007F000702020302040201013012060A04007F0007020204020402010202010E30170606678108010105020101060A04007F0007010104010330820184060904007F000702020102308201753082011D06072A8648CE3D020130820110020101303406072A8648CE3D0101022900D35E472036BC4FB7E13C785ED201E065F98FCFA6F6F40DEF4F92B9EC7893EC28FCD412B1F1B32E27305404283EE30B568FBAB0F883CCEBD46D3F3BB8A2A73513F5EB79DA66190EB085FFA9F492F375A97D860EB40428520883949DFDBC42D3AD198640688A6FE13F41349554B49ACC31DCCD884539816F5EB4AC8FB1F1A604510443BD7E9AFB53D8B85289BCC48EE5BFE6F20137D10A087EB6E7871E2A10A599C710AF8D0D39E2061114FDD05545EC1CC8AB4093247F77275E0743FFED117182EAA9C77877AAAC6AC7D35245D1692E8EE1022900D35E472036BC4FB7E13C785ED201E065F98FCFA5B68F12A32D482EC7EE8658E98691555B44C5931102010103520004710DA6DAB5B770920D3D4D6807B02A13059BEFB4926E2D00CFDE4B4471571473A582934BBE92059800663578C83419E3563FE3E8AF3AE58B521D3741693C9CE19B312392CB00F59AF086863186706396";
//    private const string HexEncodedCaProtocolPublicKey = "308201753082011d06072a8648ce3d020130820110020101303406072a8648ce3d0101022900d35e472036bc4fb7e13c785ed201e065f98fcfa6f6f40def4f92b9ec7893ec28fcd412b1f1b32e27305404283ee30b568fbab0f883ccebd46d3f3bb8a2a73513f5eb79da66190eb085ffa9f492f375a97d860eb40428520883949dfdbc42d3ad198640688a6fe13f41349554b49acc31dccd884539816f5eb4ac8fb1f1a604510443bd7e9afb53d8b85289bcc48ee5bfe6f20137d10a087eb6e7871e2a10a599c710af8d0d39e2061114fdd05545ec1cc8ab4093247f77275e0743ffed117182eaa9c77877aaac6ac7d35245d1692e8ee1022900d35e472036bc4fb7e13c785ed201e065f98fcfa5b68f12a32d482ec7ee8658e98691555b44c5931102010103520004710da6dab5b770920d3d4d6807b02a13059befb4926e2d00cfde4b4471571473a582934bbe92059800663578c83419e3563fe3e8af3ae58b521d3741693c9ce19b312392cb00f59af086863186706396";
//    //private const string PublicKeyOid = "0.4.0.127.0.7.2.2.1.2";
//    private const string CaProtocolOid = "0.4.0.127.0.7.2.2.3.2.4";


//    /// <summary>
//    /// Generates test data for cross-platform Zip message tests.
//    /// Note these tests only 
//    /// </summary>
//    [Fact]
//    public void GenerateZipMessage()
//    {

//        //Sanity check
//        PublicKeyFactory.CreateKey(Hex.Decode(HexEncodedCaProtocolPublicKey));

//        var args = new RdeMessageCreateArgs
//        {

//            CaSessionArgs = new CaSessionArgs
//            {

//                ProtocolOid = CaProtocolOid,
//                PublicKeyInfo = new ChipAuthenticationPublicKeyInfo
//                {
//                    //KeyId = ,
//                    PublicKey = Hex.Decode(HexEncodedCaProtocolPublicKey),
//                    // ProtocolOid = ,
//                }
//            },
//            FileContent = Hex.Decode(HexEncodedDg14),
//            FileShortId = 14,
//            ReadLength = 64
//        };

//        var actual = new CreateRdeMessageParametersCommand().Execute(args);

//        Trace.WriteLine("DUMP RDE Message Parameters >>>>>>>");
//        //TODO encodes byte arrays as Base64...
//        Trace.WriteLine(JsonConvert.SerializeObject(actual, Formatting.Indented));
//        Trace.WriteLine("<<<<<< DUMP RDE Message Parameters");

//        var secretKey = Crypto.GetAes256SecretKeyFromResponse(actual.WrappedResponse);
//        Trace.WriteLine("SecretKey: " + Hex.ToHexString(secretKey));

//        var messageContentArgs = new MessageContentArgs();
//        messageContentArgs.Add(new FileArgs("argle", Encoding.UTF8.GetBytes("argle...")));
//        messageContentArgs.UnencryptedNote = "note";

//        var encoder = new ZipMessageEncoder();
//        var rdeMessageDecryptionInfo = actual.ToRdeMessageDecryptionInfo();
//        rdeMessageDecryptionInfo.CaProtocolOid = args.CaSessionArgs.ProtocolOid;
//        rdeMessageDecryptionInfo.DocumentDisplayName = "Default";

//        var encoded = encoder.Encode(messageContentArgs, rdeMessageDecryptionInfo, secretKey);
//        Trace.WriteLine("Hex Encoded Message: " + Hex.ToHexString(encoded));
//    }
//}