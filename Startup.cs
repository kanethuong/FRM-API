using System;
using System.Linq;
using System.Net;
using System.Text;
using kroniiapi.Hubs;
using kroniiapi.DB;
using kroniiapi.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Newtonsoft;
using kroniiapi.Requirements;
using Microsoft.AspNetCore.Http.Features;
using kroniiapi.Helper;
using Microsoft.AspNetCore.Authorization;
using kroniiapi.Services;
using kroniiapi.DTO.Email;
using OfficeOpenXml;
using kroniiapi.Helper.Upload;
using kroniiapi.Services.Calendar;

namespace kroniiapi
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // DB config
            string connectionString = Configuration.GetConnectionString("Postgre");
            services.AddDbContext<DataContext>(opt => opt.UseNpgsql(connectionString));
            services.AddScoped<DataContext, DataContext>();

            // Register the RedisCache service
            services.AddStackExchangeRedisExtensions<NewtonsoftSerializer>(Configuration.GetSection("Redis").Get<RedisConfiguration>());

            // Add Email service
            var emailConfig = Configuration.GetSection("Email").Get<EmailConfig>();
            services.AddScoped<IEmailService>(sp => new EmailService(emailConfig, _env));

            // Add validate JWT token middleware
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero, // Disable default 5 mins of Microsoft
                    // ValidIssuer = Configuration["Jwt:Issuer"],
                    // ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["Jwt:Key"]))
                };
            });

            // Add authorization to request
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Administrator", policy =>
                    policy.Requirements.Add(new AccessRequirement("administrator")));
                options.AddPolicy("Admin", policy =>
                    policy.Requirements.Add(new AccessRequirement("admin")));
                options.AddPolicy("Trainer", policy =>
                    policy.Requirements.Add(new AccessRequirement("trainer")));
                options.AddPolicy("Trainee", policy =>
                    policy.Requirements.Add(new AccessRequirement("trainee")));
                options.AddPolicy("Company", policy =>
                    policy.Requirements.Add(new AccessRequirement("company")));
            });

            // Increase input file size limit
            services.Configure<FormOptions>(f =>
            {
                f.ValueLengthLimit = int.MaxValue;
                f.MultipartBodyLengthLimit = int.MaxValue;
                f.MemoryBufferThreshold = int.MaxValue;
            });

            // Add IJwtGenerator to use in all project
            services.AddSingleton<IJwtGenerator>(new JwtGenerator(Configuration["Jwt:Key"]));

            // Add Refresh Token Service 
            services.AddSingleton<IRefreshToken, RefreshToken>();

            // Access restrict with IAuthorizationHandler
            services.AddSingleton<IAuthorizationHandler, AccessHandler>();

            // Config username and password Mega using
            services.AddSingleton<IMegaHelper>(provider =>
            {
                string username = Configuration["Email:MailAddress"];
                string password = Configuration["Email:MegaPassword"];
                return new MegaHelper(username, password);
            });

            // Map data from Model to DTO and back
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Custom Project Services register
            services.AddScoped<ICacheProvider, CacheProvider>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IAdministratorService, AdministratorService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<ITraineeService, TraineeService>();
            services.AddScoped<ITrainerService, TrainerService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IClassService, ClassService>();
            services.AddScoped<IModuleService, ModuleService>();
            services.AddScoped<IApplicationService, ApplicationService>();
            services.AddScoped<IMarkService, MarkService>();
            services.AddScoped<IExamService, ExamService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<ICalendarService, CalendarService>();
            services.AddScoped<ICertificateService, CertificateService>();
            // Setting JSON convert to camelCase in Object properties
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            // Add signalR
            services.AddSignalR();

            // Default service config
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            }).ConfigureApiBehaviorOptions(opt => // Custome Model State response object
            {
                opt.InvalidModelStateResponseFactory = actionContext =>
                    new BadRequestObjectResult(new ResponseDTO
                    {
                        Status = 400,
                        Message = "Model State is invalid",
                        Errors = actionContext.ModelState
                                .ToDictionary(
                                    kvp => kvp.Key,
                                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                                )
                    });
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "kroniiapi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "kroniiapi v1"));
            }

            // ReFormat exception message
            app.UseExceptionHandler(e => e.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerPathFeature>().Error;
                await context.Response.WriteAsJsonAsync(new ResponseDTO(500, exception.Message));
            }));

            app.UseHttpsRedirection();

            //Get front-end url from appsettings.json
            var frontEndDevUrl = Configuration["FrontEndDevUrl"];

            //Get front-end url from appsettings.json
            var frontEndUrl = Configuration["FrontEndUrl"];

            //CORS config for Front-end url
            app.UseCors(options => options.WithOrigins(frontEndDevUrl, frontEndUrl)
                                        .AllowAnyMethod()
                                        .AllowAnyHeader()
                                        .AllowCredentials());

            // ReFormat error message
            app.Use(async (context, next) =>
            {
                await next();

                if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(new ResponseDTO(401).ToString());
                }

                if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
                {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(new ResponseDTO(403).ToString());
                }
            });

            // Load index.html in wwwroot folder as index page
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                // Map to SignalR hub
                endpoints.MapHub<NotifyHub>("/hubs/notification");
            });
        }
    }
}
