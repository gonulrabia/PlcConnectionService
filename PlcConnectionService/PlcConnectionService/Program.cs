using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlcConnectionService;
using PlcConnectionService.DATA;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) =>
{
    // DbContext'i ekleyin
    services.AddDbContext<BaseDbContext>(options =>
        options.UseSqlServer(hostContext.Configuration.GetConnectionString("BaseDbConnection")));

    // Worker servisini ekleyin
    services.AddHostedService<Worker>();
});

await builder.RunConsoleAsync();