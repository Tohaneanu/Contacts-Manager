using ContactsManager.Core.Domain.IdentityEntities;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using ServiceContracts;
using Services;

namespace Contacts_Manager
{
    public static class ConfigureServiceExtension
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            //adds controllers and views as services
            services.AddControllersWithViews();
            //Global filter
            //services.AddControllersWithViews(options =>
            //{
            //    options.Filters.Add<ResponseHeaderActionFilter>(); //without arguments
            //    var logger = services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();
            //    options.Filters.Add(new ResponseHeaderActionFilter(logger, "My-Key-From-Global", "My-Value-From-Global"));
            //});

            //add services into IoC container
            services.AddScoped<ICountriesRepository, CountriesRepository>();
            services.AddScoped<IPersonsRepository, PersonsRepository>();

            services.AddScoped<ICountriesService, CountriesService>();
            services.AddScoped<IPersonsAdderService, PersonsAdderService>();
            //services.AddScoped<IPersonsGetterService, PersonsGetterService>();
            services.AddScoped<PersonsGetterService, PersonsGetterService>();
            //services.AddScoped<IPersonsGetterService, PersonsGetterServiceChild>();
            services.AddScoped<IPersonsGetterService, PersonsGetterServiceWithFewExcelFields>();
            services.AddScoped<IPersonsUpdaterService, PersonsUpdaterService>();
            services.AddScoped<IPersonsDeleterService, PersonsDeleterService>();
            services.AddScoped<IPersonsSorterService, PersonsSorterService>();

            services.AddDbContext<ApplicationDbContext>(options =>
            { options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")); });

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
                .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();

            //http logging
            services.AddHttpLogging(options =>
            {
                options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
            });
            return services;
        }
    }
}
