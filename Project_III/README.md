# Virtual Event Management System – Web API

**B4_AtharvaBibave_upGradVirtualEventManagementSystem-III**

---

## Prerequisites

- Visual Studio 2022
- .NET 8 SDK
- SQL Server Management Studio (SSMS)
- Postman (for API testing)

---

## Database Setup (Do this first)

1. Open **SQL Server Management Studio (SSMS)**
2. Connect using:
   - Server Name: `(localdb)\MSSQLLocalDB`
   - Authentication: `Windows Authentication`
3. Click **File → Open** and open `database/script.sql`
4. Press **F5** to run the script
5. You should see `EventManagementSystemDB` appear in Object Explorer

---

## Running the Application

1. Open `B4_AtharvaBibave_upGradVirtualEventManagementSystem-III.sln` in Visual Studio 2022
2. Set `VirtualEventManagementSystem.WebAPI` as the startup project
3. Press **F5** or click the green Run button
4. Swagger UI will open automatically at `https://localhost:{port}/swagger`

---

## Authentication

This API uses **JWT Bearer Token** authentication.

1. Call `POST /api/v1/auth/login` with valid credentials
2. Copy the token from the response
3. In Swagger, click **Authorize** and enter: `Bearer <your_token>`
4. All protected endpoints will now be accessible

### Default Credentials

| Role        | Email                        | Password       |
| ----------- | ---------------------------- | -------------- |
| Admin       | `admin@ems.com`              | `Admin@123`    |
| Participant | `ankitdudhe.codes@gmail.com` | `Password@123` |

---

## Project Structure

```
B4_AtharvaBibave_upGradVirtualEventManagementSystem-III/
├── Src/
│   ├── EMS.Application/                        → Service/business logic layer
│   ├── EMS.DAL/                                → Database layer
│   │   ├── Models/                             → Entity models
│   │   ├── Repositories/                       → Data access logic
│   │   ├── Migrations/                         → EF Core migrations
│   │   └── Data/EMSDbContext.cs                → EF Core DbContext
│   └── EMS.WebAPI/                             → ASP.NET Core Web API
│       ├── Controllers/                        → API endpoints
│       ├── Middleware/                         → Global exception handler
│       └── appsettings.json                    → JWT & DB config
└── Tests/
    └── EMS.DAL.Tests/                          → NUnit test project
        └── Controllers/                        → Unit tests
```

---

## Tech Stack

- **Framework:** ASP.NET Core Web API (.NET 8)
- **Database:** Microsoft SQL Server (LocalDB)
- **ORM:** Entity Framework Core
- **Authentication:** JWT Bearer Tokens
- **Authorization:** Role-based (Admin / Participant)
- **Password Hashing:** BCrypt
- **Testing:** NUnit + Moq
- **API Docs:** Swagger
