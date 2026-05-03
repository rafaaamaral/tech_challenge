using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using tech_challenge.Application.Exceptions;
using tech_challenge.Application.Interfaces.Repositories;
using tech_challenge.Application.Services.PecaInsumos;
using tech_challenge.Domain.Aggregates.PecaInsumos;
using tech_challenge.Domain.Common.Enums;

namespace tech_challenge.Teste.Application.PecaInsumos
{
    public class PecaInsumoServiceTeste
    {
        private readonly Mock<IPecaInsumoRepository> _pecaInsumoRepositoryMock;
        private readonly Mock<ILogger<PecaInsumoService>> _loggerMock;
        private readonly PecaInsumoService _service;

        public PecaInsumoServiceTeste()
        {
            _pecaInsumoRepositoryMock = new Mock<IPecaInsumoRepository>();
            _loggerMock = new Mock<ILogger<PecaInsumoService>>();

            _service = new PecaInsumoService(
                _pecaInsumoRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task CriarAsync_DeveCriarPecaInsumo_QuandoCodigoNaoExistir()
        {
            // Arrange
            _pecaInsumoRepositoryMock
                .Setup(x => x.FindAsync(It.IsAny<Expression<Func<PecaInsumo, bool>>>()))
                .ReturnsAsync(new List<PecaInsumo>());

            _pecaInsumoRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<PecaInsumo>()))
                .ReturnsAsync((PecaInsumo peca) => peca);

            // Act
            var result = await _service.CriarAsync(
                "Pastilha de Freio",
                "PEC001",
                "Pastilha dianteira",
                150.50m,
                10
            );

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Pastilha de Freio", result.Nome);
            Assert.Equal("PEC001", result.Codigo);

            _pecaInsumoRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<PecaInsumo>()),
                Times.Once
            );
        }

        [Fact]
        public async Task CriarAsync_DeveLancarInvalidOperationException_QuandoCodigoJaExistir()
        {
            // Arrange
            var pecaExistente = PecaInsumo.Criar(
                "Pastilha de Freio",
                "PEC001",
                "Pastilha dianteira",
                TipoPecaInsumo.Peca,
                150.50m,
                10
            );

            _pecaInsumoRepositoryMock
                .Setup(x => x.FindAsync(It.IsAny<Expression<Func<PecaInsumo, bool>>>()))
                .ReturnsAsync(new List<PecaInsumo> { pecaExistente });

            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CriarAsync(
                    "Pastilha de Freio Nova",
                    "PEC001",
                    "Outra descrição",
                    200m,
                    5
                )
            );

            // Assert
            Assert.Contains("já existe", exception.Message);

            _pecaInsumoRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<PecaInsumo>()),
                Times.Never
            );
        }

        [Fact]
        public async Task DeletarAsync_DeveDeletarPecaInsumo_QuandoExistir()
        {
            // Arrange
            var id = 1;

            var peca = PecaInsumo.Criar(
                "Pastilha de Freio",
                "PEC001",
                "Pastilha dianteira",
                TipoPecaInsumo.Peca,
                150.50m,
                10
            );

            _pecaInsumoRepositoryMock
                .Setup(x => x.GetByIdAsync(id))
                .ReturnsAsync(peca);

            _pecaInsumoRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<PecaInsumo>()));

            // Act
            await _service.DeletarAsync(id);

            // Assert
            _pecaInsumoRepositoryMock.Verify(x => x.GetByIdAsync(id), Times.Once);
            _pecaInsumoRepositoryMock.Verify(x => x.UpdateAsync(peca), Times.Once);
        }

        [Fact]
        public async Task DeletarAsync_DeveLancarNotFoundException_QuandoNaoExistir()
        {
            // Arrange
            var id = 99;

            _pecaInsumoRepositoryMock
                .Setup(x => x.GetByIdAsync(id))
                .ReturnsAsync((PecaInsumo?)null);

            // Act / Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.DeletarAsync(id)
            );

            _pecaInsumoRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<PecaInsumo>()),
                Times.Never
            );
        }

        [Fact]
        public async Task ObterPorCodigoAsync_DeveRetornarPecaInsumo_QuandoCodigoExistir()
        {
            // Arrange
            var codigo = "PEC001";

            var peca = PecaInsumo.Criar(
                "Pastilha de Freio",
                codigo,
                "Pastilha dianteira",
                TipoPecaInsumo.Peca,
                150.50m,
                10
            );

            _pecaInsumoRepositoryMock
                .Setup(x => x.FindAsync(It.IsAny<Expression<Func<PecaInsumo, bool>>>()))
                .ReturnsAsync(new List<PecaInsumo> { peca });

            // Act
            var result = await _service.ObterPorCodigoAsync(codigo);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Pastilha de Freio", result.Nome);
            Assert.Equal(codigo, result.Codigo);
        }

        [Fact]
        public async Task ObterPorCodigoAsync_DeveLancarNotFoundException_QuandoCodigoNaoExistir()
        {
            // Arrange
            var codigo = "PEC999";

            _pecaInsumoRepositoryMock
                .Setup(x => x.FindAsync(It.IsAny<Expression<Func<PecaInsumo, bool>>>()))
                .ReturnsAsync(new List<PecaInsumo>());

            // Act / Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.ObterPorCodigoAsync(codigo)
            );
        }

        [Fact]
        public async Task ObterTodosAsync_DeveRetornarTodosOsInsumos()
        {
            // Arrange
            var insumos = new List<PecaInsumo>
        {
            PecaInsumo.Criar("Pastilha de Freio", "PEC001", "Pastilha dianteira", TipoPecaInsumo.Peca, 150.50m, 10),
            PecaInsumo.Criar("Óleo Motor", "PEC002", "Óleo 5w30", TipoPecaInsumo.Peca, 80m, 20)
        };

            _pecaInsumoRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(insumos);

            // Act
            var result = await _service.ObterTodosAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task ObterTodosAsync_DeveRetornarListaVazia_QuandoNaoHouverInsumos()
        {
            // Arrange
            _pecaInsumoRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<PecaInsumo>());

            // Act
            var result = await _service.ObterTodosAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
