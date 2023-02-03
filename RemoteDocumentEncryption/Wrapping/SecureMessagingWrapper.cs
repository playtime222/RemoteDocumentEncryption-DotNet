using System;
using System.Linq;

namespace NL.Rijksoverheid.RDW.RDE.CaSessionUtilities.Wrapping;



/// <summary>
/// JUST the encryption methods required during the wrapping of the command and the paddedResponse
/// </summary>
public abstract class SecureMessagingWrapper
{
    public string CipherAlg { get; }
    public string MacAlg { get; }

    //private readonly string cipherAlg;
    public byte[] KsEnc { get; }
    public byte[] KsMac { get; }
    
    public int MaxTranceiveLength = 256; //TODO Or could be 65536 but does that need RB2 and so is out of scope?

    protected SecureMessagingWrapper(byte[] ksEnc, byte[] ksMac, string cipherAlg, string macAlg)
    {
        CipherAlg = cipherAlg;
        MacAlg = macAlg;

        KsEnc = ksEnc;
        KsMac = ksMac;
    }

    //Returns block of BlockSize ending in the bytes of the SSC
    public abstract byte[] GetEncodedSendSequenceCounter(long ssc);

     ///the length to use for padding for the cipher
    public abstract int BlockSize { get; }

    public abstract byte[] CalculateMac(byte[] data);

    public abstract byte[] GetEncodedDataForResponse(byte[] paddedResponse);
}