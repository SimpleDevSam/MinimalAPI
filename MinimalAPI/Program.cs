using Microsoft.EntityFrameworkCore;
using MinimalAPI_KeyCloack.Application.Validators;
using MinimalAPI_KeyCloack.DependencyInjection;
using MinimalAPI_KeyCloack.WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDataAccessServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddValidatorsServices();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.UseHttpsRedirection();
app.RegisterEndpointDefinitions();

app.Run();

public partial class Program { }