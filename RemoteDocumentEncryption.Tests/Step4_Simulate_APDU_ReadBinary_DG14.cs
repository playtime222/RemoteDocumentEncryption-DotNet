using System;
using System.Diagnostics;
using System.Linq;
using NL.Rijksoverheid.RDW.RDE.CaSessionUtilities.Wrapping;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace CaSessionUtilitiesTest;

public class Step4_Simulate_APDU_ReadBinary_DG14
{
    //public const string Case1_RB_Length10 = "8711011342ceb25180b830b9d709ce38f73321990290008e0870f137bd85dcca6b9000";
    //public const string Case1_RB_Length100 = "8771011f7aaf02f3df52cfcdfb38fbb37aa55f743a8b79edc11d9cf1ce0da10c277611951c2af8d29497939f78c4816b6c0169a0a7b02e59d6702defa176312c43ceb3dddc749c42c34f289f397e6ae150b93c2270b25e7ff8a4f43a8b00046e626350299b8d3b457481a11a807c0969da3cf8990290008e082bdd07f44a1e3aea9000";
    //public const string Case2_RB_Length10 = "87110114f49c70e1994c1a41d9abb7f8a017db990290008e080edb80e3652153869000";
    //public const string Case2_RB_Length100 = "8771011fe8fb70b52d86ebfe51a15ac340646b529a2e4e95711410865769015cdfaa8f259049c6d6d51a6e64fb9608278de1c383e844e4824b2c861cd4e43e9ac8cf94077349c4d348d21281d928e4a4324a8effa785934e717743129c485c5b826b95b08aeccae0ee6cfd635e4a908bd8ca64990290008e088010e187203d9a739000";
    //public const string Case3_RB_Length10 = "8711013355dcb2c385b18fa1e3695dffe9d865990290008e08e8b2e9db387c8ac69000";
    //public const string Case3_RB_Length100 = "8771018ff89cd5f96169ae5abcf43a4dc5dec89c1df82be43c2f910b4b1089b943e5b7001b22484b4df3a876745f72e16dccb7b6973c805343116e63fff0792aea81fa6c31ee1a94478387cc3f2841e97b2252c9cd4b1c11a37fca41cae0e039005b72788bf6b5eb081dc2a11cedd003ee4491990290008e08fcddab640c22d1989000";
    //public const string Case5_RB_Length64 = "875101DC91E07F07518888E25A8B7A2814893D10E1D4FCCBD9A5C8920807F6F05EFA47897954F61CED057F1E2E04C2A912A12DD71E5259A367838EE2422BA070A7F103BD68BDF8D2F7E8C83B0B2C8EFA239AAD990290008E0800B92437B38A83AC9000";
    //public const string Case6_RB_Length63 = "87410109CD04E4E0368850A9F0881449FF6D31A17F3FC58F1A2B37765FB515EDC1C8393A82ECC7BDA506A8FAC89411BD805A46979CFCB7079C01102EA6ED4F215E3869990290008E08B222F87E8B5010749000";
    //[InlineData("New 5", Step2_KsEncAndKsMacFromSharedSecret.Case5_KsEnc, Step2_KsEncAndKsMacFromSharedSecret.Case5_KsMac, 64, Case5_RB_Length64)]

    //[InlineData("New 6", "491d608bd275efded22349fd23a8caf5afb40e73c9f777c8ed138afa940f374d", "51f74babf372099fb6c68f83fa73df5602e120d3d62147581f9391ef6c24154c", 63, Case6_RB_Length63)]
    //[InlineData("New 7", "b80c2753e73dab799e08e7708bf9673d872a45c43202a4f8d9dde8e203ec33a3", "3dcad036dbf2e4225a6c5e0fb50bb5da5d13b915ba8460db8219b557deb20d43", 1,  "8711014017BC9CDE322167C6A7ECC1691E8E4C990290008E089C79405D268157B29000")]
    //[InlineData("New 8", "860aa1583594ced261f420011db1d753d65d37d9bab72ee60cfd632b966037f4", "f5bf5c920393f498f91971aa40b874db85003bb52c9db79d06d2219c453a8cc9", 25, "8721017EE98D02332EFCC722FF7CC29010F8906810BFD8BDE4936450D6C4C9DCA7AB85990290008E08F9466AF0F3E29C269000")]
    //[InlineData("New 18", "5ae1146e2c6c9e0c1385bb4af557bb784f89b8ceb49a1514bcefb0c4fa361230", "5d49c9f123ef9e448ab08cfb44ff9211fb5f14e188eb1e4f928f91b8eaf73467", 477, "878201E10184C8EA158484A24304D297D028326400B0694DA3A5922E1B83FC81217126E23CAB70AB55C3B94FE185E11B52DADFD6F652FFE33913FC234D3B6AE865A0B60A014AA9F4D895FA1A60CEA647FF440BC31F68E99FB1B6A94D8A33F355695BB2F472094C22E4B01722E2163528E9654D1D7B009B5DE266598F30F2373213D5F0E1614D3F797420F9B7ADDE8156FBD8AB0F7006310770C36600E70DB3A308596841838514A564834E91A98D552BB1476312737F2D48AB4CD367D42DBB89249C48543451180114358F9FFE2CE6F6B9290BBDCC9A634EA46B6C1EB349E2D8CAA7146CA96E5C29A45250C7F7FAB877A74DE9F86609CB1D4A88C3ED781BA55425F3A270BFC1284F238C722DE6CB45BF0C1AEC98961CD1E1B4013646711CE6BF4A1DA96083D851CC36E5ECB762B2A0EC106E3A791F513D276EE7779164949A454AFA93AE7183F2D7D3509359309DC3B35B4CEB1F49B4486E2F65B7AF53C7021A3CCE6130673F5E4E408C4BB3FE9A0B1958090A6E95D0F56200138C39FFC2CB787DFECA24047826971EDEDF066B1B1F397A14F4CCDB384DA0953202A91137302A3827F9F376E1B5EE1B9959A539C26E330B898A537518AFC6D7DBA3CDD1100AEC3C1983ADA92C10554C2949942A45ED5CF433FAA286DB66C6268B166DAE24BF9D4987A73C8B990290008E080E83F030001460C39000")]
    //[Theory]
    //public void EncodeResponse(string name, string ksEncString, string ksMacString, int requestedLength, string expectedWrappedResponse)
    //{
    //    NewMethod(ksEncString, ksMacString, requestedLength, expectedWrappedResponse);
    //}

    [InlineData("J 1")]
    [InlineData("Case 5")]
    [InlineData("Case 6")]
    [InlineData("Case 7")]
    [InlineData("Case 8")]
    [Theory]
    public void Test(string caseName)
    {
        var testCase = TestCases.Items[caseName];
        NewMethod(testCase.KsEnc, testCase.KsMac, testCase.Length, testCase.WrappedResponse);
    }

    private static void NewMethod(string ksEncString, string ksMacString, int requestedLength,
        string expectedWrappedResponse)
    {
        var ksEnc = Hex.Decode(ksEncString);
        var ksMac = Hex.Decode(ksMacString);
        var encoder = new ResponseEncoder(new AesSecureMessagingWrapper(ksEnc, ksMac));
        var result = encoder.Write(Arrays.CopyOf(Hex.Decode(Spec2014Content.DG14Hex), requestedLength));

        Trace.WriteLine("Actual  : " + Hex.ToHexString(result));
        Trace.WriteLine("Expected: " + expectedWrappedResponse.ToLower());

        var encodedActual = Hex.ToHexString(result);

        Assert.StartsWith("87", encodedActual); //Start tag
        var substring =
            encodedActual.Substring(encodedActual.Length - 32); //9902 + 9000 + 8e0b + 8 bytes (= 16 char) + 9000 again
        Debug.WriteLine($"Mac and block delimiter extract: {substring}");
        Assert.StartsWith("990290008e08", substring); //End of data block, start of MAC
        Assert.EndsWith("9000", substring); //Ends of block

        Assert.Equal(Hex.Decode(expectedWrappedResponse), result);
    }
}