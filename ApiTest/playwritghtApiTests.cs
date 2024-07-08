using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;

[TestFixture]
public class PlaywrightApiTests
{
    private IAPIRequestContext _request;

    [OneTimeSetUp]
    public async Task Setup()
    {
        var playwright = await Playwright.CreateAsync();
        _request = await playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
        {
            BaseURL = "http://localhost:5000"
        });
    }

    // issue with running tests with docker
    [Test]
    public async Task Test_GetUrls()
    {
        var response = await _request.GetAsync("/api/getUrls");
        Assert.IsTrue(response.Ok);

        var responseBody = await response.JsonAsync();
        Console.WriteLine(JsonSerializer.Serialize(responseBody, new JsonSerializerOptions { WriteIndented = true }));
    }

    [Test]
    public async Task Test_ShortUrl()
    {
        var postData = new { URL = "https://stackoverflow.com/questions/62294539/how-to-do-a-post-request-with-playwright" };
        var response = await _request.PostAsync("/api/shortUrl", new()
        {
            DataObject = postData
        });

        Assert.IsTrue(response.Ok);

        var responseBody = await response.JsonAsync();
        Console.WriteLine(JsonSerializer.Serialize(responseBody, new JsonSerializerOptions { WriteIndented = true }));
    }

    [Test]
    public async Task Test_DeleteUrl()
    {
        // make short url first?
        var deleteData = new { urlCode = "" };
        var response = await _request.DeleteAsync("/api/DeleteUrl", new()
        {
            DataObject = deleteData
        });

        Assert.IsTrue(response.Ok);
    }

    [Test]
    public async Task Test_RedirectShortUrl()
    {
        var urlCode = "";
        var response = await _request.GetAsync($"/api/{urlCode}");

        Assert.IsTrue(response.Ok);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _request.DisposeAsync();
    }
}
