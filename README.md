# DonationHub

DonationHub is a donation management platform built with **ASP.NET Core Web API, .NET 10, Entity Framework Core, and SQL Server**.

The project follows Clean Architecture principles to separate business logic, application use cases, infrastructure concerns, and API presentation.

---

## 1. Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server (2022 or compatible)
- SQL Server Management Studio (SSMS) – optional
- Visual Studio 2026 or Visual Studio Code
- EF Core CLI tools

Install (or update) the EF Core CLI:

```bash
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
```

---

## 2. Project Structure

```text
DonationHub/
├── DonationHub.Domain/
│   ├── Common/
│   ├── Constants/
│   ├── Entities/
│   ├── Enums/
│   ├── Helper/
│   └── ValueObjects/
│
├── DonationHub.Application/
│   ├── DTOs/
│   ├── Exceptions/
│   ├── Features/
│   └── Interfaces/
│
├── DonationHub.Infrastructure/
│   ├── Identity/
│   ├── Implementaion/        (Repositories, Services, UnitOfWork)
│   ├── Migrations/
│   ├── Options/
│   └── Persistence/          (DbContext, Configurations, Interceptors, Seeding)
│
├── DonationHub.Api/
│   ├── Controllers/
│   ├── ExceptionHandlers/
│   └── Program.cs
│
└── Donation.Tests/           (unit tests)
```

### Architecture Responsibilities

- **Domain:** entities, value objects, business rules.
- **Application:** DTOs, service/repository abstractions, application services.
- **Infrastructure:** EF Core, repositories, Identity, JWT, seeding, options.
- **API:** HTTP endpoints, OpenAPI/Scalar, exception handling.

---

## 3. Database Configuration

The connection string is read from the `DefaultConnection` key in `DonationHub.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=DonationHubDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

Change `Server=.` to your SQL Server instance if needed.

> **Security note:** never commit production credentials. Use User Secrets or environment variables for sensitive values.

---

## 4. Database Migrations

The project uses EF Core Code First migrations (stored in `DonationHub.Infrastructure/Migrations`).

Apply existing migrations (run from the solution root):

```bash
dotnet ef database update --project DonationHub.Infrastructure --startup-project DonationHub.Api
```

Create a new migration:

```bash
dotnet ef migrations add MigrationName --project DonationHub.Infrastructure --startup-project DonationHub.Api
dotnet ef database update --project DonationHub.Infrastructure --startup-project DonationHub.Api
```

---

## 5. Secrets: JWT Key, Seeded Admin and Dev Test Users

Secrets are **not** stored in `appsettings.json`. Set them with User Secrets (never commit the real values).

```bash
dotnet user-secrets init --project DonationHub.Api
```

### JWT signing key (required)

The app will not start without it. It must be at least 32 characters.

```bash
dotnet user-secrets set "Jwt:SecretKey" "replace-with-a-long-random-secret-32+chars" --project DonationHub.Api
```

### Seeded Admin

```bash
dotnet user-secrets set "SeedAdmin:Password" "YourAdminPassword123" --project DonationHub.Api
```

### Dev test users (Development environment only)

```bash
dotnet user-secrets set "SeedDevUsers:CampaignManager:Password" "YourManagerPassword123" --project DonationHub.Api
dotnet user-secrets set "SeedDevUsers:Donor:Password" "YourDonorPassword123" --project DonationHub.Api
```

Emails and full names have defaults in `appsettings.json` (`SeedAdmin`, `SeedDevUsers` sections). To override them, set `SeedAdmin:Email`, `SeedDevUsers:CampaignManager:Email`, etc. the same way.

Notes:

- Passwords must satisfy the Identity policy: at least 8 characters, with a digit, an uppercase and a lowercase letter.
- Seeding runs automatically on application startup (roles, admin, categories; dev users only in Development), so apply migrations first.
- A seed user with a missing email/password is skipped with a warning.

---

## 6. Running the Application

```bash
dotnet run --project DonationHub.Api
```

Or run `DonationHub.Api` from Visual Studio. The `https` launch profile serves on `https://localhost:7151` and opens Scalar.

Make sure SQL Server is running and migrations have been applied.

---

## 7. API Documentation and Testing

In the **Development** environment the API exposes OpenAPI with the Scalar UI:

| What | URL |
|------|-----|
| Scalar UI | `/scalar/v1` (e.g. `https://localhost:7151/scalar/v1`) |
| OpenAPI JSON | `/openapi/v1.json` |

Swagger UI is not configured in this project; Scalar is used instead.

### Testing protected endpoints

1. Open Scalar.
2. Call `POST /api/v1/auth/login` with the seeded credentials.
3. Copy the returned access token.
4. Open the **Authorization / Bearer** section in Scalar and paste the token **by itself**, without the `Bearer ` prefix.
5. Call the protected endpoints (for example, admin-only endpoints require the Admin account).

---

## 8. Design Decisions

### 8.1 Writes: Generic Repository + Unit of Work

All write operations (create, update, delete) go through the Generic Repository and the Unit of Work:

- Business logic is separated from data access.
- Common persistence code is reused through the Generic Repository.
- A single `SaveChangesAsync()` in the Unit of Work commits changes.
- Multi-step operations run inside one transaction (`ExcuteInTRansactionAsync`).

### 8.2 Reads: DbContext directly with AsNoTracking + DTO projection

Per **SRS §3.3, Rule 5**, read queries use `DbContext` directly instead of the Generic Repository:

- **AsNoTracking:** no change tracking for read-only queries, lowering memory use and improving speed.
- **DTO projection:** `Select()` fetches only the needed columns and returns DTOs instead of full entities.

Illustrative example:

```csharp
var items = await _context.Categories
    .AsNoTracking()
    .OrderBy(c => c.NameEn)
    .Select(c => new CategoryListItemDto(c.Id, c.NameEn))
    .ToListAsync(ct);
```

### 8.3 Clean Architecture

Responsibilities are split across Domain, Application, Infrastructure and API, reducing coupling and making the code easier to test and extend.

---

## 9. Security Considerations

- JWT Bearer authentication protects secured endpoints.
- Passwords are managed by ASP.NET Core Identity.
- Refresh tokens are stored as hashes, not plain text.
- Role-based authorization (`[Authorize(Roles = ...)]`) restricts administrative endpoints.
- Secrets (JWT key, seed passwords) live in User Secrets during development.

---

## 10. Notes

- Do not commit User Secrets or production credentials.
- Apply migrations before running the application.
- Use the Development environment for local testing.
- Keep the .NET SDK and EF Core tools compatible with `net10.0`.
