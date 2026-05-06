# Event Management System

## Prerequisites

- Visual Studio 2022
- .NET 8 SDK
- SQL Server Management Studio (SSMS)

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

1. Open `EventManagementSystem.slnx` in Visual Studio 2022
2. Press **F5** or click the green Run button
3. The app will open in your browser automatically

---

## Login Credentials

### Admin

- Email: `admin@ems.com`
- Password: `Admin@123`

### Sample Participant (any of these)

- Email: `ankitdudhe.codes@gmail.com`
- Password: `Password@123` for all default added users same password
  or else you can add as per your choice!

---

## Project Structure

```
EventManagementSystem/
├── EventManagementSystem.AppUI/        → ASP.NET Core MVC Web App
│   ├── Controllers/                    → Request handlers
│   ├── Views/                          → Razor UI pages
│   ├── Models/                         → View models
│   └── appsettings.json                → Database connection config
├── EventManagementSystem.DataAccessLayer/ → Database layer
│   ├── Models/                         → Entity models
│   ├── Repositories/                   → Data access logic
│   ├── Migrations/                     → EF Core migrations
│   └── Data/EMSDbContext.cs            → EF Core DbContext
└── database/
    └── script.sql                      → Database setup script
```

---

## Tech Stack

- **Framework:** ASP.NET Core MVC (.NET 8)
- **Database:** Microsoft SQL Server (LocalDB)
- **ORM:** Entity Framework Core
- **Authentication:** Session-based + Microsoft Entra ID
- **Password Hashing:** BCrypt
