using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities.Encoders;

namespace NL.Rijksoverheid.RDW.RDE.CaSessionUtilities.Messaging.ZipV2;

public class ZipMessageDecoder
{
    private string _Version;
    private MessageCipherInfo _MessageCipherInfo;
    private byte[] _Message;
    private string _RdeSessionArgsJson;

    private readonly List<MessageFile> _Files = new();
    private byte[] _SecretKey;
    private byte[] _Iv;

    
    /// <summary>
    /// Step 1 - Call this, do the phone and MRTD shuffle to get the seed, then derive the secret key
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public MessageCipherInfo DecodeRdeSessionArgs(byte[] message)
    {
        if (_Message != null) throw new ArgumentException();
        _Message = message;

        _Version = ReadPlainTextString(ZipMessageEncoder.VersionEntryName);
        if (!_Version.Equals(ZipMessageEncoder.Version, StringComparison.InvariantCultureIgnoreCase))
            throw new ArgumentException("Version not supported.");

        _RdeSessionArgsJson = ReadPlainTextString(ZipMessageEncoder.RdeSessionArgsEntryName);
        _MessageCipherInfo = JsonConvert.DeserializeObject<MessageCipherInfo>(_RdeSessionArgsJson);

        _Iv = Hex.Decode(_MessageCipherInfo.Iv);
        return _MessageCipherInfo;
    }

    /// <summary>
    /// Step 2 
    /// </summary>
    public MessageV2 Decode(byte[] secretKey)
    {
        if (_Message == null) throw new InvalidOperationException();

        _SecretKey = secretKey;

        VerifyPlainTextString(_Version, ZipMessageEncoder.VersionGmacEntryName);
        VerifyPlainTextString(_RdeSessionArgsJson, ZipMessageEncoder.RdeSessionArgsGmacEntryName);
        var note = ReadPlainTextString(ZipMessageEncoder.NoteEntryName);
        VerifyPlainTextString(note, ZipMessageEncoder.NoteGmacEntryName);

        var metadataJsonBytes = ReadAndVerify(ZipMessageEncoder.MetadataEntryName, ZipMessageEncoder.MetadataGmacEntryName);
        var json = Encoding.UTF8.GetString(metadataJsonBytes);
        var metadata = JsonConvert.DeserializeObject<Metadata>(json);

        for (var i = 0; i < metadata.Filenames.Length; i++)
        {
            var entryName = GetNextEntryName();
            _Files.Add(new MessageFile(metadata.Filenames[i], ReadAndVerify(entryName, GetMacEntryName())));
        }

        return new MessageV2(note, _MessageCipherInfo, _Files.ToArray());
    }

    private byte[] ReadAndVerify(string entryName, string gmacEntryName)
    {
        var cipherText = ReadEntry(entryName);
        var result = Crypto.GetAesCbcPkcs5PaddingPlainText(_SecretKey, _Iv, cipherText);
        Verify(result, gmacEntryName);
        return result;
    }

    private int _FileCounter = 4; //-> First one is R_5_1
    private string GetNextEntryName() => ZipMessageEncoder.GetEntryName(++_FileCounter);
    private string GetMacEntryName() => ZipMessageEncoder.GetMacEntryName(_FileCounter);

    private string ReadPlainTextString(string entryName)
    {
        return Encoding.UTF8.GetString(ReadEntry(entryName));
    }

    private void VerifyPlainTextString(string value, string entryName)
    {
        Verify(Encoding.UTF8.GetBytes(value), entryName);
    }

    private void Verify(byte[] value, string gmacEntryName)
    {
        var gmac = ReadEntry(gmacEntryName);
        Crypto.VerifyAesGMac(_SecretKey, _Iv, value, gmac); //Boom here if bad
    }

    private byte[] ReadEntry(string name)
    {
        using var input = new MemoryStream(_Message);
        using var zipStream = new ZipArchive(input, ZipArchiveMode.Read, true, Encoding.UTF8);
        var zipEntry = zipStream.Entries.Single(x => x.Name == name);
        using var stream = zipEntry.Open();
        var buffer = new byte[2048];
        var result = new MemoryStream();
        int len = stream.Read(buffer);
        while (len > 0)
        {
            result.Write(buffer, 0, len);
            len = stream.Read(buffer);
        }
        return result.ToArray();
    }
}
