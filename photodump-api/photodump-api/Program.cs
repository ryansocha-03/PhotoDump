using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using photodump_api.Features.GetGuestList;
using photodump_api.Features.UploadMedia;
using photodump_api.Infrastructure.Db;
using photodump_api.Infrastructure.Db.Data;
using photodump_api.Infrastructure.Db.Repositories;
using photodump_api.Shared.Classes;
using photodump_api.Shared.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlite("Data Source=local.db"));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add repositories
builder.Services.AddScoped<IGuestRepository, GuestRepository>();
builder.Services.AddScoped<IMediaRepository, MediaRepository>();

// Add handlers/services
builder.Services.AddSingleton<IMediaStorage, MinIoService>();
builder.Services.AddScoped<GetGuestListHandler>();
builder.Services.AddScoped<UploadMediaHandler>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    SeedData.Initialize(db);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler("/error");

app.Map("/error", async (HttpContext context) =>
{
    var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;
    if (exception is BadHttpRequestException)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsync(exception.Message);
    }
    else if (exception is IOException)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsync(exception.Message);
    }
});

// Add endpoints
app.AddGuestListEndpoint();
app.AddUploadMediaEndpoint();

app.Run();