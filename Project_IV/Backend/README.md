# Virtual Event Management System – Web API

**Project:** B4_AtharvaBibave_upGradVirtualEventManagementSystem-III  
**Submitted by:** Atharva Mahesh Bibave  
**Program:** upGrad .NET + Angular Full Stack Development (Enterprise PAT)

---

## Prerequisites

- Visual Studio 2022
- .NET 8 SDK
- SQL Server LocalDB (included with Visual Studio)
- Postman (for API testing)

---

## How to Run

1. Extract the ZIP and open `EMS.sln` in Visual Studio 2022
2. Set `EMS.WebAPI` as the startup project
3. Press **F5** or click the green Run button

> **The database is created automatically on first run.** No SQL scripts, no migrations command needed.  
> Swagger UI will open at `https://localhost:{port}/swagger`

---

## Default Login Credentials

| Role        | Email                                     | Password      |
| ----------- | ----------------------------------------- | ------------- |
| Admin       | admin@ems.com                             | Admin@123     |
| Participant | Register via `POST /api/v1/auth/register` | (your choice) |

---

## Authentication

This API uses **JWT Bearer Token** authentication.

1. Call `POST /api/v1/auth/login` with your credentials
2. Copy the `token` value from the response
3. In Swagger, click **Authorize** and enter: `Bearer <your_token>`
4. All protected endpoints will now be accessible

---

## Project Structure

```
EMS.sln
├── EMS.DAL/                  → Data Access Layer
│   ├── Models/               → Entity models
│   ├── Data/                 → DbContext
│   ├── Repositories/         → Data access logic
│   └── Migrations/           → EF Core migrations (auto-applied on startup)
├── EMS.Application/          → Business Logic Layer
│   ├── Services/             → Service classes
│   ├── DTOs/                 → Request / Response DTOs
│   └── Validation/           → Custom validation attributes
├── EMS.WebAPI/               → ASP.NET Core Web API
│   ├── Controllers/          → API endpoints
│   ├── Middleware/           → Global exception handler
│   └── appsettings.json      → JWT & DB configuration
└── EMS.DAL.Tests/            → NUnit unit tests
```

---

## Tech Stack

| Area             | Technology                       |
| ---------------- | -------------------------------- |
| Framework        | ASP.NET Core Web API (.NET 8)    |
| Database         | SQL Server LocalDB               |
| ORM              | Entity Framework Core            |
| Authentication   | JWT Bearer Tokens                |
| Authorization    | Role-based (Admin / Participant) |
| Password Hashing | BCrypt                           |
| Caching          | In-Memory Cache                  |
| API Versioning   | URL-based (`/api/v1/`)           |
| Testing          | NUnit + Moq                      |
| API Docs         | Swagger / OpenAPI                |

---

## API Endpoints Overview

| Module       | Endpoint                             | Access        |
| ------------ | ------------------------------------ | ------------- |
| Auth         | POST `/api/v1/auth/register`         | Public        |
| Auth         | POST `/api/v1/auth/login`            | Public        |
| Events       | GET/POST `/api/v1/events`            | Admin         |
| Events       | GET `/api/v1/events/{id}`            | Authenticated |
| Sessions     | GET/POST `/api/v1/sessions`          | Admin         |
| Speakers     | GET/POST `/api/v1/speakers`          | Admin         |
| Categories   | GET/POST `/api/v1/categories`        | Admin         |
| Participants | POST `/api/v1/participants/register` | Participant   |
| Participants | GET `/api/v1/participants`           | Admin         |
| Users        | GET `/api/v1/users`                  | Admin         |

> Full endpoint details available in Swagger UI after running the project.
