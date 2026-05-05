using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using tech_challenge.Application.Interfaces.Repositories.Base;
using tech_challenge.Application.Interfaces.Services;
using tech_challenge.Application.Services.Clientes;
using tech_challenge.Application.Services.OrdemServicos;
using tech_challenge.Application.Services.PecaInsumos;
using tech_challenge.Application.Services.Servicos;
using tech_challenge.Application.Services.Usuarios;
using tech_challenge.Application.Services.Veiculos;

namespace tech_challenge.Application.Services.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IClienteService, ClienteService>();
            services.AddScoped<IOrdemServicoService, OrdemServicoService>();
            services.AddScoped<IPecaInsumoService, PecaInsumoService>();
            services.AddScoped<IServicoService, ServicoService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IVeiculoService, VeiculoService>();

            return services;
        }
    }
}
