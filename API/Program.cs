using API;
using Infrastructure.Seeds;
using Microsoft.AspNetCore.Hosting;

public class Program
{
    public static async Task Main(string[] args)
    {
        
        var host = CreateHostBuilder(args).Build();

        
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var dataSeeder = services.GetRequiredService<DataSeeder>();

                
                await dataSeeder.SeedAllAsync();
            }
            catch (Exception ex)
            {
                
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred during database seeding.");
            }
        }

        
        await host.RunAsync();
    }


    public static IHostBuilder CreateHostBuilder(string[] args) =>

        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<StartUp>();
            });
}