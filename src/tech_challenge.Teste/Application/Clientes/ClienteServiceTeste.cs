using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using tech_challenge.Application.Exceptions;
using tech_challenge.Application.Interfaces.Repositories;
using tech_challenge.Application.Services.Clientes;
using tech_challenge.Domain.Aggregates.Clientes;
using tech_challenge.Domain.Aggregates.Usuarios;

namespace tech_challenge.Teste.Application.Clientes
{
    public class ClienteServiceTeste
    {
        private readonly Mock<IClienteRepository> _clienteRepositoryMock;
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly Mock<ILogger<ClienteService>> _loggerMock;
        private readonly ClienteService _service;

        public ClienteServiceTeste()
        {
            _clienteRepositoryMock = new Mock<IClienteRepository>();
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            _loggerMock = new Mock<ILogger<ClienteService>>();

            _service = new ClienteService(
                _clienteRepositoryMock.Object,
                _usuarioRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task AddAsync_DeveAdicionarCliente_QuandoDocumentoNaoExistir()
        {
            // Arrange
            var nome = "Rafael Amaral";
            var documento = "899.165.050-39";
            var email = "rafael@email.com";
            var telefone = "11999999999";

            _clienteRepositoryMock
                .Setup(x => x.FindAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
                .ReturnsAsync(new List<Cliente>());

            _clienteRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Cliente>()))
                .ReturnsAsync((Cliente cliente) => cliente);

            _usuarioRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Usuario>()))
                .ReturnsAsync((Usuario usuario) => usuario);

            // Act
            var result = await _service.AddAsync(nome, documento, email, telefone);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nome, result.Nome);
            Assert.Equal(email, result.Email);

            _clienteRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Cliente>()), Times.Once);
            _usuarioRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Usuario>()), Times.Once);
        }

        [Fact]
        public async Task AddAsync_DeveLancarInvalidOperationException_QuandoDocumentoJaExistir()
        {
            // Arrange
            var clienteExistente = Cliente.Criar(
                "Cliente Existente",
                "899.165.050-39",
                "cliente@email.com",
                "11999999999"
            );

            _clienteRepositoryMock
                .Setup(x => x.FindAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
                .ReturnsAsync(new List<Cliente> { clienteExistente });

            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.AddAsync(
                    "Rafael Amaral",
                    "899.165.050-39",
                    "rafael@email.com",
                    "11999999999"
                )
            );

            // Assert
            Assert.Contains("já existe", exception.Message);

            _clienteRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Cliente>()), Times.Never);
            _usuarioRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task DeletarAsync_DeveDeletarCliente_QuandoClienteExistir()
        {
            // Arrange
            var id = 1;

            var cliente = Cliente.Criar(
                "Rafael Amaral",
                "899.165.050-39",
                "rafael@email.com",
                "11999999999"
            );

            _clienteRepositoryMock
                .Setup(x => x.GetByIdAsync(id))
                .ReturnsAsync(cliente);

            _clienteRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Cliente>()));

            // Act
            await _service.DeletarAsync(id);

            // Assert
            _clienteRepositoryMock.Verify(x => x.GetByIdAsync(id), Times.Once);
            _clienteRepositoryMock.Verify(x => x.UpdateAsync(cliente), Times.Once);
        }

        [Fact]
        public async Task DeletarAsync_DeveLancarNotFoundException_QuandoClienteNaoExistir()
        {
            // Arrange
            var id = 99;

            _clienteRepositoryMock
                .Setup(x => x.GetByIdAsync(id))
                .ReturnsAsync((Cliente?)null);

            // Act
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.DeletarAsync(id)
            );

            // Assert
            _clienteRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Cliente>()), Times.Never);
        }

        [Fact]
        public async Task ListarTodosAsync_DeveRetornarClientes()
        {
            // Arrange
            var clientes = new List<Cliente>
        {
            Cliente.Criar("Cliente 1", "899.165.050-39", "cliente1@email.com", "11999999999"),
            Cliente.Criar("Cliente 2", "191.870.540-27", "cliente2@email.com", "11888888888")
        };

            _clienteRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(clientes);

            // Act
            var result = await _service.ListarTodosAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            _clienteRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task ListarTodosAsync_DeveRetornarListaVazia_QuandoNaoExistiremClientes()
        {
            // Arrange
            _clienteRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<Cliente>());

            // Act
            var result = await _service.ListarTodosAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task ObterPorDocumentoAsync_DeveRetornarCliente_QuandoDocumentoExistir()
        {
            // Arrange
            var documento = "899.165.050-39";

            var cliente = Cliente.Criar(
                "Rafael Amaral",
                documento,
                "rafael@email.com",
                "11999999999"
            );

            _clienteRepositoryMock
                .Setup(x => x.FindAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
                .ReturnsAsync(new List<Cliente> { cliente });

            // Act
            var result = await _service.ObterPorDocumentoAsync(documento);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Rafael Amaral", result.Nome);
            Assert.Equal("rafael@email.com", result.Email);
        }

        [Fact]
        public async Task ObterPorDocumentoAsync_DeveLancarNotFoundException_QuandoDocumentoNaoExistir()
        {
            // Arrange
            var documento = "899.165.050-39";

            _clienteRepositoryMock
                .Setup(x => x.FindAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
                .ReturnsAsync(new List<Cliente>());

            // Act / Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.ObterPorDocumentoAsync(documento)
            );
        }
    }
}
