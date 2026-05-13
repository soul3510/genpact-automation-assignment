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
├── Pages/          - Page Object
├── Services/
├── Tests/          
└── Utils/           


##########################################################
Task 1:  
##########################################################

• Extract the “Debugging features” section:
    o via UI (POM approach)
    o via API (MediaWiki Parse API)
• Normalize both texts
• Count unique words
• Assert that both counts are equal

I created a C# NUnit Playwright project with a clean structure.
The UI part uses a Page Object to open the Playwright Wikipedia page and extract only the Debugging features section.
The API part uses the MediaWiki Parse API. Instead of hardcoding the section index, I first retrieve the page sections, find the matching section by title or anchor, and then request that section’s HTML.
Since UI and API return slightly different formatting, I normalize both texts before comparison.
Finally, the test counts unique words from both sources and asserts equality.

Genpact.Automation.Tests/
├── Base/
├── Pages/
├── Services/           -> Task 1: MediaWikiApiClient.cs
├── Tests/              -> Task 1: DebuggingFeaturesTests.cs
└── Utils/              -> Task 1: text normalization