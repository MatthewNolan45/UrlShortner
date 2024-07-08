using UrlShortenerAPI.Models;
using UrlShortenerAPI.Services;
using UrlShortenerAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using static System.Net.WebRequestMethods;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// create database context
builder.Services.AddDbContext<UrlDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));

// add url shortening service 
builder.Services.AddScoped<ShorteningService>();


var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();



// api endpoint to handle url shortening request
// takes in ShortUrlRequest class which contains URL 
app.MapPost("api/shortUrl", (ShortUrlRequest request, ShorteningService shorteningService, HttpContext http) => {


    if (request.URL == null)
    {
        return Results.BadRequest("URL is null");
    }

    return shorteningService.ShortenUrl(request, http);

});

app.MapPost("api/DeleteUrl",async (HttpContext http, ShorteningService shorteningService) =>{

    using var reader = new StreamReader(http.Request.Body);
    var body = await reader.ReadToEndAsync();
    var data = JsonSerializer.Deserialize<DeleteUrlRequest>(body);

    if (data == null || string.IsNullOrEmpty(data.urlCode))
    {
        return Results.BadRequest("Invalid request data");
    }

    return shorteningService.DeteleUrl(data.urlCode);

});


app.MapGet("api/getUrls", (ShorteningService shorteningService) => {

    return shorteningService.getAllUrls();

});

app.MapGet("api/{urlCode}", (string urlCode, ShorteningService shorteningService) => {
    return shorteningService.RedirectShortUrl(urlCode);
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
