# Employee Management System

## 📌 Overview
The **Employee Management System** is an ASP.NET Core MVC application for managing employees and departments.  
It demonstrates best practices including CRUD operations, server-side validation, global exception handling with JSON logging, and database migrations.

---

## 🚀 Features

### Employee Management
- Create, Read, Update, Delete (CRUD) employees.
- Search by name, department, or show all using `*`.

### Department Management
- Create, Read, Update, Delete departments.
- Link employees to departments.

### Validation & Error Handling
- Server-side validation on forms.
- Global exception middleware to log unhandled errors.
- Logs stored as JSON files in `/Logs/log-YYYY-MM-DD.json`.

### Data & Storage
- Uses SQL Server via EF Core.
- Supports database migrations.
- Tables:
  - `Employees`
  - `Departments`

---

## 🛠️ Setup

### 1. Clone the Repository
```bash
git clone https://github.com/ozblech/arbox-employees-app.git
cd arbox-employees-app
```

### 2. Configure Database

Update the connection string in appsettings.json or use environment variables:

```bash
"ConnectionStrings": {
  "EmployeeDb": "Server=localhost;Database=EmployeesDb;User Id=sa;Password=YourPassword123;TrustServerCertificate=True;"
}
```


Replace User Id and Password with your SQL Server credentials.

If using Docker for SQL Server, run:

```bash
docker run -d -e "ACCEPT_EULA=Y" -e "SA_USER=sa" -e "SA_PASSWORD=YourStrongPassw0rd" -p 1433:1433 --name empsql mcr.microsoft.com/mssql/server:2022-latest
```
### 3. Apply Migrations
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```
This will create the necessary tables:

Employees

Departments

### 4. Run The Application
```bash
dotnet run --urls=http://localhost:5000/
```
The app will be available at:
👉 http://localhost:5000

📂 Project Structure
```pgsql
EmployeeManagement/
├─ Controllers/
│   ├─ EmployeesController.cs
│   ├─ DepartmentsController.cs
├─ Models/
│   ├─ Employee.cs
│   ├─ Department.cs
│   ├─ DepartmentViewModel.cs
├─ Middleware/
│   ├─ GlobalExceptionMiddleware.cs
├─ Data/
│   ├─ ApplicationDbContext.cs
│   ├─ Migrations/
├─ Logs/
│   ├─ log-YYYY-MM-DD.json
├─ Views/
│   ├─ Employees/
│   ├─ Departments/
├─ appsettings.json
├─ Program.cs
├─ Startup.cs
```

Global Exception Handling

Unhandled exceptions are logged to JSON files in /Logs.

Example log entry:
```json
{
  "Timestamp": "2025-08-16T12:34:56Z",
  "Message": "Object reference not set to an instance of an object.",
  "StackTrace": "at EmployeeManagement.Controllers.EmployeesController.Index() in /src/EmployeeManagement/Controllers/EmployeesController.cs:line 42"
}
```


🧪 Testing the Exception Middleware

Start the application.

Visit an invalid route, e.g., http://localhost:5000/doesnotexist.

Check the /Logs folder — a new log file should contain the exception.

✅ Requirements Met

Server-side validation

Global exception middleware with JSON logging

SQL DB with EF Core migrations

CRUD for Employees & Departments

Search functionality with * to show all
## 🐳 Running with Docker

(Create a network and connect the empsql container to it and then run the app container)
```bash
docker network create empnet
docker network connect empnet empsql
docker run -d --name employees-app \
  --network empnet \
  -p 5000:8080 \
  -e "ConnectionStrings__EmployeeDb=Server=empsql,1433;Database=EmployeesDb;User Id=sa;Password=YourStrongPassw0rd;Encrypt=False;TrustServerCertificate=True;" \
  ozblech/arbox-employees-app:latest
```

Access the app at http://localhost:5000

Make sure the SQL Server container (empsql) is running and on the same Docker network empnet.

## 🐳 Running with Docker Compose

You can run both the SQL Server and Employee Management app using Docker Compose. This setup also allows you to define the SQL Server credentials once and reuse them.


Create a file named .env in the same folder as your docker-compose.yml
```env
SA_USER=sa
SA_PASSWORD=YourStrongPassw0rd
```
Run:
docker-compose up -d


Program.cs:
```csharp
using EmployeeManagement.Services;
using EmployeeManagement.Data;
using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Middlewares;
```

* using ...; → allows you to reference classes in these namespaces without writing the full path.

* Services → your custom business logic (e.g., EmployeeService).

* Data → your EF Core DbContext and database setup.

* Microsoft.EntityFrameworkCore → official Entity Framework Core library for database access.

* Middlewares → your custom middleware classes (e.g., exception handling).

App Builder
```csharp
var builder = WebApplication.CreateBuilder(args);
```
* Creates a builder object that sets up:

  * Configuration (reads appsettings.json, env variables, secrets).

  * Logging.

  * Dependency injection container.

* args comes from command-line parameters when you run the app.

Register Services
```csharp
builder.Services.AddControllersWithViews();
```
* Registers support for MVC Controllers + Razor Views in the DI container.

* Without this, controllers and views won’t work.


Dependency Injection – Business Services
```csharp
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
```
* Adds your custom services into the DI container.

* Scoped lifetime = one instance per HTTP request.

* Whenever a controller asks for IEmployeeService, the framework injects an EmployeeService object.

👉 This is the Dependency Injection (DI) pattern – makes your code more testable and maintainable.

Database Configuration
```csharp
var connectionString = builder.Configuration.GetConnectionString("EmployeeDb");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
```

* Reads the connection string from appsettings.json (EmployeeDb).

* Registers ApplicationDbContext with DI so it can be injected into controllers/services.

* Tells EF Core to use SQL Server as the database provider.

So now, anywhere in your app, you can inject ApplicationDbContext and query the database.

Build App
```csharp
var app = builder.Build();
```
* Finalizes the configuration and creates the application object (WebApplication).

* After this point, you configure middleware and request pipeline.

Global Exception Middleware
```csharp
app.UseMiddleware<GlobalExceptionMiddleware>();
```
* Adds your custom middleware into the request pipeline.

* Handles unhandled exceptions globally (e.g., logging, returning friendly error response).

* This runs before controllers.

Ensure Database Exists
```csharp
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
}
```
* Creates a scope to resolve services (because ApplicationDbContext is scoped).

* Gets the ApplicationDbContext.

* Calls EnsureCreated() → creates the database if it doesn’t already exist.

⚠️ In production, you’d usually use migrations instead of EnsureCreated().

Error Handling (Production Only)
```csharp
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
```
* Checks environment (Development / Production).

* In Production, sets up an error handler page (/Home/Error) instead of showing stack traces to users.

Static Files
```csharp
app.UseStaticFiles();
```
* Serves static files from wwwroot folder (CSS, JS, images, etc.).

Routing
```csharp
app.UseRouting();
```

* Enables the endpoint routing system.

* Prepares the app to match incoming requests to routes defined later.

Authorization
```csharp
app.UseAuthorization();
```

* Applies authorization rules (e.g., [Authorize] attributes on controllers).

* If you also had app.UseAuthentication(), it would validate user identity first.

Controller Route Mapping
```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

* Defines the default route pattern for MVC:

  * /ControllerName/ActionName/Id

  * Example: /Employee/Details/3 → calls EmployeeController.Details(3).

* If no controller/action is given, it defaults to HomeController.Index.

Run the App
```csharp
app.Run();
```

* Starts the Kestrel web server.

* The app is now listening for HTTP requests on the configured port.