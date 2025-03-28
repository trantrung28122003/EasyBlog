using CommentApi.Infrastructure.DependencyInjection;
using CommentApi.Application.DependencyInjection;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructureService(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddApplicationService(builder.Configuration);
var app = builder.Build();


app.UseInfrastructurePolicy();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.Run();
