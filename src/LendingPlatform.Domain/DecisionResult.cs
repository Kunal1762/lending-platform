namespace LendingPlatform.Domain;


/// The outcome of underwriting a single application: the decision made,
/// the LTV that was calculated, and the reason if declined.

public record DecisionResult(
    LoanDecision Decision,
    decimal LoanToValue,
    string? DeclineReason)
{
    public bool IsApproved => Decision == LoanDecision.Approved;

    public static DecisionResult Approved(decimal ltv) =>
        new(LoanDecision.Approved, ltv, null);

    public static DecisionResult Declined(decimal ltv, string reason) =>
        new(LoanDecision.Declined, ltv, reason);
}