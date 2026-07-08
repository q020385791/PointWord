using System.Globalization;

namespace PointWord.Core.Utils;

/// <summary>
/// Splits target text into visible text elements (graphemes), excluding line breaks and whitespace-only elements.
/// </summary>
public static class TargetTextSplitter
{
    /// <summary>
    /// Splits a target text string into renderable per-character text elements.
    /// </summary>
    /// <param name="targetText">Raw target text input.</param>
    /// <returns>An ordered list of text elements suitable for per-character rendering.</returns>
    public static IReadOnlyList<string> SplitCharacters(string? targetText)
    {
        if (string.IsNullOrWhiteSpace(targetText))
        {
            return Array.Empty<string>();
        }

        string normalized = targetText
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n');

        List<string> result = new();
        TextElementEnumerator enumerator = StringInfo.GetTextElementEnumerator(normalized);

        while (enumerator.MoveNext())
        {
            string element = enumerator.GetTextElement();
            if (element == "\n" || string.IsNullOrWhiteSpace(element))
            {
                continue;
            }

            result.Add(element);
        }

        return result;
    }
}
