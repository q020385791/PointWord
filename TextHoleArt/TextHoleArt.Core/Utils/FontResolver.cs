using System.Drawing.Text;

namespace TextHoleArt.Core.Utils;

internal static class FontResolver
{
    private const string GlobalFallback = "Segoe UI";

    private static readonly Lazy<HashSet<string>> InstalledFamilies = new(
        () => new InstalledFontCollection()
            .Families
            .Select(family => family.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase));

    internal static string ResolveFontFamilyName(string? requested, params string[] fallbacks)
    {
        if (!string.IsNullOrWhiteSpace(requested) && InstalledFamilies.Value.Contains(requested))
        {
            return requested;
        }

        foreach (string fallback in fallbacks)
        {
            if (!string.IsNullOrWhiteSpace(fallback) && InstalledFamilies.Value.Contains(fallback))
            {
                return fallback;
            }
        }

        return InstalledFamilies.Value.Contains(GlobalFallback)
            ? GlobalFallback
            : InstalledFamilies.Value.FirstOrDefault() ?? GlobalFallback;
    }
}
