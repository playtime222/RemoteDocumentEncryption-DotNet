using System;
using System.IO;
using System.Linq;

namespace NL.Rijksoverheid.RDW.RemoteDocumentEncryption.Wrapping.Implementation;

public static class TLVUtil
{
    public static byte[] WrapDO(int tag, byte[] data /*encoded Le*/)
    {
        using var ms = new MemoryStream();
        var tlvOutputStream = new TLVOutputStream(ms);
        tlvOutputStream.writeTag(tag);
        tlvOutputStream.writeValue(data);
        tlvOutputStream.flush();
        tlvOutputStream.close();
        return ms.ToArray();
    }

    public static byte[] GetLengthAsBytes(int length)
    {
        var ms = new MemoryStream();
        if (length < 0x80)
            /* short form */
            ms.WriteByte((byte)length);
        else
        {//Never called

            var byteCount = CalculateLog(length, 256);
            ms.WriteByte((byte)(0x80 | byteCount));
            for (var i = 0; i < byteCount; i++)
            {
                var pos = 8 * (byteCount - i - 1);
                ms.WriteByte((byte)((length & 0xFF << pos) >> pos));
            }
        }
        return ms.ToArray();
    }

    //Never called
    private static int CalculateLog(int n, int b)
    {
        var result = 0;
        while (n > 0)
        {
            n = n / b;
            result++;
        }
        return result;
    }

    public static byte[] GetTagAsBytes(int tag)
    {
        var ms = new MemoryStream();
        var byteCount = (int)(Math.Log(tag) / Math.Log(256)) + 1;
        for (var i = 0; i < byteCount; i++)
        {
            var pos = 8 * (byteCount - i - 1);
            ms.WriteByte((byte)((tag & 0xFF << pos) >> pos));
        }
        var tagBytes = ms.ToArray();
        switch (GetTagClass(tag))
        {
            //case ASN1Constants.APPLICATION_CLASS:
            //    tagBytes[0] |= 0x40;
            //    break;
            case ASN1Constants.CONTEXT_SPECIFIC_CLASS: //Only ever this one
                tagBytes[0] |= 0x80;
                break;
            //case ASN1Constants.PRIVATE_CLASS:
            //    tagBytes[0] |= 0xC0;
            //    break;
            default:
                throw new InvalidOperationException("Unsupported tag class.");
        }
        if (!IsPrimitive(tag))
            tagBytes[0] |= 0x20;
        return tagBytes;
    }

    private static int GetTagClass(int tag)
    {
        var i = 3;
        for (; i >= 0; i--)
        {
            var mask = 0xFF << 8 * i;
            if ((tag & mask) != 0x00)
                break;
        }
        var msByte = (tag & 0xFF << 8 * i) >> 8 * i & 0xFF;
        switch (msByte & 0xC0)
        {
            //case 0x00:
            //    return ASN1Constants.UNIVERSAL_CLASS;
            //case 0x40:
            //    return ASN1Constants.APPLICATION_CLASS;
            case 0x80:
                return ASN1Constants.CONTEXT_SPECIFIC_CLASS; //Only ever this one
            case 0xC0:
            default:
                //return ASN1Constants.PRIVATE_CLASS;
                throw new InvalidOperationException();
        }
    }

    private static bool IsPrimitive(int tag)
    {
        var i = 3;
        for (; i >= 0; i--)
        {
            var mask = 0xFF << 8 * i;
            if ((tag & mask) != 0x00)
                break;
        }
        var msByte = (tag & 0xFF << 8 * i) >> 8 * i & 0xFF;
        return (msByte & 0x20) == 0x00;
    }
}