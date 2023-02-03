using System;
using System.Linq;

namespace NL.Rijksoverheid.RDW.RemoteDocumentEncryption.Messaging;

public class MessageV2 {
    public string Note { get; }
    public MessageCipherInfo SessionArgs { get; }
    public MessageFile[] Files { get; }

    public MessageV2(string note, MessageCipherInfo messageCipherInfo, MessageFile[] objects) {

        Note = note;
        SessionArgs = messageCipherInfo;
        Files = objects;
    }
}
