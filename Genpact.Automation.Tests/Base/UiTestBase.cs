using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Genpact.Automation.Tests.Base;

public class UiTestBase : PageTest
{
    protected const string WikipediaPlaywrightUrl = "https://en.wikipedia.org/wiki/Playwright_(software)";

    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {

            /**
            I set a desktop viewport because the Color beta control is part of Wikipedia’s right-side appearance panel. 
            On smaller screens the layout can change or collapse, so setting a predictable viewport makes the UI test more stable and deterministic.
            **/
            ViewportSize = new ViewportSize
            {
                Width = 1600,
                Height = 1000
            }
        };
    }

    [SetUp]
    public async Task BaseSetUp()
    {
        /**
        I added tracing to make failures easier to investigate.
        Playwright trace files can show screenshots, DOM snapshots, actions, timing, and source information, so instead of guessing why a UI test failed, I can open the trace and debug the exact browser state.
        In automation infrastructure, the test result is not enough. When a test fails, the framework should help the engineer understand why it failed quickly. Tracing supports that.
        Tracing can be founf here: Genpact.Automation.Tests\bin\Debug\net8.0 - run tracing with: "powershell -ExecutionPolicy Bypass -File bin/Debug/net8.0/playwright.ps1 show-trace bin/Debug/net8.0/trace-DebuggingFeatures_UiAndApiUniqueWordCounts_ShouldBeEqual.zip"
        **/
        await Context.Tracing.StartAsync(new()
        {
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });
    }

    [TearDown]
    public async Task BaseTearDown()
    {
    
        var testName = TestContext.CurrentContext.Test.Name.Replace(" ", "_");
        var tracePath = Path.Combine(
            TestContext.CurrentContext.WorkDirectory,
            $"trace-{testName}.zip"
        );

       /**
        Stopped tracing and saved the trace file with a name that includes the test name. This way, if a test fails, I can easily find the corresponding trace file in the test output directory and open it with Playwright's Trace Viewer for debugging.
        **/
        await Context.Tracing.StopAsync(new()
        {
            Path = tracePath
        });
    }
}