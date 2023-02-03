using System;
using NL.Rijksoverheid.RDW.RemoteDocumentEncryption;
using Org.BouncyCastle.Utilities.Encoders;

namespace CaSessionUtilitiesTest;

public class CryptoSanityCheckCrossPlatform
{
    [InlineData("00", "ADCBA368FD14A836908252EF76D09BAD2766C5FFB2FE7857F468676FC4B293E0", "69fb0629543d3ac966ca0b39d795f182")]
    [InlineData("00010203", "ADCBA368FD14A836908252EF76D09BAD2766C5FFB2FE7857F468676FC4B293E0", "06981c1a7f9bf61d7d12f1bf4e65de27")]
    [Theory]
    private void SanityCheckCrossPlatformAesCmac(string buffer, string ksMac, string expected)
    {
        var result = Crypto.GetAesCMac(Hex.Decode(ksMac), Hex.Decode(buffer));
        Assert.Equal(Hex.Decode(expected), result);
    }
}