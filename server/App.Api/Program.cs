using App.Api.Extensions;
using Asp.Versioning;
using Broker.RabbitMQ.Extensions;
using ContentStore.MinIO.Extensions;
using Infrastructure.EntityFramework.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add core services.
builder.Services.AddEfCoreDatabase(builder.Configuration);
builder.Services.AddMinIoContentStore(builder.Configuration);
builder.Services.AddRabbitMqBroker(builder.Configuration);

// Register actual services
builder.AddDataProtectionServices();
builder.Services.AddApiServices();

builder.Services.AddSessionAuth(builder.Configuration);
builder.Services.AddWorkerAuth(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
    config.ReportApiVersions = true;
    config.ApiVersionReader = new UrlSegmentApiVersionReader();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();