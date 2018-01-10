using System;
using System.Net;
using System.Threading;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SupportWheelOfFate.Commands.Schedule;
using SupportWheelOfFate.Database;
using SupportWheelOfFate.Domain.Contracts;
using SupportWheelOfFate.Queries.Schedule;
using SupportWheelOfFate.Services;

namespace SupportWheelOfFate.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<SupportWheelContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
            services.AddMediatR(typeof(GenerateScheduleCommand), typeof(ScheduleQuery));
            services.AddMvc();

            services.AddScoped<IScheduleGenerator, ScheduleGenerator>();
            services.AddScoped<CronJobs, CronJobs>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors(
                    builder =>
                        builder.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin());
            }

            app.Use(async (context, next) =>
            {
                await next.Invoke();
                if (context.Response.StatusCode == (int)HttpStatusCode.NotFound
                    && !context.Request.Path.StartsWithSegments("/api/"))
                {
                    //makes F5 working on any route
                    context.Response.StatusCode = 200;
                    context.Request.Path = "/";
                    await next.Invoke();
                }
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseMvc();
            ConfigureSchedule(app);
        }

        private void ConfigureSchedule(IApplicationBuilder app)
        {
            app.UseHangfireServer();
            app.UseHangfireDashboard();

            RecurringJob.AddOrUpdate<CronJobs>(
                service => service.GenerateSchedule(),
                Cron.Weekly(DayOfWeek.Sunday));
        }
    }
}
