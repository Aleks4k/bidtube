using bidtube.Application.Auctions.Contracts;
using bidtube.Application.Bids.Contracts;
using bidtube.Application.Categories.Contracts;
using bidtube.Application.Common.Contracts;
using bidtube.Application.Notifications.Contracts;
using bidtube.Application.Users.Contracts;
using bidtube.Domain.Data;
using bidtube.Infrastructure.Repository;
using bidtube.Infrastructure.Services;
using bidtube.Infrastructure.Settings;
using CloudinaryDotNet;
using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace bidtube.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
            var connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION");
            services.AddDbContext<AppDbContext>(
                dbContextOptions => dbContextOptions
                    .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                    .LogTo(Console.WriteLine, LogLevel.Information)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
            );
            var cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
            cloudinary.Api.Secure = true;
            services.AddSingleton(cloudinary);
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            var googleSettings = new ValidationSettings() { Audience = new[] { Environment.GetEnvironmentVariable("GoogleSettings_ClientId") } };
            services.AddSingleton(googleSettings);
            services.AddScoped<IGoogleAuthService, GoogleAuthService>();
            var jwtSettings = new JwtSettings()
            {
                AccessTokenKey = Environment.GetEnvironmentVariable("JWT_AccessTokenKey")!,
                RefreshTokenKey = Environment.GetEnvironmentVariable("JWT_RefreshTokenKey")!,
                Issuer = Environment.GetEnvironmentVariable("JWT_Issuer")!,
                Audience = Environment.GetEnvironmentVariable("JWT_Audience")!,
                AccessTokenTTL = Convert.ToInt32(Environment.GetEnvironmentVariable("JWT_AccessTokenTTL")),
                RefreshTokenTTL = Convert.ToInt32(Environment.GetEnvironmentVariable("JWT_RefreshTokenTTL"))
            };
            services.AddSingleton(jwtSettings);
            services.AddScoped<IJwtService, JwtService>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.AccessTokenKey)),
                    ClockSkew = TimeSpan.Zero
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/hub")))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            services.AddAuthorization();
            services.AddScoped<IUser, UserRepository>();
            services.AddScoped<ICategory, CategoryRepository>();
            services.AddScoped<IAuction, AuctionRepository>();
            services.AddScoped<IBid, BidRepository>();
            services.AddScoped<IEventLog, EventLogRepository>();
            services.AddScoped<INotification, NotificationRepository>();
            services.AddHostedService<AuctionEndService>();
            return services;
        }
    }
}
