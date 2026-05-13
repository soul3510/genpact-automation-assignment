using System.Text.RegularExpressions;

namespace Genpact.Automation.Tests.Utils;

/**
The normalization:
1. It converts text to lowercase, 
2. It removes punctuation/special characters,
3. It normalizes whitespace, 
4. It splits into words, 
5. It removes duplicates with Distinct(), 
6. It counts the unique words.
This approach ensures that the unique word count is based on the actual words used, regardless of their case or any punctuation, and it provides a consistent way to compare the UI and API texts.
**/
public static class TextNormalizer
{
    public static string Normalize(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var lowerCase = text.ToLowerInvariant();

        var lettersAndNumbersOnly = Regex.Replace(
            lowerCase,
            @"[^\p{L}\p{N}\s]",
            " "
        );

/**
The MediaWiki Parse API returns HTML, not only visible plain text.
So I removed scripts, styles, edit-section markup, references, comments, and parser metadata before converting the HTML to plain text.

Please note: Regex is acceptable here for a small assignment, but for larger HTML parsing I would prefer a library like AngleSharp or HtmlAgilityPack.
**/
        var normalizedSpaces = Regex.Replace(
            lettersAndNumbersOnly,
            @"\s+",
            " "
        );

        return normalizedSpaces.Trim();
    }

    public static int CountUniqueWords(string text)
    {
        var normalized = Normalize(text);

        if (string.IsNullOrWhiteSpace(normalized))
        {
            return 0;
        }

        return normalized
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Distinct()
            .Count();
    }
}