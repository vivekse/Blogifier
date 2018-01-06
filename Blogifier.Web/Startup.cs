﻿using Blogifier.Core;
using Blogifier.Core.Middleware;
using Blogifier.Data;
using Blogifier.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using System;
using Microsoft.AspNetCore.HttpOverrides;

namespace Blogifier
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            Log.Logger = new LoggerConfiguration()
              .Enrich.FromLogContext()
              .WriteTo.RollingFile("Logs/blogifier-{Date}.txt", LogEventLevel.Warning)
              .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            System.Action<DbContextOptionsBuilder> databaseOptions = options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));

            services.AddDbContext<ApplicationDbContext>(databaseOptions);

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog(dispose: true));

            services.AddMvc()
            .ConfigureApplicationPartManager(p =>
            {
                foreach (var assembly in Core.Configuration.GetAssemblies())
                {
                    if (assembly.GetName().Name != "Blogifier.Web" && assembly.GetName().Name != "Blogifier.Core")
                    {
                        p.ApplicationParts.Add(new AssemblyPart(assembly));
                    }
                }
            });

            services.AddBlogifier(databaseOptions, Configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }

            app.UseStaticFiles();

            app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });

            app.UseAuthentication();

            app.UseETagger();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Blog}/{action=Index}/{id?}");
            });

            app.UseBlogifier(env);

            if (!Core.Common.ApplicationSettings.UseInMemoryDatabase && Core.Common.ApplicationSettings.InitializeDatabase)
            {
                try
                {
                    using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                    {
                        var db = scope.ServiceProvider.GetService<ApplicationDbContext>().Database;
                        db.EnsureCreated();
                        if (db.GetPendingMigrations() != null)
                        {
                            db.Migrate();
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
