using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Filters;
using API.Helper;
using API.Interfaces;
using API.Repositories;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection service, IConfiguration config)
        {

            service.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });
            service.AddCors();
             service.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            service.AddScoped<ITokenService,TokenService>();
            service.AddScoped<IUserRepository,UserRepository>();
            service.AddScoped<IPhotoService, PhotoService>();
           
            service.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            service.AddScoped<LogUserActivity>();


            return service;
        }
    }
}