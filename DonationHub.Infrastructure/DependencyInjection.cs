using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Application.Interfaces.Services;
using DonationHub.Application.Interfaces.UnitOfWork;
using DonationHub.Infrastructure.Identity;
using DonationHub.Infrastructure.Implementaion.Repositories;
using DonationHub.Infrastructure.Implementaion.Services;
using DonationHub.Infrastructure.Implementaion.UnitOfWorK;
using DonationHub.Infrastructure.Options;
using DonationHub.Infrastructure.Persistence;
using DonationHub.Infrastructure.Persistence.Interceptors;
using DonationHub.Infrastructure.Persistence.Seeding;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection connection string was not found.");

            // HttpContextAccessor.
            services.AddHttpContextAccessor();

            //Current Service
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // Interceptors
            services.AddScoped<SoftDeleteInterceptor>();

            // DataBase
            services.AddDbContext<AppDbContext>((serviceProvider, options) =>
            {
                options.UseSqlServer(connectionString);

                options.AddInterceptors(serviceProvider.GetRequiredService<SoftDeleteInterceptor>());
            });


            // Options
            services.AddOptions<JwtOptions>()
                .Bind(configuration.GetSection(JwtOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<SeedAdminOptions>()
                .Bind(configuration.GetSection(SeedAdminOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<DonationOptions>()
                .Bind(configuration.GetSection(DonationOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            // Repositories and Unit of Work
            services.AddScoped<IUserRepository, UserRepository>(); 
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Services 
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            services.AddScoped<DbSeeder>();


            // TimeProvider
            services.AddSingleton(TimeProvider.System);

            // Identity
            services
                .AddIdentityCore<ApplicationUser>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 8;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;
                    options.User.RequireUniqueEmail = true;
                })
                .AddRoles<IdentityRole<Guid>>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();


            // Authentication
            services.AddAuthentication(o =>
              {
              o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
              o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
              })
                 .AddJwtBearer();

           services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
                .Configure< IOptions<JwtOptions> >((options, jwt) =>
                {
                    var jwtOptions = jwt.Value;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                    };
                });

            // Authorization
            services.AddAuthorization();
            

            return services;
        }
    }
}
