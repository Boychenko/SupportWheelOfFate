using System;
using MediatR;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SupportWheelOfFate.Commands.Schedule;
using SupportWheelOfFate.Database;

namespace SupportWheelOfFate.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<SupportWheelContext>();
                    var env = services.GetRequiredService<IHostingEnvironment>();
                    if (!env.IsDevelopment())
                    {
                        context.Database.Migrate();
                    }

                    var mediator = services.GetRequiredService<IMediator>();
                    mediator.Send(new GenerateScheduleCommand()).Wait();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
