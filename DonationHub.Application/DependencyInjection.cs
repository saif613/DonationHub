using System;
using System.Collections.Generic;
using System.Text;
using DonationHub.Application.Features.Implementaion;
using DonationHub.Application.Features.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DonationHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        return services;
    }

}
