using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Filters;

public class ApiLoggingFilter : IActionFilter
{
    private readonly ILogger<ApiLoggingFilter> _logger;

    public ApiLoggingFilter(ILogger<ApiLoggingFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        _logger.LogInformation("### ApiLoggingFilter -> OnActionExecuting");
        _logger.LogInformation("########## ########## ########## ##########");
        _logger.LogInformation($"{DateTime.UtcNow.ToShortTimeString()}");
        _logger.LogInformation($"ModelState: {context.ModelState.IsValid}");
        _logger.LogInformation("########## ########## ########## ##########");
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        _logger.LogInformation("### ApiLoggingFilter -> OnActionExecuted");
        _logger.LogInformation("########## ########## ########## ##########");
        _logger.LogInformation($"{DateTime.UtcNow.ToShortTimeString()}");
        _logger.LogInformation("########## ########## ########## ##########");
    }
}
