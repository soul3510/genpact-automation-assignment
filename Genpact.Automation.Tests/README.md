# genpact-automation-assignment
C# Playwright automation assignment for UI and API validation on Wikipedia


Although my main recent automation work has been in Java, TypeScript and Playwright, I implemented this assignment in C# as requested, focusing on clean architecture, maintainability, and reliable UI/API validation.

##########################################################
Workflow (Github + C# SDK + Playwright + Test structure): 
##########################################################

1. Created github repository: "genpact-automation-assignment"
2. Installed .NET SDK with: "winget install Microsoft.DotNet.SDK.8" - need to make sure that the the sdk exists in the win path after installing. If not: Fix PATH temporarily with: 

$env:Path += ";C:\Program Files\dotnet"
dotnet --version

3. Create the repo root test project: "dotnet new nunit -n Genpact.Automation.Tests" 
4. Enter project folder: "cd Genpact.Automation.Tests"
5. Add Playwright for C#: "dotnet add package Microsoft.Playwright.NUnit"
6. Build the project: "dotnet build"
7. Install Playwright browsers: "powershell -ExecutionPolicy Bypass -File bin/Debug/net8.0/playwright.ps1 install" (For regular PowerShell - what i work with in my vsc)
8. Verify tests are running: "dotnet test"

9. Repository basic structure:

I separated the framework into layers: Tests, Pages, Services, Utils, and Base.
The test itself stays readable and focuses on the business validation, while UI logic is inside the Page Object, API logic is inside the API client, and text normalization is reusable utility logic.

Genpact.Automation.Tests/
├── Base/
├── Pages/          - POM - Page Object Model
├── Services/
├── Tests/          
└── Utils/           


##########################################################
Task 1: DebuggingFeaturesTests.cs
##########################################################

Requirement:
• Extract the “Debugging features” section:
    o via UI (POM approach)
    o via API (MediaWiki Parse API)
• Normalize both texts
• Count unique words
• Assert that both counts are equal

Explanation:
I created a C# NUnit Playwright project with a clean structure.
The UI part uses a Page Object to open the Playwright Wikipedia page and extract only the Debugging features section.
The API part uses the MediaWiki Parse API. Instead of hardcoding the section index, I first retrieve the page sections, find the matching section by title or anchor, and then request that section’s HTML.
Since UI and API return slightly different formatting, I normalize both texts before comparison.
Finally, the test counts unique words from both sources and asserts equality.

Genpact.Automation.Tests/
├── Base/
├── Pages/              -> Task 1: PlaywrightWikiPage.cs
├── Services/           -> Task 1: MediaWikiApiClient.cs
├── Tests/              -> Task 1: DebuggingFeaturesTests.cs
└── Utils/              -> Task 1: TextNormalizer.cs

##########################################################
Task 2:  MicrosoftDevelopmentToolsTests.cs
##########################################################

Requirement:
Via UI: go to Microsoft development tools section and validate that all the technology names under this section are text links. If one is not a link, the test should fail.

Explanation:
Task 2 is implemented as a strict validation.
At the time of execution, the test finds 124 technology names under the "Microsoft development tools" section.
The test fails because the following technology names are rendered as plain text instead of links:

- Visual Basic
- Playwright

This is the expected behavior according to the assignment requirement: if a technology name is not a text link, the test should fail.
I designed the test to fail correctly, not just to pass.

(The assignment says it is my decision whether to implement it as one test or multiple tests.
I chose one test because the requirement is one business rule: all technology names in this section should be links.
The failure message still gives detailed diagnostics by listing exactly which technology names are not links.)


Test Scenrio Logic:

1. Navigate to the Playwright Wikipedia page.
2. Scroll/find the Microsoft development tools box.
3. Click Show if the box is collapsed.
4. Collect all actual technology-name list items inside the box.
5. Ignore group headers and UI controls.
6. Assert at least one technology was found.
7. Validate each technology name has a matching <a href>.
8. Fail with a clear list of non-linked names.

Genpact.Automation.Tests/
├── Base/
├── Pages/              -> Task 2: PlaywrightWikiPage.cs, MicrosoftDevelopmentToolsLinkValidationResult.cs
├── Services/           -> Task 2: 
├── Tests/              -> Task 2: MicrosoftDevelopmentToolsTests.cs
└── Utils/              -> Task 2: 



##########################################################
Task 3:  
##########################################################

Requirement:
Via UI: Go to the “Color (beta)” section (from the right) and change the color to “Dark”
validate that the color actually changed


Please note: 
The requirement assume that the section is: “Color (beta)”  but the actual is "“Color”. So I build the test assuming the right text is "Color". 


Explanation:
The test opens the Wikipedia Playwright page, sets the Color option to Light first to establish a known baseline, then changes it to Dark.
After that, it validates both that the selected option is Dark and that the page appearance actually changed to a dark theme.

Regarding the page object method i used: 
1. I used background color check in the method since it is actually reflects the actual CSS and it stronger validation then HTML attribute alone. 
2. I also used calculate luminance in the method since color can changed between browsers or UI versions, so it is not a good idea to check a strict rgb color like rgb(16, 20, 24)
3. I also used JavaScript evaluation in the Page Object since the page is a dynamic page and javascript allows for better dom inspecting

Genpact.Automation.Tests/
├── Base/
├── Pages/          -> Task 3: PlaywrightWikiPage.cs, AppearanceColorState.cs
├── Services/
├── Tests/          -> Task 3: AppearanceColorTests.cs
└── Utils/    