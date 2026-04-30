using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Application.Interfaces.Repositories;
using tech_challenge.Domain.Aggregates.Usuarios;
using tech_challenge.Infrastructure.Persistence.Context;
using tech_challenge.Infrastructure.Persistence.Repositories.Base;

namespace tech_challenge.Infrastructure.Persistence.Repositories
{
    public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Usuario> ObterPorEmailAsync(string email)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Login == email);

            if (usuario == null)
                throw new Exception("Usuário não encontrado");

            return usuario;
        }
    }
}
