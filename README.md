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

### 8.1 Separating the Domain User from the Identity User

Authentication data (email, password hash, roles) lives in `ApplicationUser`
(Infrastructure/Identity, managed by ASP.NET Core Identity), while business data
(full name, preferred language, active status, audit fields) lives in the
`User` domain entity. Both share the same `Id`.

This keeps the Domain layer free from any dependency on ASP.NET Core Identity,
so the domain model can be tested in isolation and the identity provider can be
replaced without touching business logic. The cost is that some operations
(e.g. creating a user) must create both records inside one transaction.

### 8.2 Writes: Generic Repository + Unit of Work

All writes (create, update, delete) go through the Generic Repository and the
Unit of Work, so persistence code is reused, `SaveChangesAsync()` is called in a
single place, and multi-step operations can run inside one transaction.

### 8.3 Reads: DbContext directly with AsNoTracking + DTO projection

Per **SRS §3.3, Rule 5**, read queries bypass the Generic Repository and query
`DbContext` directly. Reads don't need change tracking or domain behavior, so
`AsNoTracking` and `Select()` into DTOs fetch only the columns the response
needs, and the Generic Repository stays focused on writes.

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
