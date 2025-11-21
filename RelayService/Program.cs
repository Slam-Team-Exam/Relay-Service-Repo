using System.Collections.Concurrent;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// optional: swagger for development
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Relay Service API",
        Version = "v1"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Relay Service API v1"));
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();


[ApiController]
public class Relay : ControllerBase
{

    [HttpPost("FoundMatch")]
    public async Task<IActionResult> FoundMatch([FromBody] List<string> input)
    {
        List<string> PlayerID = [input[0], input[1]];
        string MatchIP = input[2];

        return Ok();
    }

    [HttpPost("PlayerInfoUpdate")]
    public async Task<IActionResult> PlayerInfoUpdate()
    {
        return Ok();
    }

    [HttpPost("StoreUpdate")]
    public async Task<IActionResult> StoreUpdate()
    {
        return Ok();
    }
}
