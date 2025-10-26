using Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var dbConnectionString= $"Host={Environment.GetEnvironmentVariable("DB_HOST")};Database={Environment.GetEnvironmentVariable("DB_NAME")};Username={Environment.GetEnvironmentVariable("DB_USER")};Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}";

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(dbConnectionString)
);

var host = builder.Build();

host.Run();