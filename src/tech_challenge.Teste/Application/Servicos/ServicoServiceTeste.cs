using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using tech_challenge.Application.Exceptions;
using tech_challenge.Application.Interfaces.Repositories;
using tech_challenge.Application.Services.Servicos;
using tech_challenge.Domain.Aggregates.Servicos;

namespace tech_challenge.Teste.Application.Servicos
{
    public class ServicoServiceTeste
    {
        private readonly Mock<IServicoRepository> _servicoRepositoryMock;
        private readonly Mock<ILogger<ServicoService>> _loggerMock;
        private readonly ServicoService _service;

        public ServicoServiceTeste()
        {
            _servicoRepositoryMock = new Mock<IServicoRepository>();
            _loggerMock = new Mock<ILogger<ServicoService>>();

            _service = new ServicoService(
                _servicoRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task CriarAsync_DeveCriarServico_QuandoCodigoNaoExistir()
        {
            // Arrange
            _servicoRepositoryMock
                .Setup(x => x.FindAsync(It.IsAny<Expression<Func<Servico, bool>>>()))
                .ReturnsAsync(new List<Servico>());

            _servicoRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Servico>()))
                .ReturnsAsync((Servico s) => s);

            // Act
            var result = await _service.CriarAsync(
                "Troca de Óleo",
                "SERV001",
                "Troca completa",
                120m,
                60
            );

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Troca de Óleo", result.Nome);
            Assert.Equal("SERV001", result.Codigo);

            _servicoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Servico>()), Times.Once);
        }

        [Fact]
        public async Task CriarAsync_DeveLancarInvalidOperationException_QuandoCodigoJaExistir()
        {
            // Arrange
            var servicoExistente = Servico.Criar(
                "Troca de Óleo",
                "SERV001",
                "Troca completa",
                120m,
                60
            );

            _servicoRepositoryMock
                .Setup(x => x.FindAsync(It.IsAny<Expression<Func<Servico, bool>>>()))
                .ReturnsAsync(new List<Servico> { servicoExistente });

            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CriarAsync(
                    "Outro Serviço",
                    "SERV001",
                    "Descrição",
                    200m,
                    30
                )
            );

            // Assert
            Assert.Contains("Já existe", exception.Message);

            _servicoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Servico>()), Times.Never);
        }

        [Fact]
        public async Task DeletarAsync_DeveDeletarServico_QuandoExistir()
        {
            // Arrange
            var id = 1;

            var servico = Servico.Criar(
                "Troca de Óleo",
                "SERV001",
                "Troca completa",
                120m,
                60
            );

            _servicoRepositoryMock
                .Setup(x => x.GetByIdAsync(id))
                .ReturnsAsync(servico);

            _servicoRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Servico>()));

            // Act
            await _service.DeletarAsync(id);

            // Assert
            _servicoRepositoryMock.Verify(x => x.GetByIdAsync(id), Times.Once);
            _servicoRepositoryMock.Verify(x => x.UpdateAsync(servico), Times.Once);
        }

        [Fact]
        public async Task DeletarAsync_DeveLancarNotFoundException_QuandoNaoExistir()
        {
            // Arrange
            var id = 99;

            _servicoRepositoryMock
                .Setup(x => x.GetByIdAsync(id))
                .ReturnsAsync((Servico?)null);

            // Act / Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.DeletarAsync(id)
            );

            _servicoRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Servico>()), Times.Never);
        }

        [Fact]
        public async Task ObterPorCodigoAsync_DeveRetornarServico_QuandoExistir()
        {
            // Arrange
            var codigo = "SERV001";

            var servico = Servico.Criar(
                "Troca de Óleo",
                codigo,
                "Troca completa",
                120m,
                60
            );

            _servicoRepositoryMock
                .Setup(x => x.FindAsync(It.IsAny<Expression<Func<Servico, bool>>>()))
                .ReturnsAsync(new List<Servico> { servico });

            // Act
            var result = await _service.ObterPorCodigoAsync(codigo);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Troca de Óleo", result.Nome);
            Assert.Equal(codigo, result.Codigo);
        }

        [Fact]
        public async Task ObterPorCodigoAsync_DeveLancarNotFoundException_QuandoNaoExistir()
        {
            // Arrange
            var codigo = "SERV999";

            _servicoRepositoryMock
                .Setup(x => x.FindAsync(It.IsAny<Expression<Func<Servico, bool>>>()))
                .ReturnsAsync(new List<Servico>());

            // Act / Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.ObterPorCodigoAsync(codigo)
            );
        }

        [Fact]
        public async Task ObterTodosAsync_DeveRetornarListaDeServicos()
        {
            // Arrange
            var servicos = new List<Servico>
        {
            Servico.Criar("Troca de Óleo", "SERV001", "Troca completa", 120m, 60),
            Servico.Criar("Alinhamento", "SERV002", "Alinhamento completo", 80m, 40)
        };

            _servicoRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(servicos);

            // Act
            var result = await _service.ObterTodosAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task ObterTodosAsync_DeveRetornarListaVazia_QuandoNaoExistiremServicos()
        {
            // Arrange
            _servicoRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<Servico>());

            // Act
            var result = await _service.ObterTodosAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
