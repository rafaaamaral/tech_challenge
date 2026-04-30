using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Application.Interfaces.Repositories.Base;
using tech_challenge.Application.Interfaces.Services;
using tech_challenge.Application.Services.Clientes;
using tech_challenge.Application.Services.Usuarios;

namespace tech_challenge.Application.Services.DependencyInjection
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IClienteService, ClienteService>();
            services.AddScoped<IUsuarioService, UsuarioService>();

            return services;
        }
    }
}
