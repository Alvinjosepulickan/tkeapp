using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TKE.SC.BFF.BusinessProcess.Services;
using TKE.SC.BFF.DataAccess.Services;
using TKE.SC.BFF.Helper;
using TKE.SC.BFF.Infrastructure.Swagger;
using TKE.SC.Common.Model;
using TKE.SC.Common.Caching;
using Hangfire.MemoryStorage;
using System.Net.Http;

namespace TKE.SC.BFF
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
            ).AddJwtBearer(o =>
            {
                o.Authority = Configuration["ParamSettings:Identity:Issuer"];
                o.Audience = Configuration["ParamSettings:Identity:Audience"];
                o.RequireHttpsMetadata = false;
                if (Convert.ToBoolean(Configuration["ParamSettings:Identity:LocalEnv"]))
                    o.BackchannelHttpHandler = new HttpClientHandler { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
            });
            // Add framework services.  
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiReader", policy => policy.RequireClaim("scope", "AppGateway"));
            });

            #region Api Versioning  
            services.AddApiVersioning(config =>
            {
                // Specify the default API Version as 1.0
                config.DefaultApiVersion = new ApiVersion(1, 0);

                // If the client hasn't specified the API version in the request, use the default API version number 
                config.AssumeDefaultVersionWhenUnspecified = true;

                // Advertise the API versions supported for the particular endpoint
                config.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";

                options.SubstituteApiVersionInUrl = true;
            });
            #endregion

            #region Swagger Service Configuration

            services.AddSwaggerGen(options =>
            {
                options.OperationFilter<SwaggerDefaultValues>();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{Configuration["ParamSettings:Identity:Issuer"]}connect/authorize"),
                            TokenUrl = new Uri($"{Configuration["ParamSettings:Identity:Issuer"]}connect/token"),
                            Scopes = new Dictionary<string, string>() { { "openid", "" }, { "profile", "" }, { "AppGateway", "" } },
                        }
                    },

                    OpenIdConnectUrl = new Uri($"{Configuration["ParamSettings:Identity:Issuer"]}.well-known/openid-configuration")
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                 {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });

            services.Configure<RouteOptions>(options => { options.LowercaseUrls = true; });
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            #endregion

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver =
                  new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.NullValueHandling =
                   NullValueHandling.Ignore;
                options.SerializerSettings.ReferenceLoopHandling =
                 ReferenceLoopHandling.Serialize;

            });

            var serializer = new JsonSerializer
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            services.AddLocalization(options => { options.ResourcesPath = "Resources"; });



            var serviceProvider = services.BuildServiceProvider();

            services.Configure<ParamSettings>(Configuration.GetSection(Constant.PARAMSETTINGS));

            services.AddCacheManager();
            services.AddBusinessServices();
            services.AddDataAccessServices();

            services.AddDistributedMemoryCache()
             .AddSession()
             .AddSingleton(serializer);

            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.EnableForHttps = true;
            });
            services.AddHttpClient();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddDistributedRedisCache(option =>
            {
                option.Configuration = Configuration.GetSection(Constant.PARAMSETTINGS)[Constant.REDISURI];
                option.InstanceName = Configuration.GetSection(Constant.PARAMSETTINGS)[Constant.INSTANCENAME];
            });

            //services.AddControllers().AddNewtonsoftJson(options =>
            //options.SerializerSettings.ContractResolver =
            //new CamelCasePropertyNamesContractResolver());


            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            // Add Hangfire services.
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetSection(Constant.PARAMSETTINGS)[Constant.TKEDBConnection]));

            var options = new SqlServerStorageOptions
            {
                QueuePollInterval = TimeSpan.FromSeconds(1)
            };
            services.AddHangfire(config =>
            {
                config.UseMemoryStorage(new MemoryStorageOptions { FetchNextJobTimeout = TimeSpan.FromHours(2) });
            });
            services.AddMvc();
            services.BuildServiceProvider();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IApiVersionDescriptionProvider provider)
        {
            loggerFactory.AddSerilog(Log.Logger);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var option = new RewriteOptions();
            app.UseRewriter(option);
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors(x => x
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .SetIsOriginAllowed(origin => true)
                       );
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();

            //swagger
            app.UseOpenApi();
            app.UseSwagger(options => { options.RouteTemplate = "api-docs/{documentName}/docs.json"; });
            app.UseSwaggerUI(options =>
            {
                options.InjectStylesheet("/swagger-ui/custom.css");
                options.InjectJavascript("/swagger-ui/custom.js");
                options.RoutePrefix = "api-docs";
                foreach (var description in provider.ApiVersionDescriptions)
                    options.SwaggerEndpoint($"/api-docs/{description.GroupName}/docs.json", description.GroupName.ToLowerInvariant());
            });

            app.UseResponseCompression();

            //HangFire
            app.UseHangfireDashboard();
            var options = new BackgroundJobServerOptions
            {
                WorkerCount = 1    //Hangfire's default worker count is 20, which opens 20 connections simultaneously.
                                   // For this we are overriding the default value.
            };
            app.UseHangfireServer(options);

            app.UseDeveloperExceptionPage();



            app.UseMiddleware(typeof(ExceptionHandlingMiddleware));
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}