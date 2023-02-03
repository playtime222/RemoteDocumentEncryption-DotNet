using System;
using System.Linq;

namespace NL.Rijksoverheid.RDW.RDE.CaSessionUtilities.Messaging;

public interface IMessageEncoder {

    byte[] Encode(MessageContentArgs messageArgs, RdeMessageDecryptionInfo messageCryptoArgs, byte[] secretKey);
}
