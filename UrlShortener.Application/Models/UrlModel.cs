namespace UrlShortener.Application.Models
{
    public class UrlModel
    {
        public int? Id { get; set; }

        public string UserUrl { get; set; }

        public string ShortedUrl { get; set; }

        public int Clics { get; set; }
    }
}
