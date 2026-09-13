using Microsoft.EntityFrameworkCore;
using tech_challenge.Domain.Aggregates.Usuarios;
using tech_challenge.Domain.Common.Enums;
using tech_challenge.Domain.Shared.ValuesObjects;

namespace tech_challenge.Infrastructure.Persistence.Context.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            var senhaHash = BCrypt.Net.BCrypt.HashPassword("123456");

            if (!await context.Usuarios.AnyAsync(u => u.Login == "atendimento@techchallengefase1.com"))
            {
                var usuario = new Usuario
                {
                    Nome = "Atendimento",
                    Login = "atendimento@techchallengefase1.com",
                    Senha = senhaHash,
                    Documento = new Documento("899.165.050-39"),
                    Perfil = PerfilUsuario.Atendimento
                };

                await context.Usuarios.AddAsync(usuario);
            }

            if (!await context.Usuarios.AnyAsync(u => u.Login == "mecanico@techchallengefase1.com"))
            {
                var usuario = new Usuario
                {
                    Nome = "Mecânico",
                    Login = "mecanico@techchallengefase1.com",
                    Senha = senhaHash,
                    Documento = new Documento("899.165.050-39"),
                    Perfil = PerfilUsuario.Mecanico
                };

                await context.Usuarios.AddAsync(usuario);
            }

            await context.SaveChangesAsync();
        }
    }
}
