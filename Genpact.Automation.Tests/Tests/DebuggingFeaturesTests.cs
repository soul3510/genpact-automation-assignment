using Genpact.Automation.Tests.Base;
using Genpact.Automation.Tests.Pages;
using Genpact.Automation.Tests.Services;
using Genpact.Automation.Tests.Utils;

using Allure.NUnit;
using Allure.NUnit.Attributes;

namespace Genpact.Automation.Tests.Tests;


[TestFixture]
[AllureNUnit]
[AllureFeature("Task 1 - Debugging features comparison")]
public class DebuggingFeaturesTests : UiTestBase
{
    [Test]
    [AllureStory("Compare UI and API unique word counts")]
    public async Task DebuggingFeatures_UiAndApiUniqueWordCounts_ShouldBeEqual()
    {
        var wikiPage = new PlaywrightWikiPage(Page);
        var mediaWikiApiClient = new MediaWikiApiClient();

        await wikiPage.NavigateAsync();

        var uiText = await wikiPage.GetDebuggingFeaturesSectionTextAsync();
        var apiText = await mediaWikiApiClient.GetDebuggingFeaturesSectionTextAsync();

        var uiUniqueWordCount = TextNormalizer.CountUniqueWords(uiText);
        var apiUniqueWordCount = TextNormalizer.CountUniqueWords(apiText);

        TestContext.WriteLine($"UI unique word count: {uiUniqueWordCount}");
        TestContext.WriteLine($"API unique word count: {apiUniqueWordCount}");

        Assert.That(
            uiUniqueWordCount,
            Is.EqualTo(apiUniqueWordCount),
            $"Expected UI and API unique word counts to be equal. UI text: {uiText}. API text: {apiText}"
        );
    }
}