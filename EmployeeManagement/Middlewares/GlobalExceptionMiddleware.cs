using System.Text.Json;

namespace EmployeeManagement.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        // _next is a delegate representing the next middleware in the pipeline. You must call it to continue processing the request.
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _env;

        // ASP.NET Core automatically injects the next middleware when the pipeline runs.
        public GlobalExceptionMiddleware(RequestDelegate next, IHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Continue down the pipeline
                // If no exception → request continues normally.
                await _next(context);
            }
            catch (Exception ex)
            {
                // Handle exception
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Ensure Logs folder exists
            var logFolder = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            Directory.CreateDirectory(logFolder);

            // Log file name
            var logFile = Path.Combine(logFolder, $"log-{DateTime.Now:yyyy-MM-dd}.json");

            // Log entry as JSON
            var logEntry = new
            {
                Timestamp = DateTime.Now,
                exception.Message,
                exception.StackTrace,
                context.Request.Path,
                _env.EnvironmentName
            };

            var logJson = JsonSerializer.Serialize(logEntry) + Environment.NewLine;
            await File.AppendAllTextAsync(logFile, logJson);
            // Optional: return a friendly error page
            if (_env.IsDevelopment())
            {
                // In dev, show detailed HTML
                context.Response.StatusCode = 500;
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(@"
                    <html>
                        <body>
                            <h1>Something went wrong! (Development)</h1>
                            <p>Our team has been notified.</p>
                            <p>Exception message: " + exception.Message + @"</p>
                            <a href='/'>Go back to Home</a>
                        </body>
                    </html>");
            }
            else
            {
                context.Response.Clear();
                context.Response.StatusCode = 500;
                context.Response.Redirect("/Home/Error");
            }
        }
    }
}
