using System.Collections.Generic;
using System;

public static class Splitter
{
    public static List<string> SplitWithSpan(this string str, char delimiter)
    {
        var result = new List<string>();
        ReadOnlySpan<char> span = str.AsSpan();

        int end;
        while ((end = span.IndexOf(delimiter)) != -1)
        {
            result.Add(span.Slice(0, end).ToString());
            span = span.Slice(end + 1);
        }

        result.Add(span.ToString());
        return result;
    }
}