using Genpact.Automation.Tests.Base;
using Genpact.Automation.Tests.Pages;

using Allure.NUnit;
using Allure.NUnit.Attributes;

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
[AllureNUnit]
[AllureFeature("Task 2 - Microsoft development tools links")]
public class MicrosoftDevelopmentToolsTests : UiTestBase
{
    [Test]
    [AllureStory("Validate all technology names are text links")]
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

/**
Prevent a false positive: by asserting that at least one technology name was found, we ensure that the test is actually validating the correct section of the page.
If the locator is wrong and finds zero technology names, the list of non-linked items would also be empty, and the test would pass without validating anything.
So I assert that technology names were actually found before checking if non-linked names are empty.
**/
        Assert.That(
            validationResult.TechnologyNames,
            Is.Not.Empty,
            "No technology names were found under Microsoft development tools. " +
            "This prevents a false positive where the test passes without validating anything."
        );

/**
Playwright is a known item inside the Microsoft development tools box.
I used it as a sanity check to prove the test reached the correct section and did not validate some unrelated part of the page.
**/
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