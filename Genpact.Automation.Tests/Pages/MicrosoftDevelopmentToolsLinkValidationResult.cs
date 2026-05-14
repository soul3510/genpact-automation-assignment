namespace Genpact.Automation.Tests.Pages;

public sealed record MicrosoftDevelopmentToolsLinkValidationResult(
    IReadOnlyList<string> TechnologyNames,
    IReadOnlyList<string> NonLinkedTechnologyNames
);