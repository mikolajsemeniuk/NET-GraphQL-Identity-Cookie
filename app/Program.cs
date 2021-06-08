using System.Threading.Tasks;
using app.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace app
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            if (args.Length == 1 && args[0] == "roles")
            {
                using var scope = host.Services.CreateScope();
                var services = scope.ServiceProvider;

                var userManager = services.GetRequiredService<UserManager<User>>();
                var roleManager = services.GetRequiredService<RoleManager<Role>>();

                foreach (var role in new[] { "member", "moderator", "admin" })
                    await roleManager.CreateAsync(new Role { Name = role });

                var admin = new User
                {
                    UserName = "admin",
                    Email = "admin@mock.com"
                };
                var moderator = new User
                {
                    UserName = "moderator",
                    Email = "moderator@mock.com"
                };

                await userManager.CreateAsync(admin, "P@ssw0rd");
                await userManager.CreateAsync(moderator, "P@ssw0rd");
                await userManager.AddToRolesAsync(admin, new[] { "admin", "moderator" });
                await userManager.AddToRolesAsync(moderator, new[] { "moderator" });
            }
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
