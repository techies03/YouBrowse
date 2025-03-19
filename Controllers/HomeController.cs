using Microsoft.AspNetCore.Mvc;
using YouBrowse.Models;
using YouBrowse.Services;
public class HomeController : Controller
{
    // GET: /Home/Index
    public IActionResult Index()
    {
        return View();  // This will return Views/Home/Index.cshtml
    }
}
