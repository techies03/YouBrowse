using Microsoft.AspNetCore.Mvc;
using YouBrowse.Models;
using YouBrowse.Services;
public class SearchController : Controller
{
    // GET: /Search/Index
    public IActionResult Index()
    {
        return View();
    }

    // GET: /Search/SearchResults
    [HttpGet]
    public IActionResult SearchResults(string query, string uploadDate, string videoDuration)
    {
        // Validate the inputs if necessary
        if (string.IsNullOrEmpty(query))
        {
            ViewBag.ErrorMessage = "Search query cannot be empty.";
            return View("Index");
        }

        // Redirect to the ResultController with the search query and filters
        return RedirectToAction("Index", "Result", new { query, uploadDate, videoDuration });
    }
}

