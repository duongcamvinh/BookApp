using Api.Data;
using Api.Entities;
using Api.Helpers;
using Api.Interfaces;
using Api.Middleware;
using Api.Service;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Api
{
    public class Startup
    {
        private readonly IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CloundinarySettings>(_config.GetSection("CloudinarySettings"));
            services.AddDbContext<DataContext>(op =>
            {
                op.UseSqlServer(_config.GetConnectionString("DefaultConnection"));
            });
            services.AddControllers();

            services.AddIdentityCore<AppUsers>(op =>
            {
                op.Password.RequireLowercase = false;
                op.Password.RequireDigit = false;
                op.Password.RequireNonAlphanumeric = false;
                op.Password.RequireUppercase = false;
                op.SignIn.RequireConfirmedEmail = false;//xac thuc email truoc khi dang nhap
            })
            .AddRoles<AppRole>()
            .AddEntityFrameworkStores<DataContext>()
            .AddSignInManager<SignInManager<AppUsers>>()
            .AddRoleManager<RoleManager<AppRole>>()
            .AddRoleValidator<RoleValidator<AppRole>>()
            .AddDefaultTokenProviders();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(op =>
            {
                op.TokenValidationParameters = new TokenValidationParameters
                {

                    ValidateIssuer = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenKey"])),
                    RequireExpirationTime = true,
                    

                };
            });
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.Configure<IpRateLimitOptions>(_config.GetSection("IpRateLimiting"));// cấu hình giới hạn lượt request ở appsettings
            services.Configure<IpRateLimitPolicies>(_config.GetSection("IpRateLimitPolicies")); // cấu hình policy 
            services.AddMemoryCache();// ghi nhận thông tin cache
            services.AddInMemoryRateLimiting();// bộ nhớ đếm số lưong 
            services.AddAuthorization(o =>
            {
               
                o.AddPolicy("EditUser", policy => policy.RequireRole("User", "Admin"));
            });
            services.AddCors();
            services.AddOptions();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IBookRespository, BookRespository>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseIpRateLimiting();
            //if (env.IsDevelopment())
            //{
            // //   app.UseDeveloperExceptionPage();

            //}
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000"));
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
