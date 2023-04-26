using System;
using System.Text.RegularExpressions;

public static class RegexExtensions
{
    public static long LongGroup(this Match m, string id) => Convert.ToInt64(m.Groups[id].Value);
    public static long? OptionalLongGroup(this Match m, string id) => m.Groups.TryGetValue(id, out var value) && value != null && value.Value != "" ? Convert.ToInt64(value.Value) : null;
    public static string StringGroup(this Match m, string id) => m.Groups[id].Value;
    public static string? OptionalStringGroup(this Match m, string id) => m.Groups.TryGetValue(id, out var value) && value != null && value.Length > 0 ? value.Value : null;
}