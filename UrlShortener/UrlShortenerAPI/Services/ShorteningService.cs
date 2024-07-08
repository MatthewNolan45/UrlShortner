using Microsoft.AspNetCore.WebUtilities;
using UrlShortenerAPI.Entities;
using UrlShortenerAPI.Models;

namespace UrlShortenerAPI.Services
{
    public class ShorteningService
    {
        private int urlCodeLength = 8;

        private readonly UrlDbContext _context;

        public ShorteningService(UrlDbContext context)
        {
            _context = context;
        }
        public IResult ShortenUrl(ShortUrlRequest request, HttpContext httpConext)
        {
            Uri uriResult;
            // Check to make sure the url is valid 
            if (Uri.TryCreate(request.URL, UriKind.Absolute, out uriResult))
            {
                // create guid and url code from the guid 
                Guid newUrlId = Guid.NewGuid();
                string newUrlCode = GenerateUrlCode(newUrlId);

                ShortUrl newShortUrl = new ShortUrl {
                    id = newUrlId,
                    originalurl= request.URL,
                    urlcode = newUrlCode,
                    shortenedurl = $"{httpConext.Request.Scheme}://{httpConext.Request.Host}/api/{newUrlCode}",
                    urlcreatedtime = DateTime.UtcNow
                };

                // add to database 
                _context.shorturls.Add(newShortUrl);
                _context.SaveChanges();
                return Results.Ok(newShortUrl.shortenedurl);
            }
            else
            {
                // if url is not valid return bad request to user 
                return Results.BadRequest("Url is not valid");
            }
        }

        public IResult getAllUrls()
        {
            // return a list of all urls
            return Results.Ok(_context.shorturls.ToList());
        }



        public IResult DeteleUrl(string urlCode)
        {

            // check the the url exists
            ShortUrl urlToDelte = _context.shorturls.FirstOrDefault(u => u.urlcode == urlCode);
            if(urlToDelte == null)
            {
                return Results.BadRequest();
            }

            // remove it 
            _context.shorturls.Remove(urlToDelte);
            _context.SaveChanges();

            return Results.Ok("URL Deleted");
        }

        public IResult RedirectShortUrl(string UrlCode)
        {
            // find url by urlCode
            ShortUrl shortUrl = _context.shorturls.FirstOrDefault(x => x.urlcode == UrlCode);
            
            // if url not in db
            if(shortUrl == null)
            {
                return Results.NotFound();
            }

            // Redirect user back to the original url
            return Results.Redirect(shortUrl.originalurl);
        }

        private string GenerateUrlCode(Guid UrlGuid)
        {
            // code will be the first 8 characters of a guid 
            string urlCode = UrlGuid.ToString("n").Substring(0, urlCodeLength);
            
            // check that the value is unique
            
            return urlCode;
        }

    }
}
