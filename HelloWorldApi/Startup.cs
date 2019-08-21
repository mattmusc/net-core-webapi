using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace HelloWorldApi
{
    /// <summary>
    /// Initialization class, used to configure the web application
    /// </summary>
    public class Startup
    {
        /// <inheritdoc />
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Injected config object, reads properties from appsettings.json
        /// </summary>
        private IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options =>
                {
                    // Use camel case properties in the serializer and the spec (optional)
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    // Use string enums in the serializer and the spec (optional)
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                });

            #region swagger

            // Add Swagger DI service and configure documents

            // Adds the NSwag services
            services
                // Register a Swagger 2.0 document generator
                .AddSwaggerDocument(document =>
                {
                    document.DocumentName = "swagger";
                    // Post process the generated document
                    document.PostProcess = d => d.Info.Title = Configuration.GetValue<string>("AppName");
                });

            #endregion
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();

            #region swagger

            //// Add OpenAPI and Swagger middlewares to serve documents and web UIs

            // URLs: 
            // - http://localhost:65384/swagger/v1/swagger.json
            // - http://localhost:65384/swagger

            // Add Swagger 2.0 document serving middleware
            app.UseOpenApi(options =>
            {
                options.DocumentName = "swagger";
                options.Path = "/swagger/v1/swagger.json";
            });

            // Add web UIs to interact with the document
            app.UseSwaggerUi3(options =>
            {
                // Define web UI route
                options.Path = "/swagger";

                // Define OpenAPI/Swagger document route (defined with UseSwaggerWithApiExplorer)
                options.DocumentPath = "/swagger/v1/swagger.json";
            });

            #endregion
        }
    }
}