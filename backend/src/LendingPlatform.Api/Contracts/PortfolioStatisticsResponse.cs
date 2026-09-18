namespace LendingPlatform.Api.Contracts;

public record PortfolioStatisticsResponse(
    int TotalApplications,
    int ApprovedCount,
    int DeclinedCount,
    decimal TotalValueOfLoansWritten,
    decimal MeanLoanToValue);