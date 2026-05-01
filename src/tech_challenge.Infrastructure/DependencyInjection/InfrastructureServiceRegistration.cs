using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using tech_challenge.Application.Interfaces.Repositories;
using tech_challenge.Application.Interfaces.Repositories.Base;
using tech_challenge.Infrastructure.Persistence.Context;
using tech_challenge.Infrastructure.Persistence.Repositories;
using tech_challenge.Infrastructure.Persistence.Repositories.Base;

namespace tech_challenge.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(config.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IVeiculoRepository, VeiculoRepository>();
            services.AddScoped<IOrdemServicoRepository, OrdemServicoRepository>();
            services.AddScoped<IServicoRepository, ServicoRepository>();
            services.AddScoped<IPecaInsumoRepository, PecaInsumoRepository>();

            return services;
        }
    }
}
