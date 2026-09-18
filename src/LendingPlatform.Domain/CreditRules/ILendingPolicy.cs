namespace LendingPlatform.Domain.Underwriting;

public interface ILendingPolicy
{
    DecisionResult Evaluate(decimal loanAmount, decimal assetValue, int creditScore);
}