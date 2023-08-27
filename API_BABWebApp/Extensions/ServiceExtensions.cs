using LoggerService;
using Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.IO;
using System.Text;
using Contracts;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using Entities.Models;
using Entities;
using Entities.Helpers;
using Entities.DTOs.Clocking;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Cors;

namespace API_BABWebApp.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: "CorsPolicy",
                    policy => policy.WithOrigins("https://gentle-tree-05305200f.3.azurestaticapps.net")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithExposedHeaders("X-Pagination"));
            });
        }
        public static void ConfigureIISIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {
            });
        }
        public static void ConfigureJSONSerializer(this IServiceCollection services)
        {
            services.AddControllersWithViews().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
                    .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

        }
        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
        }
        public static void ConfigureMySqlContext(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config["ConnectionStrings:BABWebAppConnection"];
            services.AddDbContext<RepositoryContext>(o => {
                o.UseMySql(connectionString, MySqlServerVersion.LatestSupportedServerVersion);
                o.EnableSensitiveDataLogging();
            });
        }
        public static void ConfigureRepositoryWrapper(this IServiceCollection services)
        {
            //Configure SortHelper for each entity
            services.AddScoped<ISortHelper<TransportRequest>, SortHelper<TransportRequest>>();
            services.AddScoped<ISortHelper<Volunteer>, SortHelper<Volunteer>>();
            services.AddScoped<ISortHelper<Company>, SortHelper<Company>>();
            services.AddScoped<ISortHelper<MarketSeller>, SortHelper<MarketSeller>>();
            services.AddScoped<ISortHelper<Event>, SortHelper<Event>>();
            services.AddScoped<ISortHelper<BeneficiaryFamily>, SortHelper<BeneficiaryFamily>>();
            services.AddScoped<ISortHelper<BeneficiaryOrganization>, SortHelper<BeneficiaryOrganization>>();
            services.AddScoped<ISortHelper<Trip>, SortHelper<Trip>>();
            services.AddScoped<ISortHelper<Clocking>, SortHelper<Clocking>>();
            services.AddScoped<ISortHelper<VolunteerWorkStatisticsDTO>, SortHelper<VolunteerWorkStatisticsDTO>>();

            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IEventVolunteerRepository, EventVolunteerRepository>();
            services.AddScoped<IVolunteerRepository, VolunteerRepository>();
            services.AddScoped<IBeneficiaryFamilyMemberRepository, BeneficiaryFamilyMemberRepository>();
            services.AddScoped<IClockingRepository, ClockingRepository>();

            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        }
        public static void ConfigureSwaggerGenerator(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BAB API", Version = "v1" });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.OperationFilter<AddHeaderParameters>();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header. \r\n\r\n Enter the token in the text input below."
                });
            });
        }
        public static void ConfigureJWTAuthenticator(this IServiceCollection services, IConfiguration configuration)
        {
            var secretKey = configuration["SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new Exception("SecretKey is not set in the configuration");
            }

            var validIssuer = configuration["ValidIssuer"];
            var validAudience = configuration["ValidAudience"];

            services.AddAuthentication(opt => {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true, // on production make it true
                        ValidateAudience = true, // on production make it true
                        ValidateLifetime = false,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = validIssuer,
                        ValidAudience = validAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                        ClockSkew = TimeSpan.Zero
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine($"Authentication failed: {context.Exception}");
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
        }
    }
}
