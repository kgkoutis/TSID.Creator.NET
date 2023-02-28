using System.Text;

namespace TSID.Creator.NET.Tests.Unit.Extensions;

public static class Arrays
{
    public static string ToString(this byte[] a) {
        if (a == null)
            return "null";
        var iMax = a.Length - 1;
        if (iMax == -1)
            return "[]";

        var b = new StringBuilder();
        b.Append('[');
        for (var i = 0; ; i++) {
            b.Append(a[i]);
            if (i == iMax)
                return b.Append(']').ToString();
            b.Append(", ");
        }
    }
}