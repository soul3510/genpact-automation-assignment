using Microsoft.Playwright;

namespace Genpact.Automation.Tests.Pages;

public class PlaywrightWikiPage
{
    private readonly IPage _page;

    public PlaywrightWikiPage(IPage page)
    {
        _page = page;
    }

    public async Task NavigateAsync()
    {
        await _page.GotoAsync(
            "https://en.wikipedia.org/wiki/Playwright_(software)",
            new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 60000
            }
        );
    }


/**
1. I start from the exact heading anchor #Debugging_features, which is the main title: "Debugging features" 
2. I iterate through the next sibling elements until I detect the next section heading. This is wgere the test will stop collecting text, ensuring that only content from the "Debugging features" section is included.
**/
    public async Task<string> GetDebuggingFeaturesSectionTextAsync()
    {
        var heading = _page.Locator("#Debugging_features"); //

        await heading.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 60000
        });

        return await heading.EvaluateAsync<string>(
            @"heading => {
                const parts = [heading.textContent.trim()];

                const headingContainer =
                    heading.closest('.mw-heading') ||
                    heading.closest('h1,h2,h3,h4,h5,h6') ||
                    heading;

                let currentNode = headingContainer.nextElementSibling;

                while (currentNode) {
                    const tagName = currentNode.tagName ? currentNode.tagName.toUpperCase() : '';

                    const isNextHeading =
                        ['H2', 'H3', 'H4', 'H5', 'H6'].includes(tagName) ||
                        currentNode.matches('.mw-heading, .mw-heading2, .mw-heading3, .mw-heading4, .mw-heading5, .mw-heading6');

                    if (isNextHeading) {
                        break;
                    }

                    const clone = currentNode.cloneNode(true);

                    clone
                        .querySelectorAll('.mw-editsection, .reference, sup.reference, style, script')
                        .forEach(element => element.remove());

                    const text = clone.textContent || '';

                    if (text.trim()) {
                        parts.push(text.trim());
                    }

                    currentNode = currentNode.nextElementSibling;
                }

                return parts.join('\n');
            }"
        );
    }


   public async Task<IReadOnlyList<string>> GetNonLinkedTechnologyNamesInMicrosoftDevelopmentToolsAsync()
{
    await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

    await _page.WaitForFunctionAsync(
        @"() => document.body && document.body.textContent.includes('Microsoft development tools')",
        null,
        new PageWaitForFunctionOptions
        {
            Timeout = 60000
        }
    );

    var nonLinkedTechnologyNames = await _page.EvaluateAsync<string[]>(
        @"() => {
            const normalize = text => (text || '')
                .replace(/\s+/g, ' ')
                .trim();

            const candidates = Array.from(
                document.querySelectorAll('table, div, nav')
            )
            .filter(element => {
                const text = normalize(element.textContent);
                return text.includes('Microsoft development tools');
            })
            .sort((a, b) =>
                normalize(a.textContent).length - normalize(b.textContent).length
            );

            const container = candidates[0];

            if (!container) {
                throw new Error('Could not find Microsoft development tools container.');
            }

            const showToggle = Array.from(container.querySelectorAll('button, a, span'))
                .find(element => /^show$/i.test(normalize(element.textContent)));

            if (showToggle) {
                showToggle.click();
            }

            const listItems = Array.from(container.querySelectorAll('li'));
            const nonLinkedItems = [];

            for (const item of listItems) {
                const itemClone = item.cloneNode(true);

                itemClone
                    .querySelectorAll('ul, ol, style, script')
                    .forEach(element => element.remove());

                const technologyName = normalize(itemClone.textContent);

                if (!technologyName) {
                    continue;
                }

                const hasMatchingTextLink = Array.from(item.querySelectorAll('a[href]'))
                    .some(link => normalize(link.textContent) === technologyName);

                if (!hasMatchingTextLink) {
                    nonLinkedItems.push(technologyName);
                }
            }

            return [...new Set(nonLinkedItems)];
        }"
    );

    return nonLinkedTechnologyNames;
}
}