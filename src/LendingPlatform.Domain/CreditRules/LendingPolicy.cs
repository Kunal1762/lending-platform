using LendingPlatform.Domain.Underwriting;

namespace LendingPlatform.Domain.CreditRules;

public class LendingPolicy : ILendingPolicy
{
    public DecisionResult Evaluate(decimal loanAmount, decimal assetValue, int creditScore)
    {
        if (assetValue <= 0)
            throw new ArgumentOutOfRangeException(nameof(assetValue),
                "Asset value must be greater than zero.");

        var ltv = CalculateLtv(loanAmount, assetValue);

        // 1. General size limits apply first, regardless of LTV or credit score.
        if (loanAmount < LendingRules.MinLoanAmount)
            return DecisionResult.Declined(ltv,
                $"Loan amount is below the minimum of {LendingRules.MinLoanAmount:C0}.");

        if (loanAmount > LendingRules.MaxLoanAmount)
            return DecisionResult.Declined(ltv,
                $"Loan amount exceeds the maximum of {LendingRules.MaxLoanAmount:C0}.");

        // 2. Large loans (>= £1m) follow a separate, stricter path.
        return loanAmount >= LendingRules.LargeLoanThreshold
            ? EvaluateLargeLoan(ltv, creditScore)
            : EvaluateStandardLoan(ltv, creditScore);
    }

   
    /// <summary>
    /// ltv is returned as percentage. its a 2 decimal perdentage value, round only when 
    /// displaying can cause errot
    /// </summary>
    /// <param name="loanAmount"></param>
    /// <param name="assetValue"></param>
    /// <returns></returns>
    public static decimal CalculateLtv(decimal loanAmount, decimal assetValue)
        => loanAmount / assetValue * 100m;

    private static DecisionResult EvaluateLargeLoan(decimal ltv, int creditScore)
    {
        // 60% or less, round the ltv
        if (ltv > LendingRules.LargeLoanMaxLtv)
            return DecisionResult.Declined(ltv,
                $"LTV of {ltv:F2}% exceeds the {LendingRules.LargeLoanMaxLtv}% maximum for loans of £1m or more.");

        if (creditScore < LendingRules.LargeLoanMinCreditScore)
            return DecisionResult.Declined(ltv,
                $"Credit score of {creditScore} is below the minimum of {LendingRules.LargeLoanMinCreditScore} for loans of £1m or more.");

        return DecisionResult.Approved(ltv);
    }

/// <summary>
/// the decision is separated into bands we first check <60 then 60 to 80 and 80 to 90
/// the condition satisfied first drives the decision
/// </summary>
/// <param name="ltv"></param>
/// <param name="creditScore"></param>
/// <returns></returns>
    private static DecisionResult EvaluateStandardLoan(decimal ltv, int creditScore)
    {
        // The bands in the brief overlap literally — 50% LTV satisfies
        // "< 60%", "< 80%" AND "< 90%" all at once. Treated here as ordered,
        // first-match bands (documented in ASSUMPTIONS.md).
        if (ltv >= LendingRules.StandardBand3MaxLtv)
            return DecisionResult.Declined(ltv,
                $"LTV of {ltv:F2}% is at or above the {LendingRules.StandardBand3MaxLtv}% maximum.");

        var requiredScore = ltv switch
        {
            < LendingRules.StandardBand1MaxLtv => LendingRules.StandardBand1MinCreditScore,
            < LendingRules.StandardBand2MaxLtv => LendingRules.StandardBand2MinCreditScore,
            _                                   => LendingRules.StandardBand3MinCreditScore
        };

        return creditScore >= requiredScore
            ? DecisionResult.Approved(ltv)
            : DecisionResult.Declined(ltv,
                $"Credit score of {creditScore} is below the minimum of {requiredScore} required at an LTV of {ltv:F2}%.");
    }
}