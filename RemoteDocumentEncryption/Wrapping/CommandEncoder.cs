using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NL.Rijksoverheid.RDW.RemoteDocumentEncryption.Wrapping.Implementation;

namespace NL.Rijksoverheid.RDW.RemoteDocumentEncryption.Wrapping;

public class CommandEncoder
{
    private const long SecureSessionCounter = 1;

    private readonly SecureMessagingWrapper _Wrapper;

    public CommandEncoder(SecureMessagingWrapper wrapper)
    {
        _Wrapper = wrapper;
    }

    public CommandApdu Encode(CommandApdu commandApdu)
    {
        return WrapCommandAPDU(commandApdu);
    }

    public static CommandApdu CreateRbCommandApdu(int shortFileId, int fidByteCount)
    {
        int sfi = 0x80 | (shortFileId & 0xFF);
        return new CommandApdu(ISO7816.CLA_ISO7816, ISO7816.INS_READ_BINARY, (byte)sfi, 0, fidByteCount);
    }

    public CommandApdu CreateWrappedRbCommandApdu(int shortFileId, int fidByteCount)
        => Encode(CreateRbCommandApdu(shortFileId, fidByteCount));

    /**
     * Performs the actual encoding of a command APDU.
     * Based on Section E.3 of ICAO-TR-PKI, especially the examples.
     *
     * @param commandApdu the command APDU
     *
     * @return a byte array containing the wrapped APDU buffer
     */
    private CommandApdu WrapCommandAPDU(CommandApdu commandAPDU)
    {
        int cla = commandAPDU.CLA;
        int ins = commandAPDU.INS;
        int p1 = commandAPDU.P1;
        int p2 = commandAPDU.P2;
        //int lc = commandApdu.Nc;
        int le = commandAPDU.Ne;

        byte[] maskedHeader = { (byte)(cla | (byte)0x0C), (byte)ins, (byte)p1, (byte)p2 };
        byte[] paddedMaskedHeader = PaddingIso9797.GetPaddedArrayMethod2(maskedHeader, _Wrapper.BlockSize);

        //bool hasDO85 = ((byte)commandApdu.INS == ISO7816.INS_READ_BINARY2);

        byte[] do8587 = new byte[0];
        byte[] do97 = new byte[0];

        /* Include the expected length, if present. */
        if (le > 0)
        {
            do97 = TLVUtil.WrapDO(0x97, EncodeLe(le));
        }

        var n = GetN(paddedMaskedHeader, do8587, do97);
        var do8E = GetDo8eBlock(n);

        /* Construct protected APDU... */
        using var memoryStream = new MemoryStream();
        memoryStream.Write(do8587);
        memoryStream.Write(do97);
        memoryStream.Write(do8E);
        var data = memoryStream.ToArray();

        /*
         * The requested response is 0x00 or 0x0000, depending on whether extended length is needed.
         */
        if (le <= 256 && data.Length <= 255)
        {
            return new CommandApdu(maskedHeader[0], maskedHeader[1], maskedHeader[2], maskedHeader[3], data, 256);
        }
        
        if (le > 256 || data.Length > 255)
        {
            return new CommandApdu(maskedHeader[0], maskedHeader[1], maskedHeader[2], maskedHeader[3], data, 65536);
        }
        
        //Not sure if this case ever occurs, but this is consistent with previous behavior. */
        return new CommandApdu(maskedHeader[0], maskedHeader[1], maskedHeader[2], maskedHeader[3], data, _Wrapper.MaxTranceiveLength);
    }

    private byte[] GetDo8eBlock(byte[] n)
    {
        var cc = _Wrapper.CalculateMac(n);

        Trace.WriteLine($"{"mac",-10}: {cc.PrettyHexFormat()}");
        const int ccLength = 8;
        using MemoryStream memoryStream = new();
        memoryStream.WriteByte((byte)0x8E);
        memoryStream.WriteByte((byte)ccLength); //
        memoryStream.Write(cc, 0, ccLength);
        var do8E = memoryStream.ToArray();
        Trace.WriteLine($"{"d08e",-10}: {do8E.PrettyHexFormat()}");
        return do8E;
    }

    private byte[] GetN(byte[] paddedMaskedHeader, byte[] do8587, byte[] do97)
    {
        using var ms = new MemoryStream();

        var encSeq = _Wrapper.GetEncodedSendSequenceCounter(SecureSessionCounter);
        Trace.WriteLine($"{"ssc",-10}: {encSeq.PrettyHexFormat()}");

        ms.Write(encSeq);
        ms.Write(paddedMaskedHeader);
        ms.Write(do8587);
        ms.Write(do97);
        var n = PaddingIso9797.GetPaddedArrayMethod2(ms.ToArray(), _Wrapper.BlockSize);
        Trace.WriteLine($"{"n",-10}: {n.PrettyHexFormat()}");
        return n;
    }

    private static byte[] EncodeLe(int le) => le is >= 0 and <= 256 ? new [] { (byte)le } : new [] { (byte)((le & 0xFF00) >> 8), (byte)(le & 0xFF) };
}