namespace LendingPlatform.Domain.Underwriting;


/// Every numeriv threshold from the lending policy, separated in one place.

public static class LendingRules
{
    public const decimal MinLoanAmount = 100_000m;
    public const decimal MaxLoanAmount = 1_500_000m;
    public const decimal LargeLoanThreshold = 1_000_000m;

    // Loans >= £1m
    public const decimal LargeLoanMaxLtv = 60m;
    public const int LargeLoanMinCreditScore = 950;

    // Loans < £1m — ordered LTV bands
    public const decimal StandardBand1MaxLtv = 60m;   // LTV < 60%
    public const int StandardBand1MinCreditScore = 750;

    public const decimal StandardBand2MaxLtv = 80m;   // 60% <= LTV < 80%
    public const int StandardBand2MinCreditScore = 800;

    public const decimal StandardBand3MaxLtv = 90m;   // 80% <= LTV < 90%
    public const int StandardBand3MinCreditScore = 900;

    public const int MinCreditScore = 1;
    public const int MaxCreditScore = 999;
}