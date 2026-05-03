using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using tech_challenge.Domain.Common.Enums;

namespace tech_challenge.Application.Services.Usuarios.Model
{
    [ExcludeFromCodeCoverage]
    public class UsuarioModel
    {
        public Guid UniqueCode { get; set; }
        public required string Nome { get; set; } 
        public required string Login { get; set; }
        public required string Senha { get; set; }
        public required PerfilUsuario Perfil { get; set; }
    }
}
