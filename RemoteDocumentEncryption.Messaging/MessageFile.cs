using System;
using System.Linq;

namespace NL.Rijksoverheid.RDW.RemoteDocumentEncryption.Messaging;

public class MessageFile {
    public string Filename { get; }
    public byte[] Content { get; }

    public MessageFile(string filename, byte[] content) {
        Filename = filename;
        Content = content;
    }
}
