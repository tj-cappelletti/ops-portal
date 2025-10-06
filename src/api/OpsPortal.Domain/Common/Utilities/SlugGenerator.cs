using System.Text.RegularExpressions;

namespace OpsPortal.Domain.Common.Utilities;

public static class SlugGenerator
{
    public static string Generate(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Convert to lowercase
        var slug = input.ToLowerInvariant();

        // Replace spaces and underscores with hyphens
        slug = Regex.Replace(slug, @"[\s_]+", "-");

        // Remove invalid characters
        slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");

        // Remove duplicate hyphens
        slug = Regex.Replace(slug, @"-+", "-");

        // Trim hyphens from ends
        slug = slug.Trim('-');

        return slug;
    }

    public static string GenerateUnique(string input, Func<string, bool> isUnique)
    {
        var baseSlug = Generate(input);
        var slug = baseSlug;
        var counter = 1;

        while (!isUnique(slug))
        {
            slug = $"{baseSlug}-{counter}";
            counter++;
        }

        return slug;
    }
}
