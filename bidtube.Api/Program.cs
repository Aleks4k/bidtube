using bidtube.Infrastructure;
using bidtube.Application;
using bidtube.Api.Filters;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using bidtube.Api.Hubs;
using bidtube.Application.Common.Contracts;
using bidtube.Application.Hub.Services;
using bidtube.Api.Hubs.Contracts;
using bidtube.Api.Hubs.Services;
//dotnet run --project bidtube.Api
//dotnet ef migrations add Initial --project ./bidtube.Domain --startup-project ./bidtube.Api --verbose
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionFilterAttribute>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddFluentValidationAutoValidation(configuration =>
{
    configuration.OverrideDefaultResultFactoryWith<ValidationBehaviour>();
});
builder.Services.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddCors(options =>
{
    options.AddPolicy("bidtubeOrigin", policy =>
    {
        policy.WithOrigins("https://bidtube.pages.dev")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
        policy.WithOrigins("http://localho.st:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
builder.Services.AddScoped<IHubSender, HubSenderService>();
builder.Services.AddSingleton<IConnectedUsers, ConnectedUsersService>();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("bidtubeOrigin");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ApplicationHub>("hub");
app.Run();