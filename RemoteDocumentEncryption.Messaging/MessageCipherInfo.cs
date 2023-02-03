using System;
using System.Linq;

namespace NL.Rijksoverheid.RDW.RemoteDocumentEncryption.Messaging;

public class MessageCipherInfo {
    
    //Hex-encoded 16 byte IV - set by the message encoder. TODO Could choose to do this as a separate step depending on the chosen cipher
    public string Iv { get; set; }

    //TODO support multiple ciphers - with or without IVs
    //public string encryptionAlgorithm { get; set; }

    public RdeMessageDecryptionInfo RdeInfo { get; set; }
}