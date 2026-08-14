using WebApiApp.Application.Interfaces;
using WebApiApp.Application.Mappings;
using WebApiApp.Application.Services;
using WebApiApp.Domain.Interfaces;
using WebApiApp.Infrastructure.Context;
using WebApiApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace WebApiApp.CrossCutting.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
       options.UseSqlServer(
           configuration.GetConnectionString("DefaultConnection"),
           // Esta linha abaixo garante que as migrations fiquem na Infraestrutura:
           b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
       ));


            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
            services.AddScoped<IProdutoService, ProdutoService>();
            services.AddScoped<ICategoriaService, CategoriaService>();

            services.AddAutoMapper(cfg =>
            {
                cfg.AddMaps(typeof(DomainToDTOMappingProfile).Assembly);
            });

            return services;
        }
    }
}
