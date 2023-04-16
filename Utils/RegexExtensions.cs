using System;
using System.Text.RegularExpressions;

public static class RegexExtensions
{
    public static long LongGroup(this Match m, string id) => Convert.ToInt64(m.Groups[id].Value);
}