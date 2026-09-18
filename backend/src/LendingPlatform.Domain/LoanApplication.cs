namespace LendingPlatform.Domain;

/// <summary>
/// A submitted application and the decision recorded against it. The
/// decision and LTV are stored, not recomputed on read — an underwriting
/// outcome is a historical fact, and changing the rules later must not
/// retroactively rewrite past decisions.
/// </summary>
public class LoanApplication
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public decimal LoanAmount { get; private set; }
    public decimal AssetValue { get; private set; }
    public int CreditScore { get; private set; }

    public decimal LoanToValue { get; private set; }
    public LoanDecision Decision { get; private set; }
    public string? DeclineReason { get; private set; }

    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    // Required by EF Core to construct instances when reading from the database.
    private LoanApplication() { }

    public LoanApplication(
        decimal loanAmount,
        decimal assetValue,
        int creditScore,
        DecisionResult result)
    {
        LoanAmount = loanAmount;
        AssetValue = assetValue;
        CreditScore = creditScore;
        LoanToValue = result.LoanToValue;
        Decision = result.Decision;
        DeclineReason = result.DeclineReason;
    }
}