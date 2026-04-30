using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Domain.Common.Entities;
using tech_challenge.Domain.Common.Enums;

namespace tech_challenge.Domain.Aggregates.Usuarios
{
    public class Usuario : Audit
    {
        public string Nome { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public PerfilUsuario Perfil { get; set; }
    }
}
