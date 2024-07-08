using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System;
using UrlShortener.Models;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace UrlShortener.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly HttpClient _httpClient; 

        public IndexModel(ILogger<IndexModel> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public List<ShortUrl> Urls = new List<ShortUrl>();

        public async Task OnGetAsync()
        {
            try
            {
                Urls = await _httpClient.GetFromJsonAsync<List<ShortUrl>>("http://host.docker.internal:5000/api/getUrls");
            }
            catch(Exception ex)
            {
                Urls = new List<ShortUrl>();
            }
            
        }

        public async Task<IActionResult> OnPostAddUrlAsync(string OriginalUrl)
        {
            try
            {

                var postData = new ShortUrlRequest { URL = OriginalUrl };

                //var json = JsonSerializer.Serialize(postData);
                //var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsJsonAsync("http://host.docker.internal:5000/api/shortUrl", postData);
                response.EnsureSuccessStatusCode();

            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine(ex.Message);
            }
            return RedirectToPage("Index");

        }

        public async Task<IActionResult> OnPostDeleteUrlAsync(string UrlCode)
        {
            try
            {

                var response = await _httpClient.PostAsJsonAsync("http://host.docker.internal:5000/api/DeleteUrl", new { UrlCode = UrlCode });
                response.EnsureSuccessStatusCode();

            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine(ex.Message);
            }

            return RedirectToPage("Index");
        }
    }



    public class ShortUrlRequest
    {
        public string URL { get; set; }
    }
}
