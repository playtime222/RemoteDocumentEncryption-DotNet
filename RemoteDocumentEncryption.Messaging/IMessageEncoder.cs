using System;
using System.Linq;

namespace NL.Rijksoverheid.RDW.RemoteDocumentEncryption.Messaging;

public interface IMessageEncoder {

    byte[] Encode(MessageContentArgs messageArgs, RdeMessageDecryptionInfo messageCryptoArgs, byte[] secretKey);
}
