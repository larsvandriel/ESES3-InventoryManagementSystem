using InventoryManagementSystem.API.Filters;
using InventoryManagementSystem.Contracts;
using InventoryManagementSystem.Entities;
using InventoryManagementSystem.Entities.Helpers;
using InventoryManagementSystem.Entities.Models;
using InventoryManagementSystem.Repository;
using LoggingService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
        }

        public static void ConfigureIISIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {

            });
        }

        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
        }

        public static void ConfigureMsSqlContext(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config["mssqlconnection:connectionString"];
            services.AddDbContext<RepositoryContext>(options => options.UseSqlServer(connectionString));
        }

        public static void ConfigureRepositoryWrapper(this IServiceCollection services)
        {
            services.AddScoped<ISortHelper<Inventory>, SortHelper<Inventory>>();
            services.AddScoped<IDataShaper<Inventory>, DataShaper<Inventory>>();
            services.AddScoped<ISortHelper<InventoryItem>, SortHelper<InventoryItem>>();
            services.AddScoped<IDataShaper<InventoryItem>, DataShaper<InventoryItem>>();
            services.AddScoped<ISortHelper<InventoryItemNotificationThreshold>, SortHelper<InventoryItemNotificationThreshold>>();
            services.AddScoped<IDataShaper<InventoryItemNotificationThreshold>, DataShaper<InventoryItemNotificationThreshold>>();
            services.AddScoped<ISortHelper<Product>, SortHelper<Product>>();
            services.AddScoped<IDataShaper<Product>, DataShaper<Product>>();

            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        }

        public static void RegisterFilters(this IServiceCollection services)
        {
            services.AddScoped<ValidateMediaTypeAttribute>();
        }

        public static void AddCustomMediaTypes(this IServiceCollection services)
        {
            services.Configure<MvcOptions>(config =>
            {
                var newtonsoftJsonOutputFormatter = config.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();

                if (newtonsoftJsonOutputFormatter != null)
                {
                    newtonsoftJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.eses2.hateoas+json");
                }
                var xmlOutputFormatter = config.OutputFormatters.OfType<XmlDataContractSerializerOutputFormatter>()?.FirstOrDefault();

                if (xmlOutputFormatter != null)
                {
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.eses2.hateoas+xml");
                }
            });
        }
    }
}
