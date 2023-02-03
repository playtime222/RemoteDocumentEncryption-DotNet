using System;
using System.Linq;

namespace NL.Rijksoverheid.RDW.RDE.CaSessionUtilities.Messaging;

/**
 * Pass this and a crypto spec to a message formatter
 * TODO use a builder?
 */
public class MessageContentArgs
{
    public string UnencryptedNote { get; set; }
    private readonly List<FileArgs> _Files = new();

    
    //TODO separate building from accessing
    public FileArgs[] FileArgs => _Files.ToArray();

    public void Add(FileArgs file)
    {
        _Files.Add(file);
    }
}

