using CommentApi.Application.DependencyInjection;
using CommentApi.Application.Hubs;
using CommentApi.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructureService(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddApplicationService(builder.Configuration);
builder.Services.AddSignalR().AddJsonProtocol();

var app = builder.Build();

app.UseInfrastructurePolicy();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<CommentHub>("/commentHub");

app.Run();
