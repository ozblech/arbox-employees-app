Employee Management System
📌 Overview
The Employee Management System is an ASP.NET Core MVC application for managing employees and departments.
It demonstrates best practices in CRUD operations, server-side validation, global exception handling with JSON logging, and database migrations.


🚀 Features
    • Employee Management
        ◦ Create, Read, Update, Delete (CRUD) employees.
        ◦ Search by name, department, or show all with *.
    • Department Management
        ◦ Create, Read, Update, Delete departments.
        ◦ Link employees to departments.
    • Validation & Error Handling
        ◦ Server-side validation on forms.
        ◦ Global exception middleware to log unhandled errors.
        ◦ Logs stored as JSON files:
pgsql
/Logs/log-YYYY-MM-DD.json

    • Data & Storage
        ◦ Uses SQL Server (via EF Core).
        ◦ Supports database migrations.
        ◦ Tables: Employees, Departments.

🛠️ Setup
1. Clone the Repository
bash
git clone https://github.com/yourusername/employee-management.git
cd employee-management
2. Configure Database
This project uses SQL Server.
Update the connection string in appsettings.json:
json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=EmployeeDb;User Id=sa;Password=YourPassword123;"
}
    • Replace User Id and Password with your SQL Server credentials.
    • If using Docker for SQL Server, ensure the container is running:
      bash
      docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourPassword123" -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest
3. Apply Migrations
Run the following to create database tables:
bash
dotnet ef database update
This will create:
    • Employees table
    • Departments table
4. Run the Application
bash
dotnet run
The app will be available at:
👉 http://localhost:5000

📂 Project Structure
pgsql
EmployeeManagement/
│── Controllers/
│   ├── EmployeesController.cs
│   ├── DepartmentsController.cs
│
│── Models/
│   ├── Employee.cs
│   ├── Department.cs
│   ├── DepartmentViewModel.cs
│
│── Middleware/
│   ├── ExceptionMiddleware.cs
│
│── Data/
│   ├── ApplicationDbContext.cs
│   ├── Migrations/
│
│── Logs/
│   ├── log-2025-08-16.json
│
│── Views/
│   ├── Employees/
│   ├── Departments/
│
│── appsettings.json
│── Program.cs
│── Startup.cs

📑 Global Exception Handling
Unhandled exceptions are logged to JSON files in /Logs.
Example log entry:
json
{
  "Timestamp": "2025-08-16T12:34:56Z",
  "Message": "Object reference not set to an instance of an object.",
  "StackTrace": "at EmployeeManagement.Controllers..."
}

🧪 Testing the Exception Middleware
    1. Start the application.
    2. Visit an invalid route, e.g. http://localhost:5000/doesnotexist.
    3. Check the /Logs folder — a new log file should contain the exception.

✅ Requirements Met
✔ Server-side validation
✔ Global exception middleware with JSON logging
✔ SQL DB with EF Core migrations
✔ CRUD for Employees & Departments
✔ Search functionality with * to show all

