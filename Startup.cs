using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Brandix.DCAP.API.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;


namespace Brandix.DCAP.API
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
            services.AddCors(options =>
            {
                options.AddPolicy("CORS", corsPolicyBuilder => corsPolicyBuilder.AllowAnyOrigin()
                    // Apply CORS policy for any type of origin  
                    .AllowAnyMethod()
                    // Apply CORS policy for any type of http methods  
                    .AllowAnyHeader()
                    // Apply CORS policy for any headers  
                    .AllowCredentials());
                // Apply CORS policy for all users  
            });

            // Add DCAPDbContext services.
            services.AddDbContext<DCAPDbContext>(options => options.UseMySql(Configuration.GetConnectionString("DCAPDatabase")));
           
           
           //services.Configure<APIConfiguration>(Configuration.GetSection("IAPIConfiguration"));
            //services.AddDbContext<DCAPDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("FOSSConection")));
 
            //services.AddSingleton<IFOSSConn,FOSSConnection>();

            
            //services.AddScoped<IFOSSConn, IFOSSConn>();

            //services.AddTransient<IFOSSConn, IFOSSConn>();
            //services.AddTransient<IFOSSConn, IFOSSConn>();
   
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //Add our new middleware to the pipeline
            app.UseMiddleware<Brandix.DCAP.API.Controllers.RequestLogger>(); 

            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseCors("CORS");
            
        }
    }
}
