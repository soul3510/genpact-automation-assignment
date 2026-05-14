using Genpact.Automation.Tests.Base;
using Genpact.Automation.Tests.Pages;

using Allure.NUnit;
using Allure.NUnit.Attributes;

namespace Genpact.Automation.Tests.Tests;

[TestFixture]
[AllureNUnit]
[AllureFeature("Task 3 - Appearance color")]
public class AppearanceColorTests : UiTestBase
{
    [Test]
    [AllureStory("Change Color option to Dark and validate theme")]
    public async Task ColorBeta_WhenChangedToDark_ShouldApplyDarkTheme()
    {

        var wikiPage = new PlaywrightWikiPage(Page);

        await wikiPage.NavigateAsync();

        await wikiPage.SetColorBetaToLightAsync();
        var lightState = await wikiPage.GetAppearanceColorStateAsync();

        TestContext.WriteLine(
            $"Before change - selected color: {lightState.SelectedColor}, background: {lightState.BackgroundColor}, dark applied: {lightState.IsDarkModeApplied}"
        );

        await wikiPage.SetColorBetaToDarkAsync();
        var darkState = await wikiPage.GetAppearanceColorStateAsync();

        TestContext.WriteLine(
            $"After change - selected color: {darkState.SelectedColor}, background: {darkState.BackgroundColor}, dark applied: {darkState.IsDarkModeApplied}"
        );

        Assert.That(
            darkState.SelectedColor,
            Is.EqualTo("Dark"),
            "Expected the Color (beta) selected option to be Dark."
        );

        Assert.That(
            darkState.IsDarkModeApplied,
            Is.True,
            "Expected the page background luminance to indicate that dark mode was actually applied."
        );

        Assert.That(
            darkState.BackgroundColor,
            Is.Not.EqualTo(lightState.BackgroundColor),
            "Expected the page background color to change after selecting Dark mode."
        );
    }
}