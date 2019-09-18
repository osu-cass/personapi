using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using PersonApi.Models;
using System.Reflection;
using System.IO;
using PersonApi.Repositories;
using PersonApi.Configurations;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace PersonApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            // Initialize the logger. While the program is running, it creates a new log file every hour and continuously logs to it.
            // Logs can be found inside the project under the Logs folder. Only the 31 most recent log files are retained.
            Log.Logger = new LoggerConfiguration()            
            .WriteTo.File(Path.Combine(@"Logs\log-.txt"), rollingInterval: RollingInterval.Hour, rollOnFileSizeLimit: true)
            .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Register the database context with the dependency injection container
            services.AddDbContext<PersonContext>(opt =>
                opt.UseInMemoryDatabase("PersonList"));

            services.AddScoped<IRepository<Person, int>, PersonRepository>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddVersionedApiExplorer(
                options =>
                {
                        //The format of the version added to the route URL  
                        options.GroupNameFormat = "'v'VVV";
                        //Tells swagger to replace the version in the controller route  
                        options.SubstituteApiVersionInUrl = true;
                }); ;

            services.AddApiVersioning(options => options.ReportApiVersions = true);

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(options =>
            {
                // Resolve the temprary IApiVersionDescriptionProvider service  
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                // Add a swagger document for each discovered API version  
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, new Info()
                    {
                        Title = $"{this.GetType().Assembly.GetCustomAttribute<System.Reflection.AssemblyProductAttribute>().Product} {description.ApiVersion}",
                        Version = description.ApiVersion.ToString(),
                        Description = "Simple ASP.NET Core WebAPI example",
                    });
                }

                // Add a custom filter for setting the default values
                options.OperationFilter<SwaggerDefaultValues>();

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });

            // Add functionality to inject IOptions<T>
            services.AddOptions();

            // Add our Config object so it can be injected
            services.Configure<ProjectConfigurations>(Configuration.GetSection("ProjectConfigurations"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider provider)
        {
            // If the app is in a development environment, use a page that displays very detailed information about exceptions.
            // In a production environment, we don't want to expose this level of detail to the client.
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // Else, enable the Exception Handling Middleware.
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Enable middleware to use Serilog (logging service)
            app.UseSerilogRequestLogging();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                //Build a swagger endpoint for each discovered API version  
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });

            // Enforce redirecting from HTTP to HTTPS
            app.UseHttpsRedirection();

            // Set up routing
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
