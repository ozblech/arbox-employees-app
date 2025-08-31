using EmployeeManagement.Services;
using EmployeeManagement.Data;
using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Middlewares;

// Returns an instance of WebApplicationBuilder instance
var builder = WebApplication.CreateBuilder(args);

// Adds MVC controllers + Razor Views support to the app.
builder.Services.AddControllersWithViews();

// register EmployeeService as scoped 
// Whenever something asks for IEmployeeService, create an instance of EmployeeService and give it to them."
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
// Whenever something asks for IDepartmentService, create an instance of DepartmentService and give it to them."
builder.Services.AddScoped<IDepartmentService, DepartmentService>();

// SQL Server connection string
var connectionString = builder.Configuration.GetConnectionString("EmployeeDb");
// Whenever something needs ApplicationDbContext, create it with this SQL Server connection string.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
// ASP.NET Core Dependency Injection resolves the dependencies automatically

// Return an instance of WebApplication
var app = builder.Build();

// Add custom global exception middleware
// Every request first goes through this. If an exception occurs, it is logged to a file in Logs folder + shows error page..
app.UseMiddleware<GlobalExceptionMiddleware>();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated(); // creates DB + tables if they don't exist ⚠️ This bypasses migrations → used for quick demos, not production.
}

// Configure middleware
// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Home/Error");
// }

// Checks if the request is for a static file like CSS, JS, images. If so, it serves the file directly.
app.UseStaticFiles();

// Enables routing capabilities - Reads the URL and matches it to a route defined by app.MapControllerRoute
app.UseRouting();

// UseAuthorization is not needed as there is no authentication
// app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Starting the application
app.Run();
