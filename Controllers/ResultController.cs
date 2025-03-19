using Microsoft.AspNetCore.Mvc;
using YouBrowse.Models;
using YouBrowse.Services;


namespace YouBrowse.Controllers
{
    public class ResultController : Controller
    {
        private readonly YouTubeApiService _youTubeApiService;

        public ResultController(YouTubeApiService youTubeApiService)
        {
            _youTubeApiService = youTubeApiService;
        }

        // GET: /Result/Index
        [HttpGet]
        public IActionResult Index(string query, string uploadDate, string videoDuration, string pageToken = "", int page = 1)
        {
            try
            {
                var result = _youTubeApiService.SearchVideosAsync(query, uploadDate, videoDuration, pageToken).Result;

                var model = new YouTubeSearchResult
                {
                    Videos = result.Videos,
                    NextPageToken = result.NextPageToken,
                    PrevPageToken = result.PrevPageToken,
                    CurrentPage = page
                };

                return View(model);  // Return the results to the view
            }
            catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                // Handle quotaExceeded error
                if (ex.Error != null && ex.Error.Errors.Any(e => e.Reason == "quotaExceeded"))
                {
                    ViewBag.ErrorMessage = "YouTube API rate limit exceeded. Please try again later.";
                }
                else
                {
                    ViewBag.ErrorMessage = "An error occurred while fetching videos. Please try again.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred: " + ex.Message;
            }

            return View(new YouTubeSearchResult());  // Return empty model with error message
        }
    }
}
