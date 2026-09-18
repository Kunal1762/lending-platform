# Lending Platform

A basic lending platform built as a technical test: an API that evaluates
loan applications against a fixed policy, persists them, and
reports portfolio statistics. Frontend to follow.

## Stack
- **Backend:** C# / ASP.NET Core (.NET 10), EF Core, PostgreSQL
- **Frontend:** React 
- **Tests:** xUnit 

## Project structure

backend/
├── src/
│ ├── LendingPlatform.Domain/ # business rules, no dependencies on anything
│ └── LendingPlatform.Api/ # HTTP API, persistence, validation
├── tests/
│ └── LendingPlatform.Domain.Tests/
└── docker-compose.yml # Postgres for local development
frontend/ 


## Running the backend

**Prerequisites:** .NET 10 SDK, Docker Desktop.

1. Start Postgres:

cd backend
docker compose up -d

2. Run the API (migrations apply automatically on startup):

dotnet run --project src/LendingPlatform.Api

3. Open Swagger at the URL printed in the console, e.g.
   `http://localhost:5058/swagger`

## Running the tests

cd backend
dotnet test


No database or Docker required — the rules engine has zero infrastructure
dependencies, so these run in isolation.(for the tests)

## API endpoints

| Method | Path                 | Description                          |
|--------|----------------------|---------------------------------------|
| POST   | `/api/applications`  | Submit an application, get a decision |
| GET    | `/api/applications`  | List the most recent applications     |
| GET    | `/api/statistics`    | Portfolio-wide aggregate statistics   |

## Design notes

- **Rules engine is a dependency-free class library.** `LendingPlatform.Domain`
  has no reference to ASP.NET, EF Core, or anything else — it can be unit
  tested with no database and reused unchanged if the delivery mechanism
  ever changed (e.g. a batch job instead of an API).
- **Decisions are stored, not recomputed.** An application's LTV and outcome
  are persisted at the time of decision. If the rules change
  later, historical decisions must not silently change with them.
- **Money is `decimal` everywhere**, never `double`/`float`, to avoid
  floating-point rounding errors in financial figures.
- See `ASSUMPTIONS.md` for how ambiguities in the brief's business rules
  were interpreted.

## What I'd do differently for production

- Add authentication/authorization — currently the API is fully open(role based so that only authorized people can use it).
- Add structured logging and an audit trail beyond the stored decision.
- Add pagination to `GET /api/applications` rather than a fixed `Take(100)`.
- Move credentials out of `docker-compose.yml` into a secrets manager —
  they're committed here only for reviewer convenience.


## Validation vs. decline
A structurally invalid request (e.g. asset value of zero or negative) is
rejected as a 400 Bad Request during input validation, not evaluated as a
declined loan application. "Zero collateral" is not a lending decision —
it's a malformed request.

## Statistics definitions
- "Total value of loans written to date" = sum of loan amounts for
  **approved** applications only.
- "Mean average LTV across all applications" = average LTV across
  **every** application, approved and declined alike.

