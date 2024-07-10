using System.Text.RegularExpressions;

namespace Frank.SourceGenerators.Localization.Internals;

internal static partial class NameHelper
{
    public static string MakeTypeFriendlyName(string source) => MyRegex().Replace(source.Replace("&", "And"), "");
    
    [GeneratedRegex(@"\A[0-9]|[^a-zA-Z0-9]")]
    private static partial Regex MyRegex();
}