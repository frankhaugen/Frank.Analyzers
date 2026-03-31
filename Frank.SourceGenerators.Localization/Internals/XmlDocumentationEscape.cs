using System.Text;

namespace Frank.SourceGenerators.Localization.Internals;

/// <summary>
/// C# XML documentation comments cannot contain raw <c>&lt;</c>, <c>&gt;</c>, <c>&amp;</c>, or the sequence <c>--</c>.
/// </summary>
internal static class XmlDocumentationEscape
{
    public static string ForSummary(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        var sb = new StringBuilder(text.Length + 8);
        foreach (var ch in text)
        {
            switch (ch)
            {
                case '&':
                    sb.Append("&amp;");
                    break;
                case '<':
                    sb.Append("&lt;");
                    break;
                case '>':
                    sb.Append("&gt;");
                    break;
                case '"':
                    sb.Append("&quot;");
                    break;
                case '\'':
                    sb.Append("&apos;");
                    break;
                default:
                    sb.Append(ch);
                    break;
            }
        }

        // XML 1.0 / C# doc comment: "--" is not allowed inside comments.
        return sb.ToString().Replace("--", "&#45;&#45;", StringComparison.Ordinal);
    }
}
