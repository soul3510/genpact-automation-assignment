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

public async Task<MicrosoftDevelopmentToolsLinkValidationResult> GetMicrosoftDevelopmentToolsLinkValidationResultAsync()
{
    await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

    await _page.WaitForFunctionAsync(
        @"() => {
            const text = document.body?.textContent || '';
            return text.includes('Microsoft development tools');
        }",
        null,
        new PageWaitForFunctionOptions
        {
            Timeout = 60000
        }
    );

    var result = await _page.EvaluateAsync<string[][]>(
        @"() => {
            const normalize = text => (text || '')
                .replace(/\s+/g, ' ')
                .trim();

            const getDirectTechnologyName = item => {
                const clone = item.cloneNode(true);

                clone
    .querySelectorAll('ul, ol, style, script, .navbar, .navbar-mini, .plainlinks')
    .forEach(element => element.remove());

                return normalize(clone.textContent);
            };

            const candidates = Array.from(
                document.querySelectorAll('table, div, nav')
            )
            .map(element => {
                const text = normalize(element.textContent);
                const listItemsCount = element.querySelectorAll('li').length;

                return {
                    element,
                    text,
                    listItemsCount
                };
            })
            .filter(candidate =>
                candidate.text.includes('Microsoft development tools') &&
                candidate.text.includes('WinDbg') &&
                candidate.text.includes('Visual Studio') &&
                candidate.listItemsCount > 5
            )
            .sort((a, b) => a.text.length - b.text.length);

            const container = candidates[0]?.element;

            if (!container) {
                throw new Error('Could not find Microsoft development tools container with technology items.');
            }

            const showToggle = Array.from(container.querySelectorAll('button, a, span'))
                .find(element => /^show$/i.test(normalize(element.textContent)));

            if (showToggle) {
                showToggle.click();
            }

const listItems = Array.from(container.querySelectorAll('li'))
    .filter(item =>
        !item.closest('.navbar') &&
        !item.closest('.navbar-mini') &&
        !item.closest('.plainlinks')
    );

            const technologyNames = [];
            const nonLinkedTechnologyNames = [];

            for (const item of listItems) {
                const technologyName = getDirectTechnologyName(item);

                if (!technologyName) {
                    continue;
                }

                const isUiControl =
                    /^show$/i.test(technologyName) ||
                    /^hide$/i.test(technologyName);

                if (isUiControl) {
                    continue;
                }

                technologyNames.push(technologyName);

                const matchingLinks = Array.from(item.querySelectorAll('a[href]'))
                    .filter(link => normalize(link.textContent) === technologyName);

                const isTextLink = matchingLinks.length > 0;

                if (!isTextLink) {
                    nonLinkedTechnologyNames.push(technologyName);
                }
            }

            return [
                [...new Set(technologyNames)],
                [...new Set(nonLinkedTechnologyNames)]
            ];
        }"
    );

    return new MicrosoftDevelopmentToolsLinkValidationResult(
        result[0],
        result[1]
    );
} 


public async Task SetColorBetaToLightAsync()
{
    await SelectColorBetaOptionAsync("Light");
}

public async Task SetColorBetaToDarkAsync()
{
    await SelectColorBetaOptionAsync("Dark");
}

public async Task<AppearanceColorState> GetAppearanceColorStateAsync()
{
    var result = await _page.EvaluateAsync<string[]>(
        @"() => {
            const normalize = text => (text || '')
                .replace(/\s+/g, ' ')
                .trim();

            const getLabelText = input => {
                if (input.labels && input.labels.length > 0) {
                    return normalize(Array.from(input.labels)
                        .map(label => label.textContent)
                        .join(' '));
                }

                const id = input.getAttribute('id');

                if (id) {
                    const label = document.querySelector(`label[for='${CSS.escape(id)}']`);
                    if (label) {
                        return normalize(label.textContent);
                    }
                }

                return normalize(input.getAttribute('aria-label') || '');
            };

            const colorInputs = Array.from(document.querySelectorAll('input[type=""radio""]'))
                .filter(input => ['Automatic', 'Light', 'Dark'].includes(getLabelText(input)));

            const checkedInput = colorInputs.find(input => input.checked);
            const selectedColor = checkedInput ? getLabelText(checkedInput) : '';

            const parseRgb = color => {
                const match = color.match(/rgba?\((\d+),\s*(\d+),\s*(\d+)/i);

                if (!match) {
                    return null;
                }

                return {
                    r: Number(match[1]),
                    g: Number(match[2]),
                    b: Number(match[3])
                };
            };

            const getUsableBackgroundColor = () => {
                const candidates = [
                    document.documentElement,
                    document.body,
                    document.querySelector('.mw-page-container'),
                    document.querySelector('#content'),
                    document.querySelector('main')
                ].filter(Boolean);

                for (const element of candidates) {
                    const color = getComputedStyle(element).backgroundColor;

                    if (color && color !== 'transparent' && !color.endsWith(', 0)')) {
                        return color;
                    }
                }

                return getComputedStyle(document.body).backgroundColor;
            };

            const backgroundColor = getUsableBackgroundColor();
            const rgb = parseRgb(backgroundColor);

            const luminance = rgb
                ? ((0.2126 * rgb.r) + (0.7152 * rgb.g) + (0.0722 * rgb.b))
                : 255;

            const isDarkModeApplied =
                selectedColor === 'Dark' &&
                luminance < 128;

            return [
                selectedColor,
                backgroundColor,
                String(isDarkModeApplied)
            ];
        }"
    );

    return new AppearanceColorState(
        result[0],
        result[1],
        bool.Parse(result[2])
    );
}

private async Task SelectColorBetaOptionAsync(string optionName)
{
    await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

    await _page.WaitForFunctionAsync(
        @"() => {
            const text = document.body?.textContent || '';
            return text.includes('Color') &&
                   text.includes('Light') &&
                   text.includes('Dark');
        }",
        null,
        new PageWaitForFunctionOptions
        {
            Timeout = 60000
        }
    );

    var radioOption = _page.GetByRole(
        AriaRole.Radio,
        new PageGetByRoleOptions
        {
            Name = optionName,
            Exact = true
        }
    );

    if (await radioOption.CountAsync() > 0)
    {
        await radioOption.First.CheckAsync(new LocatorCheckOptions
        {
            Force = true
        });
    }
    else
    {
        var clicked = await _page.EvaluateAsync<bool>(
            @"optionName => {
                const normalize = text => (text || '')
                    .replace(/\s+/g, ' ')
                    .trim();

                const labels = Array.from(document.querySelectorAll('label'))
                    .filter(label => normalize(label.textContent) === optionName);

                for (const label of labels) {
                    const forAttribute = label.getAttribute('for');

                    if (forAttribute) {
                        const input = document.getElementById(forAttribute);

                        if (input && input.type === 'radio') {
                            input.click();
                            return true;
                        }
                    }

                    const nestedInput = label.querySelector('input[type=""radio""]');

                    if (nestedInput) {
                        nestedInput.click();
                        return true;
                    }
                }

                return false;
            }",
            optionName
        );

        if (!clicked)
        {
            throw new InvalidOperationException($"Could not find Color (beta) option: {optionName}");
        }
    }

    await _page.WaitForFunctionAsync(
        @"optionName => {
            const normalize = text => (text || '')
                .replace(/\s+/g, ' ')
                .trim();

            const getLabelText = input => {
                if (input.labels && input.labels.length > 0) {
                    return normalize(Array.from(input.labels)
                        .map(label => label.textContent)
                        .join(' '));
                }

                const id = input.getAttribute('id');

                if (id) {
                    const label = document.querySelector(`label[for='${CSS.escape(id)}']`);

                    if (label) {
                        return normalize(label.textContent);
                    }
                }

                return normalize(input.getAttribute('aria-label') || '');
            };

            return Array.from(document.querySelectorAll('input[type=""radio""]'))
                .some(input =>
                    input.checked &&
                    getLabelText(input) === optionName
                );
        }",
        optionName,
        new PageWaitForFunctionOptions
        {
            Timeout = 10000
        }
    );
}
}