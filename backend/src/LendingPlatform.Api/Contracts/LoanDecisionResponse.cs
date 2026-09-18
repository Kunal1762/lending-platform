using LendingPlatform.Domain;

namespace LendingPlatform.Api.Contracts;

public record LoanDecisionResponse(
    Guid Id,
    decimal LoanAmount,
    decimal AssetValue,
    int CreditScore,
    decimal LoanToValue,
    string Decision,
    string? DeclineReason,
    DateTime CreatedAtUtc)
{
    public static LoanDecisionResponse From(LoanApplication a) => new(
        a.Id,
        a.LoanAmount,
        a.AssetValue,
        a.CreditScore,
        Math.Round(a.LoanToValue, 2),   // rounded for display only, never for comparisons
        a.Decision.ToString(),
        a.DeclineReason,
        a.CreatedAtUtc);
}