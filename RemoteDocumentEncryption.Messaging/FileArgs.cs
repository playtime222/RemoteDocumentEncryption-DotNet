using System;
using System.Linq;

namespace NL.Rijksoverheid.RDW.RemoteDocumentEncryption.Messaging;

//TODO validation
public class FileArgs
{
    public string Name { get; set; }
    public byte[] Content { get; set; }

    public FileArgs()
    {
    }

    public FileArgs(string name, byte[] content)
    {
        Name = name;
        Content = content;
    }

    //TODO MimeType?
}


