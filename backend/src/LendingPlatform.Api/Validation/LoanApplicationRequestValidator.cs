using LendingPlatform.Api.Contracts;
using LendingPlatform.Domain.CreditRules;
using LendingPlatform.Domain.Underwriting; 

namespace LendingPlatform.Api.Validation;

public static class LoanApplicationRequestValidator
{
    public static Dictionary<string, string[]> Validate(LoanApplicationRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (request.LoanAmount <= 0)
            errors[nameof(request.LoanAmount)] = ["Loan amount must be greater than zero."];

        if (request.AssetValue <= 0)
            errors[nameof(request.AssetValue)] = ["Asset value must be greater than zero."];

        if (request.CreditScore < LendingRules.MinCreditScore ||
            request.CreditScore > LendingRules.MaxCreditScore)
            errors[nameof(request.CreditScore)] =
                [$"Credit score must be between {LendingRules.MinCreditScore} and {LendingRules.MaxCreditScore}."];

        return errors;
    }
}