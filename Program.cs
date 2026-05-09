using robot_controller_api.Persistence;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using robot_controller_api.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<UserDataAccess>();


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly",
        policy => policy.RequireClaim(
            ClaimTypes.Role,
            "Admin"));

    options.AddPolicy("UserOnly",
        policy => policy.RequireClaim(
            ClaimTypes.Role,
            "Admin",
            "User"));
});
builder.Services
    .AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions,
        BasicAuthenticationHandler>(
            "BasicAuthentication",
            null);
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();