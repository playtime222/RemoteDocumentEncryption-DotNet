using System;
using System.Collections.Generic;
using System.Linq;
using Org.BouncyCastle.Utilities.Encoders;

namespace NL.Rijksoverheid.RDW.RDE.CaSessionUtilities;

public static class FormattingUtilities
{
    public static string[] SplitEvery(this string thiz, int partLength)
    {
        if (thiz == null)
            throw new ArgumentNullException(nameof(thiz));

        if (partLength < 1)
            throw new ArgumentException("Part length has to be positive.", nameof(partLength));

        var result = new List<string>();

        for (var i = 0; i < thiz.Length; i += partLength)
            result.Add(thiz.Substring(i, Math.Min(partLength, thiz.Length - i)));

        return result.ToArray();
    }

    public static string PrettyHexFormat(this byte[] thiz, int partlength = 16)
        => string.Join("-", Hex.ToHexString(thiz).SplitEvery(partlength));


}