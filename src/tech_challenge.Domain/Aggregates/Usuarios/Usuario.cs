using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using tech_challenge.Domain.Common.Entities;
using tech_challenge.Domain.Common.Enums;
using tech_challenge.Domain.Exceptions;
using tech_challenge.Domain.Shared.ValuesObjects;

namespace tech_challenge.Domain.Aggregates.Usuarios
{
    public class Usuario : Audit
    {
        public string Nome { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public Documento Documento { get; set; } 
        public string Senha { get; set; } = string.Empty;
        public PerfilUsuario Perfil { get; set; }

        public Usuario() { }

        public static Usuario Criar(Guid uniqueCode, string nome, string login, string documento, PerfilUsuario perfil)
        {
            var senhaPadrao = BCrypt.Net.BCrypt.HashPassword("123456");// Senha padrão para novos usuários

            var usuario = new Usuario
            {
                UniqueCode = uniqueCode,
                Nome = nome,
                Login = login,
                Documento = new Documento(documento),
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
