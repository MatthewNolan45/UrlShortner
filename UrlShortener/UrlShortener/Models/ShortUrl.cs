namespace UrlShortener.Models
{
    public class ShortUrl
    {
        //all lower case, camel case caused relation error with postgresql 
        public Guid id { get; set; }

        public string shortenedurl { get; set; }

        public string originalurl { get; set; }

        public string urlcode { get; set; } = string.Empty;

        public DateTime urlcreatedtime { get; set; }

    }
}
