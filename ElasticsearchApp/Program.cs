
using ElasticsearchApp.Model;
using ElasticsearchApp.MovieService;
using Microsoft.Extensions.Configuration;
using Nest;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSingleton(provider =>
{
    var settings = new ConnectionSettings(new Uri("https://localhost:9200"))
        .DefaultIndex("movies")
        .CertificateFingerprint("81436ca0b53045454c21d6b89f4511bbad595e6b8d362a64874f2a581a6d20134")
        .BasicAuthentication("elastic", "aYyr8ya9=Selo-YiRvP_c"); 

    return new ElasticClient(settings);
});

builder.Services.AddScoped<IMovieService, MovieService>();

var app = builder.Build();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
