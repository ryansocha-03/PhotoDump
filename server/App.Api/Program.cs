using App.Api.Services;
using ContentStore.MinIO.Utilities;
using Core.Configuration.Utilities;
using Identity;
using Infrastructure.EntityFramework.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add service configurations
builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddContentStoreConfiguration(builder.Configuration);

// Register actual services
builder.Services.AddDatabaseRepositories();
builder.Services.AddMinIOServices(builder.Configuration, builder.Environment.EnvironmentName);
builder.Services.AddIdentityServices();
builder.Services.AddApiServices();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();