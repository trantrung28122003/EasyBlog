using UploadApi.Infrastructure.DependencyInjection;
using UploadApi.Application.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.addInfrastructureService(builder.Configuration);
builder.Services.AddApplicationService(builder.Configuration);
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

app.UseInfrastructurePolicy();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.Services.SeedDataAsync();
app.Run();
