# genpact-automation-assignment
C# Playwright automation assignment for UI and API validation on Wikipedia


Although my main recent automation work has been in Java, TypeScript and Playwright, I implemented this assignment in C# as requested, focusing on clean architecture, maintainability, and reliable UI/API validation.

Workflow (Github + C# SDK + Playwright + Test structure): 
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

