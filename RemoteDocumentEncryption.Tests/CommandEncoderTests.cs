using System.Diagnostics;
using NL.Rijksoverheid.RDW.RDE.CaSessionUtilities;
using NL.Rijksoverheid.RDW.RDE.CaSessionUtilities.Wrapping;
using NL.Rijksoverheid.RDW.RDE.CaSessionUtilities.Wrapping.Implementation;
using Org.BouncyCastle.Utilities.Encoders;
namespace CaSessionUtilitiesTest;

//Early debugging tests
public class CommandEncoderTests
{
    //public const string Case1_Length10_Command = "0cb08e000d97010a8e08185eade9e13a4fd300";
    //public const string Case1_Length100_Command = "0cb08e000d9701648e0824d9d59ddc0a48f300";
    //public const string Case2_Length10_Command = "0cb08e000d97010a8e08e2db7bbfe2a5434300";
    //public const string Case2_Length100_Command = "0cb08e000d9701648e08b6c9c04dc807bccd00";
    //public const string Case3_Length10_Command = "0cb08e000d97010a8e086b8d52f4b8bc826700";
    //public const string Case3_Length100_Command = "0cb08e000d9701648e084337d3b671b2d0bf00";

    /// <summary>
    /// Test cases generated RDE-LIB - nl.rijksoverheid.rdw.rde.TestDataGeneratorTests.AesCommandApduTest //TODO may move...
    /// </summary>
    /// <param name="requestedLength"></param>
    /// <param name="hexPlain"></param>
    /// <param name="expected"></param>
    /// <param name="ksEncString"></param>
    /// <param name="ksMacString"></param>
    //Proves that ksEnc is not used when encoding a RB
    [InlineData(1, "00b08e0001", "0cb08e000d9701018e08fb1cc883d1a9b32d00", "0000000000000000000000000000000000000000000000000000000000000000", "ADCBA368FD14A836908252EF76D09BAD2766C5FFB2FE7857F468676FC4B293E0")]
    [InlineData(1, "00b08e0001", "0cb08e000d9701018e08fb1cc883d1a9b32d00", "7319D1537EF2FE5CB46AFCFF2DF33B521F3A0C4FA92212D98EB49D9CD6BB8916", "ADCBA368FD14A836908252EF76D09BAD2766C5FFB2FE7857F468676FC4B293E0")]
    [InlineData(65, "00b08e0041", "0cb08e000d9701418e08cea04768ff4fd54d00", "7319D1537EF2FE5CB46AFCFF2DF33B521F3A0C4FA92212D98EB49D9CD6BB8916", "ADCBA368FD14A836908252EF76D09BAD2766C5FFB2FE7857F468676FC4B293E0")]
    [InlineData(254, "00b08e00fe", "0cb08e000d9701fe8e083fdc947adcea421200", "7319D1537EF2FE5CB46AFCFF2DF33B521F3A0C4FA92212D98EB49D9CD6BB8916", "ADCBA368FD14A836908252EF76D09BAD2766C5FFB2FE7857F468676FC4B293E0")]
    [InlineData(255, "00b08e00ff", "0cb08e000d9701ff8e085d83100cf76663d300", "7319D1537EF2FE5CB46AFCFF2DF33B521F3A0C4FA92212D98EB49D9CD6BB8916", "ADCBA368FD14A836908252EF76D09BAD2766C5FFB2FE7857F468676FC4B293E0")]
    [InlineData(256, "00b08e0000", "0cb08e000d9701008e08b16e78e0d1d0023f00", "7319D1537EF2FE5CB46AFCFF2DF33B521F3A0C4FA92212D98EB49D9CD6BB8916", "ADCBA368FD14A836908252EF76D09BAD2766C5FFB2FE7857F468676FC4B293E0")]
    //TODO Does the transceive length of 256 affect the response?
    [InlineData(257, "00b08e00000101", "0cb08e0000000e970201018e08be975ead600e26f70000", "7319D1537EF2FE5CB46AFCFF2DF33B521F3A0C4FA92212D98EB49D9CD6BB8916", "ADCBA368FD14A836908252EF76D09BAD2766C5FFB2FE7857F468676FC4B293E0")]
    [InlineData(300, "00b08e0000012c", "0cb08e0000000e9702012c8e08bd308fb58e028d140000", "7319D1537EF2FE5CB46AFCFF2DF33B521F3A0C4FA92212D98EB49D9CD6BB8916", "ADCBA368FD14A836908252EF76D09BAD2766C5FFB2FE7857F468676FC4B293E0")]

    //[InlineData(10,  "00b08e000a", Case1_Length10_Command, Step2_KsEncAndKsMacFromSharedSecret.Case1_KsEnc, Step2_KsEncAndKsMacFromSharedSecret.Case1_KsMac)]
    //[InlineData(100, "00b08e0064", Case1_Length100_Command, Step2_KsEncAndKsMacFromSharedSecret.Case1_KsEnc, Step2_KsEncAndKsMacFromSharedSecret.Case1_KsMac)]
                                   
    //[InlineData(10,  "00b08e000a", Case2_Length10_Command, Step2_KsEncAndKsMacFromSharedSecret.Case2_KsEnc, Step2_KsEncAndKsMacFromSharedSecret.Case2_KsMac)]
    //[InlineData(100, "00b08e0064", Case2_Length100_Command, Step2_KsEncAndKsMacFromSharedSecret.Case2_KsEnc, Step2_KsEncAndKsMacFromSharedSecret.Case2_KsMac)]
                                   
    //[InlineData(10,  "00b08e000a", Case3_Length10_Command, Step2_KsEncAndKsMacFromSharedSecret.Case3_KsEnc, Step2_KsEncAndKsMacFromSharedSecret.Case3_KsMac)]
    //[InlineData(100, "00b08e0064", Case3_Length100_Command, Step2_KsEncAndKsMacFromSharedSecret.Case3_KsEnc, Step2_KsEncAndKsMacFromSharedSecret.Case3_KsMac)]
    [Theory]
     public void Write(int requestedLength, string hexPlain, string expected, string ksEncString, string ksMacString)
    {
        Test(requestedLength, hexPlain, expected, ksEncString, ksMacString);
    }

    [InlineData("Case 5")]
    [InlineData("Case 6")]
    [InlineData("Case 7")]
    [InlineData("Case 8")]
    [Theory]
    public void UsingCases(string caseName)
    {
        var testCase = TestCases.Items[caseName];
        Test(testCase.Length, testCase.CommandApdu, testCase.WrappedCommandApdu, testCase.KsEnc, testCase.KsMac);
    }

    private static void Test(int requestedLength, string hexPlain, string expected, string ksEncString, string ksMacString)
    {
        Trace.WriteLine("KsEnc     : " + ksEncString);
        Trace.WriteLine("KsMac     : " + ksMacString);
        Trace.WriteLine("Length    : " + requestedLength);
        Trace.WriteLine("");

        const int shortFileId = 14;
        var sfi = 0x80 | (shortFileId & 0xFF);
        var plainApdu = new CommandApdu(ISO7816.CLA_ISO7816, ISO7816.INS_READ_BINARY, sfi, 0, requestedLength);
        var actualPlain = plainApdu.ToArray();
        Trace.WriteLine("act apdu  : " + Hex.ToHexString(actualPlain));
        Trace.WriteLine("exp apdu  : " + hexPlain);
        Assert.Equal(Hex.Decode(hexPlain), actualPlain);
        Trace.WriteLine("");

        var wrapper = new AesSecureMessagingWrapper(Hex.Decode(ksEncString), Hex.Decode(ksMacString));
        var wrapped = new CommandEncoder(wrapper).Encode(plainApdu);

        Trace.WriteLine("data      : " + wrapped.GetData().PrettyHexFormat(16));

        var actual = wrapped.ToArray();

        Trace.WriteLine("");
        Trace.WriteLine("Actual    : " + Hex.ToHexString(actual));
        Trace.WriteLine("Expected  : " + expected);

        Assert.Equal(Hex.Decode(expected), actual);
    }
}