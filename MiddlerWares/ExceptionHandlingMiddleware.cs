using Learning_Backend.Databases;
using System.Text.Json;
using Learning_Backend.Models.LearningDatabaseModels;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IServiceProvider _serviceProvider;

    public ExceptionHandlingMiddleware(RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IServiceProvider serviceProvider)
    {
        _next = next;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        bool isArgumentException = exception is ArgumentException;
        if (!isArgumentException)
        {
            using (var scope = _serviceProvider.CreateAsyncScope())
            {
                var learningDatabase = scope.ServiceProvider.GetRequiredService<LearningDatabase>();
                await SaveErrorsToDataBase(learningDatabase, exception);
            }
        }
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        var result = JsonSerializer.Serialize(new { message = isArgumentException?exception.Message:"An unexpected error occurred." });
        await context.Response.WriteAsync(result);
    }

    private async Task SaveErrorsToDataBase(LearningDatabase learningDatabase, Exception exception)
    {
        LogsTable logsTable = new LogsTable
        {
            LogMessage = exception.Message,
            LogDate = DateTime.Now,
            LogType = "Error",
            LogSource = exception.Source
        };

        await learningDatabase.Logs.AddAsync(logsTable);
        await learningDatabase.SaveChangesAsync();
    }
}
