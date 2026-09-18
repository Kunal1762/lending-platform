using LendingPlatform.Domain;
using LendingPlatform.Domain.CreditRules;
using Xunit;

namespace LendingPlatform.Domain.Tests;

public class LendingPolicyTests
{
    private readonly LendingPolicy _policy = new();

    [Theory]
    // --- General size limits ---
    [InlineData(99_999, 500_000, 999, LoanDecision.Declined)]      // just below floor
    [InlineData(100_000, 500_000, 750, LoanDecision.Approved)]     // exactly at floor
    [InlineData(1_500_000, 2_500_000, 950, LoanDecision.Approved)] // exactly at ceiling
    [InlineData(1_500_001, 3_000_000, 999, LoanDecision.Declined)] // just above ceiling

    // --- Large loans (>= £1m): LTV <= 60% AND score >= 950 ---
    [InlineData(1_000_000, 2_000_000, 950, LoanDecision.Approved)] // 50% LTV
    [InlineData(1_000_000, 2_000_000, 949, LoanDecision.Declined)] // score one short
    [InlineData(1_200_000, 2_000_000, 999, LoanDecision.Approved)] // exactly 60% LTV, inclusive
    [InlineData(1_250_000, 2_000_000, 999, LoanDecision.Declined)] // 62.5% LTV

    // --- Standard loans (< £1m), band 1: LTV < 60% needs >= 750 ---
    [InlineData(500_000, 1_000_000, 750, LoanDecision.Approved)]   // 50% LTV
    [InlineData(500_000, 1_000_000, 749, LoanDecision.Declined)]

    // --- Band 2: 60% <= LTV < 80% needs >= 800 ---
    [InlineData(600_000, 1_000_000, 800, LoanDecision.Approved)]   // exactly 60%
    [InlineData(600_000, 1_000_000, 799, LoanDecision.Declined)]
    [InlineData(799_000, 1_000_000, 800, LoanDecision.Approved)]

    // --- Band 3: 80% <= LTV < 90% needs >= 900 ---
    [InlineData(800_000, 1_000_000, 900, LoanDecision.Approved)]   // exactly 80%
    [InlineData(800_000, 1_000_000, 899, LoanDecision.Declined)]
    [InlineData(899_000, 1_000_000, 900, LoanDecision.Approved)]

    // --- LTV >= 90% always declined ---
    [InlineData(900_000, 1_000_000, 999, LoanDecision.Declined)]   // exactly 90%
    [InlineData(950_000, 1_000_000, 999, LoanDecision.Declined)]
    public void Evaluate_ReturnsExpectedDecision(
        decimal loanAmount, decimal assetValue, int creditScore, LoanDecision expected)
    {
        var result = _policy.Evaluate(loanAmount, assetValue, creditScore);
        Assert.Equal(expected, result.Decision);
    }

    [Fact]
    public void Evaluate_AtExactlyOneMillion_UsesLargeLoanRules()
    {
        // At 60% LTV with a score of 900, this would pass under standard
        // rules but must fail the large-loan score threshold of 950.
        var result = _policy.Evaluate(1_000_000m, 1_666_666.67m, 900);
        Assert.Equal(LoanDecision.Declined, result.Decision);
    }

    [Fact]
    public void Evaluate_JustBelowOneMillion_UsesStandardRules()
    {
        // £999,999 at ~50% LTV only needs a score of 750.
        var result = _policy.Evaluate(999_999m, 2_000_000m, 750);
        Assert.Equal(LoanDecision.Approved, result.Decision);
    }

    [Fact]
    public void Evaluate_DoesNotRoundLtvBeforeComparing()
    {
        // 89.95% LTV must stay in band 3, not round up to 90% and get declined.
        var result = _policy.Evaluate(899_500m, 1_000_000m, 900);
        Assert.Equal(LoanDecision.Approved, result.Decision);
    }

    [Fact]
    public void Evaluate_DeclinedApplication_AlwaysHasAReason()
    {
        var result = _policy.Evaluate(50_000m, 100_000m, 999);
        Assert.False(result.IsApproved);
        Assert.False(string.IsNullOrWhiteSpace(result.DeclineReason));
    }

    [Fact]
    public void Evaluate_ZeroAssetValue_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => _policy.Evaluate(200_000m, 0m, 800));
    }
}