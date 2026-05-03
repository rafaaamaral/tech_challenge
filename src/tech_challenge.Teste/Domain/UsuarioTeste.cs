using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Domain.Aggregates.Usuarios;
using tech_challenge.Domain.Common.Enums;
using tech_challenge.Domain.Exceptions;

namespace tech_challenge.Teste.Domain
{
    public class UsuarioTeste
    {
        [Fact]
        public void Deve_Criar_Usuario_Valido()
        {
            var usuario = Usuario.Criar(
                Guid.NewGuid(),
                "Rafael Amaral",
                "rafael.amaral@example.com",
                PerfilUsuario.Administrador
            );
            usuario.Nome.Should().Be("Rafael Amaral");
            usuario.Login.Should().Be("rafael.amaral@example.com");
        }

        [Fact]
        public void Deve_Lancar_Exception_Quando_Nome_For_Vazio()
        {
            var act = () => Usuario.Criar(
                Guid.NewGuid(),
                "",
                "",
                PerfilUsuario.Administrador);

            act.Should()
               .Throw<DomainException>()
               .WithMessage("O nome do usuário é obrigatório.");
        }

        [Fact]
        public void Deve_Lancar_Exception_Quando_Login_For_Vazio()
        {
            var act = () => Usuario.Criar(
                Guid.NewGuid(),
                "Rafael Amaral",
                "",
                PerfilUsuario.Administrador);
            act.Should()
               .Throw<DomainException>()
               .WithMessage("O login do usuário é obrigatório.");
        }
    }
}
