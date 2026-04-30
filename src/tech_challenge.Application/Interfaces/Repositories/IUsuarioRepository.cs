using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Application.Interfaces.Repositories.Base;
using tech_challenge.Domain.Aggregates.Usuarios;

namespace tech_challenge.Application.Interfaces.Repositories
{
    public interface IUsuarioRepository : IBaseRepository<Usuario>
    {
        Task<Usuario> ObterPorEmailAsync(string email);
    }
}
