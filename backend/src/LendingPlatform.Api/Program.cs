using LendingPlatform.Api.Persistence;
using Microsoft.EntityFrameworkCore;
using LendingPlatform.Domain.CreditRules;
using LendingPlatform.Domain.Underwriting;
using LendingPlatform.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LendingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
    builder.Services.AddSingleton<ILendingPolicy, LendingPolicy>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

const string CorsPolicy = "LocalFrontend";
builder.Services.AddCors(options =>
    options.AddPolicy(CorsPolicy, policy => policy
        .WithOrigins("http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod()));

var app = builder.Build();

app.UseCors(CorsPolicy);

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<LendingDbContext>().Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapApplicationEndpoints();
app.MapStatisticsEndpoints();

app.Run();