using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;

namespace NL.Rijksoverheid.RDW.RDE.CaSessionUtilities.Messaging.ZipV2;


public sealed class ZipMessageEncoder : IMessageEncoder
{
    public static string Version = "1.0.0";
    public static string VersionEntryName = "R_1_1";
    public static string VersionGmacEntryName = "AT_1";

    //JSON Not encrypted
    public static string RdeSessionArgsEntryName = "R_2_1";
    public static string RdeSessionArgsGmacEntryName = "AT_2";

    public static string NoteEntryName = "R_3_1";
    public static string NoteGmacEntryName = "AT_3";

    public static string MetadataEntryName = "R_4_1";
    public static string MetadataGmacEntryName = "AT_4";

    //private Gson gson = new Gson();
    private byte[] _SecretKey;
    private byte[] _Iv;

    private MemoryStream _Result;
    private ZipArchive _ZipStream;

    public ZipMessageEncoder()
    {
    }

    /// <summary>
    /// TODO decouple from cipher
    /// </summary>
    /// <returns></returns>
    private byte[] GenerateIv() {
        var result = new byte[16];
        new SecureRandom().NextBytes(result);
        _Iv = result;
        return result;
    }

    private int _FileCounter = 4; //-> First one is R_5_1

    public static string GetEntryName(int fileCounter) => string.Format(CultureInfo.InvariantCulture, "R_{0}_1", fileCounter);
    public static string GetMacEntryName(int fileCounter) => string.Format(CultureInfo.InvariantCulture, "AT_{0}", fileCounter);

    private string GetNextEntryName() => GetEntryName(++_FileCounter);
    private string GetMacEntryName() => GetMacEntryName(_FileCounter);

    //TODO version marker entry
    public byte[] Encode(MessageContentArgs messageArgs, RdeMessageDecryptionInfo rdeSessionArgs, byte[] secretKey)
    {
        MessageCipherInfo args = new();

        this._SecretKey = secretKey;
        args.Iv = Hex.ToHexString(GenerateIv());
        args.RdeInfo = rdeSessionArgs;

        using (_Result = new MemoryStream())
        using (_ZipStream = new ZipArchive(_Result, ZipArchiveMode.Create, true, Encoding.UTF8))
        {

            WritePlain(VersionEntryName, VersionGmacEntryName, Encoding.UTF8.GetBytes(Version));
            WritePlain(NoteEntryName, NoteGmacEntryName, Encoding.UTF8.GetBytes(messageArgs.UnencryptedNote));

            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            
            var json = JsonConvert.SerializeObject(args, serializerSettings);
            WritePlain(RdeSessionArgsEntryName, RdeSessionArgsGmacEntryName, Encoding.UTF8.GetBytes(json));

            var filenames = new List<string>();
            for (var i = 0; i < messageArgs.FileArgs.Length; i++)
            {
                var item = messageArgs.FileArgs[i];
                filenames.Add(item.Name);
                WriteEncrypted(item.Content);
            }

            var metadata = new Metadata();
            var v = filenames.ToArray();
            metadata.Filenames = v;
            WriteEncrypted(MetadataEntryName, MetadataGmacEntryName, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(metadata, serializerSettings)));
        }
        _Result.Flush();
        return _Result.ToArray();
    }

    private void WritePlain(string entryName, string gmacEntryName, byte[] content)
    {
        Write(entryName, content);
        WriteGmac(gmacEntryName, content);
    }

    private void WriteEncrypted(byte[] content) 
    {
        var e = GetNextEntryName();
        WriteEncrypted(e, GetMacEntryName(), content);
    }

    private void WriteEncrypted(string entryName, string gmacEntryName, byte[] content) 
    {
        Write(entryName, Crypto.GetAesCbcPkcs5PaddingCipherText(this._SecretKey, this._Iv, content));
        WriteGmac(gmacEntryName, content);
    }

    private void WriteGmac(string entryName, byte[] content)
    {
        Write(entryName, Crypto.GetAesGMac(_SecretKey,_Iv,content));
    }

    private void Write(string entryName, byte[] content) 
    {
        var entry = _ZipStream.CreateEntry(entryName);
        using var stream = entry.Open();
        stream.Write(content);
        stream.Flush();
    }
}