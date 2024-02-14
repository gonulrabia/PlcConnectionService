using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlcConnectionService;
using PlcConnectionService.DAL.Modules;
using PlcConnectionService.DATA;
using PlcConnectionService.Entities.Entities;
using System;
using Serilog;
using Microsoft.VisualBasic;
 

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
                    dbContext.Database.Migrate(); // Veritabanýný günceller                   
                }
                catch (Exception ex)
                {
                    // Hata oluþursa burada iþleyin
                    Console.WriteLine("Veritabaný hatasý: " + ex.Message);
                }

                Log.Logger = new LoggerConfiguration().Filter.ByExcluding(logEvent =>
        logEvent.Level == Serilog.Events.LogEventLevel.Information)
                    .WriteTo.Debug(Serilog.Events.LogEventLevel.Information)
                .WriteTo.File("C:\\Geotek\\Logs\\PlcConnectionService\\logs.txt",rollingInterval:RollingInterval.Day).CreateLogger();

            }

            host.Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).UseWindowsService()
            .UseSerilog()

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
