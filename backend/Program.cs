using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NoticiasRSSComFiltroTermo.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Services.AddHttpClient<RssAggregatorService>();

var app = builder.Build();

app.UseCors(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseStaticFiles();
app.MapFallbackToFile("index.html");

app.MapGet("/api/noticias", async (
    HttpContext http,
    RssAggregatorService service) =>
{
    var termo = http.Request.Query["termo"].ToString();
    var dataInicio = http.Request.Query["dataInicio"].ToString();
    var dataFim = http.Request.Query["dataFim"].ToString();

    var noticias = await service.BuscarNoticiasAsync(termo, dataInicio, dataFim);
    return Results.Json(noticias);
});

app.Run();
