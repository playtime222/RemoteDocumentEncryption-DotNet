using System;
using System.Diagnostics;
using System.Linq;
using NL.Rijksoverheid.RDW.RemoteDocumentEncryption.Wrapping;
using Org.BouncyCastle.Utilities.Encoders;

namespace CaSessionUtilitiesTest;

public class Step3_Encode_APDU_ReadBinary_Command
{
    [InlineData("J 1")]
    [InlineData("Case 5")]
    [InlineData("Case 6")]
    [InlineData("Case 7")]
    [InlineData("Case 8")]
    [Theory]
    public void DoIt(string caseName)
    {
        var testCase = TestCases.Items[caseName];
        Test(caseName, testCase.KsEnc, testCase.KsMac, testCase.Length, testCase.WrappedCommandApdu);
    }
    
    //[InlineData("New 5", Step2_KsEncAndKsMacFromSharedSecret.Case5_KsEnc, Step2_KsEncAndKsMacFromSharedSecret.Case5_KsMac, 64, "0CB08E000D9701408E083D9E4F0E1079F77600")]
    //[InlineData("New 6", "491d608bd275efded22349fd23a8caf5afb40e73c9f777c8ed138afa940f374d", "51f74babf372099fb6c68f83fa73df5602e120d3d62147581f9391ef6c24154c", 63, "0CB08E000D97013F8E08943DDEC4472584F800")]
    //[Theory]
    private void Test(string name, String ksEnc, String ksMac, int len, string expected)
    {
        //Note that as the RB command does not have a data parameter, no use is made of KsEnc only ksMac
        var result = CreateEncryptedRbCommand(ksEnc, ksMac, len);
        Trace.WriteLine("KsMac               : " + ksMac);
        var actualHex = Hex.ToHexString(result);
        Trace.WriteLine("Wrapped Command APDU");
        Trace.WriteLine("Actual   : " + actualHex);
        Trace.WriteLine("Expected : " + expected.ToLower());
        Assert.Equal(Hex.Decode(actualHex), result);
    }

    private byte[] CreateEncryptedRbCommand(string ksEncHex, String ksMacHex, int fidByteCount)
    {
        var command = CommandEncoder.CreateRbCommandApdu(14, fidByteCount); //Using DG14 for all tests
        Trace.WriteLine("Command APDU        : " + Hex.ToHexString(command.ToArray()));
        var ksEnc = Hex.Decode(ksEncHex);
        var ksMac = Hex.Decode(ksMacHex);
        return new CommandEncoder(new AesSecureMessagingWrapper(ksEnc, ksMac)).CreateWrappedRbCommandApdu(14,fidByteCount).ToArray();
    }
}