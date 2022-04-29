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
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RiskAssessment.DAO.Repositories;
using RiskAssessment.DAO;
using RiskAssessment.Entity.DbEntities;
using Microsoft.EntityFrameworkCore;
using RiskAssessment.Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using RiskAssessment.Entity;
using StackExchange.Redis;
using AutoMapper;
using RiskAssessment.Api.Common;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace RiskAssessment.Api
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
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;


            //db connection
            services.AddDbContext<AuditSysContext>(options =>
                    options.UseNpgsql(Configuration.GetConnectionString("AuditSysConnection"), x => x.MigrationsAssembly("RiskAssessment.Api"))
                );

            #region jwt token init
            services.Configure<JwtConfig>(Configuration.GetSection("JwtConfig"));
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(jwt =>
                {
                    var key = Encoding.ASCII.GetBytes(Configuration["JwtConfig:Secret"]);

                    jwt.SaveToken = true;
                    jwt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true, // this will validate the 3rd part of the jwt token using the secret that we added in the appsettings and verify we have generated the jwt token
                        IssuerSigningKey = new SymmetricSecurityKey(key), // Add the secret key to our Jwt encryption
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RequireExpirationTime = false,
                        ValidateLifetime = true
                    };
                });
            #endregion

            #region redis
            var multiplexer = ConnectionMultiplexer.Connect(Configuration.GetConnectionString("RedisConnection"));
            services.AddSingleton<IConnectionMultiplexer>(multiplexer);
            var iDb = multiplexer.GetDatabase();
            services.AddSingleton<IDatabase>(iDb);
            #endregion

            #region autp mapper
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ProfileMapping());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
            #endregion

            #region for upload
            services.Configure<FormOptions>(o => {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });
            #endregion

            //unitofword
            services.AddScoped<IUnitOfWork>(x => new UnitOfWork(x.GetRequiredService<AuditSysContext>()));

            //log4net
            services.AddSingleton<ILoggerManager, LoggerManager>();
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RiskAssessment.Api", Version = "v1" });
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "HoangNV - Put your JWT bearer token!",

                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

                //c.AddSecurityRequirement(new OpenApiSecurityRequirement
                //{
                //    { jwtSecurityScheme, Array.Empty<string>() }
                //});
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RiskAssessment.Api v1"));
            }

            var hosts = Configuration["AllowedHosts"].Split(';');

            app.UseCors(x => x
           .AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader());

            app.UseHttpsRedirection();

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
