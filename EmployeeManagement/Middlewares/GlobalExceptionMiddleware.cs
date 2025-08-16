using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace EmployeeManagement.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Continue down the pipeline
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
                Message = exception.Message,
                StackTrace = exception.StackTrace,
                Path = context.Request.Path
            };

            var logJson = JsonSerializer.Serialize(logEntry) + Environment.NewLine;
            await File.AppendAllTextAsync(logFile, logJson);

            // Optional: return a friendly error page
            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/html";
            await context.Response.WriteAsync(@"
                <html>
                    <body>
                        <h1>Something went wrong!</h1>
                        <p>Our team has been notified.</p>
                        <a href='/'>Go back to Home</a>
                    </body>
                </html>");
        }
    }
}
