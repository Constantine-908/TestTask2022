using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using WebApiWebLoad.Services;
using Microsoft.AspNetCore.Http;

namespace WebApiWebLoad
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
            services.AddSingleton<IUsageStatisticService, UsageStatisticService>();
            services.AddSingleton<IErrorLogService, ErrorLogService>();
            services.AddSingleton<IDateValidationService, DateValidationService>();
            
            services.AddScoped<IUspSelectCbrRatesControllerService, UspSelectCbrRatesControllerService>();
            services.AddScoped<IUspSelectCBRhistoryControllerService, UspSelectCBRhistoryControllerService>();
            services.AddScoped<IDAL>(sp => new DAL("srv-2022", "testDB"));
           
            services.AddControllers().AddXmlDataContractSerializerFormatters();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApiWebLoad", Version = "v1" });
            }); 
            services.AddHostedService<TimedHostedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {    
            app.UseSwagger();
            if (env.IsDevelopment())
            { 
                 app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiWebLoad v1"));
            }
            else
            {
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/load/swagger/v1/swagger.json", "WebApiWebLoad v1"));
            }

            app.UseExceptionHandler(error =>
            {//https://stackoverflow.com/questions/38630076/asp-net-core-web-api-exception-handling
                error.Run(async context =>
                {
                    var serverError = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;
                    if (serverError is not null)
                    {
                        IErrorLogService errorLogService = context.RequestServices.GetService<IErrorLogService>();
                        await errorLogService.AddErrorAsync(serverError, context.Request.GetDisplayUrl());
                        //add StackTrace for IsDevelopment
                        var response = new
                        {
                            error = serverError.Message
                        };
                        await context.Response.WriteAsJsonAsync(response);
                    }
                });
            });
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
