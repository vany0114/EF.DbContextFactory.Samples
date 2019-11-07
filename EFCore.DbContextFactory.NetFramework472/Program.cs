using EFCore.DbContextFactory.Examples.Data.Persistence;
using EFCore.DbContextFactory.Examples.Data.Repository;
using EFCore.DbContextFactory.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EFCore.DbContextFactory.NetFramework472
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .Run();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false);
                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // Without EFCore.DbContextFactory
                    //services.AddScoped<IOrderRepository, OrderRepository>();
                    //services.AddDbContext<OrderContext>(builder =>
                    //    builder.UseInMemoryDatabase("OrdersExample"));

                    // With EFCore.DbContextFactory
                    services.AddScoped<IOrderRepository, OrderRepositoryWithFactory>();
                    services.AddDbContextFactory<OrderContext>(builder => builder
                        .UseInMemoryDatabase("OrdersExample"));

                    services.AddScoped<IHostedService, OrderManagerService>();

                })
                .ConfigureLogging((hostingContext, logging) => {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                })
                .UseStartup<Startup>();
    }
}
