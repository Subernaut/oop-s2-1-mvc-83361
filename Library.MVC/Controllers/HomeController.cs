using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Library.MVC.Models;
using Serilog;
using Microsoft.AspNetCore.Authorization;

namespace Library.MVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        _logger.LogInformation("Visited Home/Index page.");
        return View();
    }

    public IActionResult Privacy()
    {
        _logger.LogInformation("Visited Home/Privacy page.");
        return View();
    }

    /// <summary>
    /// Handles unhandled exceptions
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

        // Log the error context if available
        if (HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>() is { } exceptionFeature)
        {
            var ex = exceptionFeature.Error;
            Log.Error(ex, "Unhandled exception occurred. RequestId: {RequestId}", requestId);
        }

        var model = new ErrorViewModel
        {
            RequestId = requestId
        };

        return View(model); // Friendly error page view
    }
    public async Task<IActionResult> IndexError() //For error debugging pupose only 
    {
        return View();
    }

    /// <summary>
    /// Handles HTTP status codes (404, 403, etc.)
    /// </summary>
    [Route("Home/StatusCode")]
    public IActionResult StatusCode(int code)
    {
        var message = code switch
        {
            404 => "Sorry, the page you requested could not be found.",
            403 => "You do not have permission to access this page.",
            500 => "Oops! Something went wrong on the server.",
            _ => "An unexpected error occurred."
        };

        var model = new StatusCodeViewModel
        {
            StatusCode = code,
            Message = message
        };

        return View(model);
    }
}