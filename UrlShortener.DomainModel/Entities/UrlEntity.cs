namespace UrlShortener.DomainModel.Entities
{
    public class Url
    {
        public int Id { get; set; }

        public string UserUrl { get; set; }

        public string ShortedUrl { get; set; }

        public int Clics { get; set; }
    }
}
