using Genpact.Automation.Tests.Base;
using Genpact.Automation.Tests.Pages;

namespace Genpact.Automation.Tests.Tests;


/**
Scenario: Validate that all technology names in the Microsoft development tools box on the Playwright Wikipedia page are text links.

1. Navigate to the Playwright Wikipedia page.
2. Scroll/find the Microsoft development tools box.
3. Click Show if the box is collapsed.
4. Collect all actual technology-name list items inside the box.
5. Ignore group headers and UI controls.
6. Assert at least one technology was found.
7. Validate each technology name has a matching <a href>.
8. Fail with a clear list of non-linked names.


PLEASE NOTE: 
The test currently fails because the live Wikipedia page has two technology names, Visual Basic and Playwright, rendered as plain text instead of links. 
**/

[TestFixture]
public class MicrosoftDevelopmentToolsTests : UiTestBase
{
    [Test]
    public async Task MicrosoftDevelopmentTools_AllTechnologyNames_ShouldBeTextLinks()
    {
        var wikiPage = new PlaywrightWikiPage(Page);

        await wikiPage.NavigateAsync();

        var validationResult =
            await wikiPage.GetMicrosoftDevelopmentToolsLinkValidationResultAsync();

        TestContext.WriteLine(
            $"Validated technology names count: {validationResult.TechnologyNames.Count}"
        );

        TestContext.WriteLine(
            $"Validated technology names: {string.Join(", ", validationResult.TechnologyNames)}"
        );

        TestContext.WriteLine(
            $"Non-linked technology names: {string.Join(", ", validationResult.NonLinkedTechnologyNames)}"
        );

        Assert.That(
            validationResult.TechnologyNames,
            Is.Not.Empty,
            "No technology names were found under Microsoft development tools. " +
            "This prevents a false positive where the test passes without validating anything."
        );

        Assert.That(
            validationResult.TechnologyNames,
            Has.Some.EqualTo("Playwright"),
            "Expected to find Playwright under Microsoft development tools. " +
            "This confirms the test validated the correct section."
        );

        Assert.That(
            validationResult.NonLinkedTechnologyNames,
            Is.Empty,
            $"The following technology names are not links: " +
            $"{string.Join(", ", validationResult.NonLinkedTechnologyNames)}"
        );
    }
}