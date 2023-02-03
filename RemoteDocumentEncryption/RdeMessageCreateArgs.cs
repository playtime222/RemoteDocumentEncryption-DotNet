using System;
using System.Linq;

namespace NL.Rijksoverheid.RDW.RDE.CaSessionUtilities;

/// <summary>
/// Should contain RDE 'Certificate'
/// TODO NB read all the DG14 oids before this point
/// </summary>
public record RdeMessageCreateArgs
{
    public CaSessionArgs CaSessionArgs { get; set; }
    public int FileShortId { get; set; }
    public byte[] FileContent { get; set; }

    /// <summary>
    /// Max is 256 cos max transceive length
    /// </summary>
    public int ReadLength { get; set; }
}