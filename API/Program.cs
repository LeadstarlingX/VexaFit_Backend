using API;
using Infrastructure.Seeds;
using Microsoft.AspNetCore.Hosting;

public class Program
{
    public static async Task Main(string[] args)
    {
        // 2. Build the host, but don't run it yet.
        var host = CreateHostBuilder(args).Build();

        // 3. This is the new block to run the seeder.
        // We create a scope to resolve our services correctly.
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                // Get the DataSeeder instance from the DI container.
                var dataSeeder = services.GetRequiredService<DataSeeder>();

                // Await the async seeding method.
                await dataSeeder.SeedAllAsync();
            }
            catch (Exception ex)
            {
                // Log any errors that occur during seeding.
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred during database seeding.");
            }
        }

        // 4. Finally, run the host to start the web application.
        await host.RunAsync();
    }


    public static IHostBuilder CreateHostBuilder(string[] args) =>

        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<StartUp>();
            });
}