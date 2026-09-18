namespace LendingPlatform.Api.Contracts;

public record LoanApplicationRequest(
    decimal LoanAmount,
    decimal AssetValue,
    int CreditScore);