using LendingPlatform.Api.Contracts;
using LendingPlatform.Api.Persistence;
using LendingPlatform.Domain;
using Microsoft.EntityFrameworkCore;

namespace LendingPlatform.Api.Endpoints;

public static class StatisticsEndpoints
{
    public static void MapStatisticsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/statistics", async (LendingDbContext db) =>
        {
            var all = db.LoanApplications.AsNoTracking();

            var total = await all.CountAsync();
            var approvedCount = await all.CountAsync(a => a.Decision == LoanDecision.Approved);

            // "Total value of loans written" = approved loans only.
            var totalWritten = await all
                .Where(a => a.Decision == LoanDecision.Approved)
                .SumAsync(a => a.LoanAmount);

            // "Mean LTV across all applications" = every application, approved or not.
            // Cast to nullable so AverageAsync returns null (not an exception) on an empty set.
            var meanLtv = await all.AverageAsync(a => (decimal?)a.LoanToValue) ?? 0m;

            return Results.Ok(new PortfolioStatisticsResponse(
                TotalApplications: total,
                ApprovedCount: approvedCount,
                DeclinedCount: total - approvedCount,
                TotalValueOfLoansWritten: totalWritten,
                MeanLoanToValue: Math.Round(meanLtv, 2)));
        })
        .WithTags("Statistics");
    }
}