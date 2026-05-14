namespace Genpact.Automation.Tests.Pages;


/**
I wanted the Page Object to return a clear structured result instead of multiple loose values.
The record contains the selected color, the background color, and whether dark mode is applied.
This keeps the assertion readable and makes debugging easier.
**/


public sealed record AppearanceColorState(
    string SelectedColor,
    string BackgroundColor,
    bool IsDarkModeApplied
);