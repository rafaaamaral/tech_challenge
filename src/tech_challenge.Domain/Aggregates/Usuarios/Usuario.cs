using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Domain.Common.Entities;
using tech_challenge.Domain.Common.Enums;
using tech_challenge.Domain.Exceptions;

namespace tech_challenge.Domain.Aggregates.Usuarios
{
    public class Usuario : Audit
    {
        public string Nome { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public PerfilUsuario Perfil { get; set; }

        public Usuario() { }

        public static Usuario Criar(Guid uniqueCode, string nome, string login, PerfilUsuario perfil)
        {
            var senhaPadrao = BCrypt.Net.BCrypt.HashPassword("123456");// Senha padrão para novos usuários

            var usuario = new Usuario
            {
                UniqueCode = uniqueCode,
                Nome = nome,
                Login = login,
                Senha =  senhaPadrao,
                Perfil = perfil
            };
            usuario.Validar();
            return usuario;
        }

        public void Validar()
        {
            if (string.IsNullOrWhiteSpace(Nome))
                throw new DomainException("O nome do usuário é obrigatório.");
            if (string.IsNullOrWhiteSpace(Login))
                throw new DomainException("O login do usuário é obrigatório.");

        }
    }
}
