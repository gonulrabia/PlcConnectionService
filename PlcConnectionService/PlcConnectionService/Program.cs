using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlcConnectionService;
using PlcConnectionService.DAL.Modules;
using PlcConnectionService.DATA;
using PlcConnectionService.Entities.Entities;
using System;
using Serilog;

namespace PlcConnectionService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var dbContext = services.GetRequiredService<BaseDbContext>();
                    dbContext.Database.Migrate(); // Veritaban�n� g�nceller                   
                }
                catch (Exception ex)
                {
                    // Hata olu�ursa burada i�leyin
                    Console.WriteLine("Veritaban� hatas�: " + ex.Message);
                }
            }

            host.Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddDbContext<BaseDbContext>(options =>
                    {
                        options.UseSqlServer(hostContext.Configuration.GetConnectionString("BaseDbConnection"));
                    });                   
                    services.AddHostedService<Worker>();
                });
    }

}
