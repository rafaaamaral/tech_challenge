using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Application.Exceptions;
using tech_challenge.Application.Interfaces.Repositories;
using tech_challenge.Application.Services.Usuarios;
using tech_challenge.Domain.Aggregates.Usuarios;
using tech_challenge.Domain.Common.Enums;

namespace tech_challenge.Teste.Application.Usuarios
{
    public class UsuarioServiceTeste
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly UsuarioService _service;

        public UsuarioServiceTeste()
        {
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            _service = new UsuarioService(
                _usuarioRepositoryMock.Object
            );
        }

        [Fact]
        public async Task ObterPorEmailAsync_DeveRetornarUsuarioModel_QuandoUsuarioExistir()
        {
            // Arrange
            var email = "rafael@email.com";

            var usuario = Usuario.Criar(
                Guid.NewGuid(),
                "Rafael Amaral",
                email,
                PerfilUsuario.Cliente
            );

            _usuarioRepositoryMock
                .Setup(x => x.ObterPorEmailAsync(email))
                .ReturnsAsync(usuario);

            // Act
            var result = await _service.ObterPorEmailAsync(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(usuario.UniqueCode, result.UniqueCode);
            Assert.Equal(usuario.Nome, result.Nome);
            Assert.Equal(usuario.Login, result.Login);
            Assert.Equal(usuario.Senha, result.Senha);
            Assert.Equal(usuario.Perfil, result.Perfil);

            _usuarioRepositoryMock.Verify(x => x.ObterPorEmailAsync(email), Times.Once);
        }

        [Fact]
        public async Task ObterPorEmailAsync_DeveLancarExcecao_QuandoUsuarioForNull()
        {
            // Arrange
            var email = "naoexiste@email.com";

            _usuarioRepositoryMock
                .Setup(x => x.ObterPorEmailAsync(email))
                .ReturnsAsync((Usuario?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.ObterPorEmailAsync(email)
            );
        }
    }
}
