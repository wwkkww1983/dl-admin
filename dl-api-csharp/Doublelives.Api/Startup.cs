﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Doublelives.Api.AutoMapper;
using Doublelives.Api.Middlewares;
using Doublelives.Core;
using Doublelives.Core.Filters;
using Doublelives.Shared.ConfigModels;
using Doublelives.Shared.Constants;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;

namespace Doublelives.Api
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
            DIConfig.Configure(services, Configuration);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpContextAccessor();

            services.AddSwaggerGen(c =>
            {
                var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "swagger.xml");
                c.IncludeXmlComments(filePath);
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "doublelives admin", Version = "v1.0" });

                // 主页右上角显示Anthorize的图标
                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Name = ApiHeaders.TOKEN,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "请输入token"
                });
                // 使用上面的规则，保护api(api上会显示Anthorize图标)
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = JwtBearerDefaults.AuthenticationScheme,
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            services.Configure<TencentCosOptions>(Configuration.GetSection("tencentCos"));
            services.Configure<JwtOptions>(Configuration.GetSection("jwt"));
            services.Configure<CacheOptions>(Configuration.GetSection("cache"));

            services.AddAutoMapper(c =>
            {
                c.AddProfile(new ViewModelProfile());
            }, typeof(Startup));

            services.AddCors(options => options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyMethod()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin()
                .SetPreflightMaxAge(TimeSpan.FromDays(1));
            }));

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var key = Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]);
                    options.TokenValidationParameters.ValidateIssuer = true;
                    options.TokenValidationParameters.ValidateAudience = true;
                    options.TokenValidationParameters.ValidateIssuerSigningKey = true;
                    options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(key);
                    options.TokenValidationParameters.ValidAudience = Configuration["Jwt:Audience"];
                    options.TokenValidationParameters.ValidIssuer = Configuration["Jwt:Issuer"];
                    options.TokenValidationParameters.NameClaimType = JwtClaimTypes.Name;
                    options.TokenValidationParameters.RoleClaimType = JwtClaimTypes.Role;
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = it =>
                        {
                            if (!string.IsNullOrEmpty(it.HttpContext.Request.Headers[ApiHeaders.TOKEN]))
                            {
                                it.Token = it.HttpContext.Request.Headers[ApiHeaders.TOKEN];
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            services
                .AddControllers(options =>
                {
                    options.Filters.Add(typeof(GlobalExceptionFilter));
                })
                .AddNewtonsoftJson(options =>
                {
                    // 配置string转enum
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    // 避免循环依赖
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    // 配置序列化时时间格式
                    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseCors("AllowAll");

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<WorkContextMiddleware>();

            loggerFactory
                .AddSentry(options =>
                {
                    options.Dsn = Configuration["sentryClientKey"];
                    options.Environment = env.EnvironmentName;
                    options.MinimumEventLevel = LogLevel.Error;
                    options.Debug = env.IsDevelopment();
                });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "doublelives admin");
                c.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}