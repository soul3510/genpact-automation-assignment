using Genpact.Automation.Tests.Base;
using Genpact.Automation.Tests.Pages;

namespace Genpact.Automation.Tests.Tests;

[TestFixture]
public class MicrosoftDevelopmentToolsTests : UiTestBase
{
    [Test]
    public async Task MicrosoftDevelopmentTools_AllTechnologyNames_ShouldBeTextLinks()
    {
        var wikiPage = new PlaywrightWikiPage(Page);

        await wikiPage.NavigateAsync();

        var nonLinkedTechnologyNames =
            await wikiPage.GetNonLinkedTechnologyNamesInMicrosoftDevelopmentToolsAsync();

        TestContext.WriteLine(
            $"Non-linked technology names: {string.Join(", ", nonLinkedTechnologyNames)}"
        );

        Assert.That(
            nonLinkedTechnologyNames,
            Is.Empty,
            $"The following technology names are not links: {string.Join(", ", nonLinkedTechnologyNames)}"
        );
    }
}