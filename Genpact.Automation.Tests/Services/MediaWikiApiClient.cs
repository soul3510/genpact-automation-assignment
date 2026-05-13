using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Genpact.Automation.Tests.Services;

public class MediaWikiApiClient
{
    private const string ApiBaseUrl = "https://en.wikipedia.org/w/api.php";
    private const string PageTitle = "Playwright_(software)";
    private const string DebuggingFeaturesSectionTitle = "Debugging features";

    private readonly HttpClient _httpClient;

    public MediaWikiApiClient()
    {
        _httpClient = new HttpClient();

        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "GenpactAutomationAssignment/1.0 (GitHub repository for Genpact automation assignment)"
        );

        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")
        );
    }

    public async Task<string> GetDebuggingFeaturesSectionTextAsync()
    {
        var sectionIndex = await GetDebuggingFeaturesSectionIndexAsync();

        var url =
            $"{ApiBaseUrl}" +
            $"?action=parse" +
            $"&page={Uri.EscapeDataString(PageTitle)}" +
            $"&prop=text" +
            $"&section={Uri.EscapeDataString(sectionIndex)}" +
            $"&disableeditsection=1" +
            $"&disabletoc=1" +
            $"&format=json" +
            $"&formatversion=2";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(json);

        var html = document.RootElement
            .GetProperty("parse")
            .GetProperty("text")
            .GetString();

        if (string.IsNullOrWhiteSpace(html))
        {
            throw new InvalidOperationException("MediaWiki API returned empty HTML for Debugging features section.");
        }

        return ConvertHtmlToPlainText(html);
    }

    private async Task<string> GetDebuggingFeaturesSectionIndexAsync()
    {
        var url =
            $"{ApiBaseUrl}" +
            $"?action=parse" +
            $"&page={Uri.EscapeDataString(PageTitle)}" +
            $"&prop=sections" +
            $"&format=json" +
            $"&formatversion=2";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(json);

        var sections = document.RootElement
            .GetProperty("parse")
            .GetProperty("sections")
            .EnumerateArray();

        foreach (var section in sections)
        {
            var line = section.GetProperty("line").GetString();

            var anchor = section.TryGetProperty("anchor", out var anchorProperty)
                ? anchorProperty.GetString()
                : string.Empty;

            var isDebuggingFeaturesSection =
                string.Equals(line, DebuggingFeaturesSectionTitle, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(anchor, "Debugging_features", StringComparison.OrdinalIgnoreCase);

            if (isDebuggingFeaturesSection)
            {
                return section.GetProperty("index").GetString()
                    ?? throw new InvalidOperationException("Debugging features section index is empty.");
            }
        }

        throw new InvalidOperationException("Could not find Debugging features section in MediaWiki sections API.");
    }

    private static string ConvertHtmlToPlainText(string html)
    {
        var cleaned = html;

        cleaned = Regex.Replace(
            cleaned,
            @"<!--.*?-->",
            " ",
            RegexOptions.Singleline | RegexOptions.IgnoreCase
        );

        cleaned = Regex.Replace(
            cleaned,
            @"<script[^>]*>.*?</script>",
            " ",
            RegexOptions.Singleline | RegexOptions.IgnoreCase
        );

        cleaned = Regex.Replace(
            cleaned,
            @"<style[^>]*>.*?</style>",
            " ",
            RegexOptions.Singleline | RegexOptions.IgnoreCase
        );

        cleaned = Regex.Replace(
            cleaned,
            @"<span[^>]*class=""[^""]*mw-editsection[^""]*""[^>]*>.*?</span>",
            " ",
            RegexOptions.Singleline | RegexOptions.IgnoreCase
        );

        cleaned = Regex.Replace(
            cleaned,
            @"<sup[^>]*class=""[^""]*reference[^""]*""[^>]*>.*?</sup>",
            " ",
            RegexOptions.Singleline | RegexOptions.IgnoreCase
        );

        cleaned = Regex.Replace(
            cleaned,
            @"<div[^>]*class=""[^""]*mw-references-wrap[^""]*""[\s\S]*",
            " ",
            RegexOptions.IgnoreCase
        );

        cleaned = Regex.Replace(
            cleaned,
            @"<ol[^>]*class=""[^""]*references[^""]*""[^>]*>.*?</ol>",
            " ",
            RegexOptions.Singleline | RegexOptions.IgnoreCase
        );

        cleaned = Regex.Replace(cleaned, "<.*?>", " ");

        return System.Net.WebUtility.HtmlDecode(cleaned);
    }
}