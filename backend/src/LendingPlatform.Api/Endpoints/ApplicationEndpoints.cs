using LendingPlatform.Api.Contracts;
using LendingPlatform.Api.Persistence;
using LendingPlatform.Api.Validation;
using LendingPlatform.Domain;
using LendingPlatform.Domain.CreditRules; // your actual namespace
using LendingPlatform.Domain.Underwriting;
using Microsoft.EntityFrameworkCore;

namespace LendingPlatform.Api.Endpoints;

public static class ApplicationEndpoints
{
    public static void MapApplicationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/applications").WithTags("Applications");

        group.MapPost("/", async (
            LoanApplicationRequest request,
            ILendingPolicy policy,
            LendingDbContext db) =>
        {
            var errors = LoanApplicationRequestValidator.Validate(request);
            if (errors.Count > 0)
                return Results.ValidationProblem(errors);

            var result = policy.Evaluate(
                request.LoanAmount, request.AssetValue, request.CreditScore);

            var application = new LoanApplication(
                request.LoanAmount, request.AssetValue, request.CreditScore, result);

            db.LoanApplications.Add(application);
            await db.SaveChangesAsync();

            var response = LoanDecisionResponse.From(application);
            return Results.Created($"/api/applications/{application.Id}", response);
        });

        group.MapGet("/", async (LendingDbContext db) =>
        {
            var applications = await db.LoanApplications
                .AsNoTracking()
                .OrderByDescending(a => a.CreatedAtUtc)
                .Take(100)
                .ToListAsync();

            return Results.Ok(applications.Select(LoanDecisionResponse.From));
        });
    }
}