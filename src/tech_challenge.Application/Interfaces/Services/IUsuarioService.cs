using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Application.Services.Usuarios.Model;

namespace tech_challenge.Application.Interfaces.Services
{
    public interface IUsuarioService
    {
        Task<UsuarioModel> ObterPorEmailAsync(string email);
    }
}
