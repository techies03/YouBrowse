namespace YouBrowse.Models
{
    public class YouTubeSearchResult
    {
        public required  List<YouTubeVideo> Videos { get; set; } = new();
        public string? NextPageToken { get; set; }
        public string? PrevPageToken { get; set; }
        public int CurrentPage { get; set; } = 1; // Default to page 1
    }

}
