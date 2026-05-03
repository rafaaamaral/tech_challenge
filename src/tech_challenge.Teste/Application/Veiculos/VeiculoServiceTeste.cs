using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using tech_challenge.Application.Exceptions;
using tech_challenge.Application.Interfaces.Repositories;
using tech_challenge.Application.Services.Veiculos;
using tech_challenge.Domain.Aggregates.Veiculos;

namespace tech_challenge.Teste.Application.Veiculos;

public class VeiculoServiceTeste
{
    private readonly Mock<IVeiculoRepository> _veiculoRepositoryMock;
    private readonly Mock<ILogger<VeiculoService>> _loggerMock;
    private readonly VeiculoService _service;

    public VeiculoServiceTeste()
    {
        _veiculoRepositoryMock = new Mock<IVeiculoRepository>();
        _loggerMock = new Mock<ILogger<VeiculoService>>();

        _service = new VeiculoService(
            _veiculoRepositoryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task AddAsync_DeveAdicionarVeiculo_QuandoPlacaNaoExistir()
    {
        // Arrange
        var clienteId = 1;
        var placa = "ABC1234";
        var marca = "Toyota";
        var modelo = "Corolla";
        var ano = 2024;

        _veiculoRepositoryMock
            .Setup(x => x.FindAsync(It.IsAny<Expression<Func<Veiculo, bool>>>()))
            .ReturnsAsync(new List<Veiculo>());

        _veiculoRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Veiculo>()))
            .ReturnsAsync((Veiculo veiculo) => veiculo);

        // Act
        var result = await _service.AddAsync(clienteId, placa, marca, modelo, ano);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(marca, result.Marca);
        Assert.Equal(modelo, result.Modelo);

        _veiculoRepositoryMock.Verify(x => x.FindAsync(It.IsAny<Expression<Func<Veiculo, bool>>>()), Times.Once);
        _veiculoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Veiculo>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_DeveLancarInvalidOperationException_QuandoPlacaJaExistir()
    {
        // Arrange
        var clienteId = 1;
        var placa = "ABC1234";

        var veiculoExistente = Veiculo.Criar(
            clienteId,
            placa,
            "Toyota",
            "Corolla",
            2024
        );

        _veiculoRepositoryMock
            .Setup(x => x.FindAsync(It.IsAny<Expression<Func<Veiculo, bool>>>()))
            .ReturnsAsync(new List<Veiculo> { veiculoExistente });

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.AddAsync(
                clienteId,
                placa,
                "Honda",
                "Civic",
                2023
            )
        );

        // Assert
        Assert.Contains("já existe", exception.Message);

        _veiculoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Veiculo>()), Times.Never);
    }

    [Fact]
    public async Task DeletarAsync_DeveDeletarVeiculo_QuandoVeiculoExistir()
    {
        // Arrange
        var veiculoId = 1;

        var veiculo = Veiculo.Criar(
            1,
            "ABC1234",
            "Toyota",
            "Corolla",
            2024
        );

        _veiculoRepositoryMock
            .Setup(x => x.GetByIdAsync(veiculoId))
            .ReturnsAsync(veiculo);

        _veiculoRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Veiculo>()));

        // Act
        await _service.DeletarAsync(veiculoId);

        // Assert
        _veiculoRepositoryMock.Verify(x => x.GetByIdAsync(veiculoId), Times.Once);
        _veiculoRepositoryMock.Verify(x => x.UpdateAsync(veiculo), Times.Once);
    }

    [Fact]
    public async Task DeletarAsync_DeveLancarNotFoundException_QuandoVeiculoNaoExistir()
    {
        // Arrange
        var veiculoId = 99;

        _veiculoRepositoryMock
            .Setup(x => x.GetByIdAsync(veiculoId))
            .ReturnsAsync((Veiculo?)null);

        // Act / Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.DeletarAsync(veiculoId)
        );

        _veiculoRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Veiculo>()), Times.Never);
    }

    [Fact]
    public async Task ListarVeiculoPorClienteAsync_DeveRetornarVeiculosDoCliente()
    {
        // Arrange
        var clienteId = 1;

        var veiculos = new List<Veiculo>
        {
            Veiculo.Criar(clienteId, "ABC1234", "Toyota", "Corolla", 2024),
            Veiculo.Criar(clienteId, "DEF5678", "Honda", "Civic", 2023)
        };

        _veiculoRepositoryMock
            .Setup(x => x.FindAsync(It.IsAny<Expression<Func<Veiculo, bool>>>()))
            .ReturnsAsync(veiculos);

        // Act
        var result = await _service.ListarVeiculoPorClienteAsync(clienteId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        _veiculoRepositoryMock.Verify(x => x.FindAsync(It.IsAny<Expression<Func<Veiculo, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task ListarVeiculoPorClienteAsync_DeveRetornarListaVazia_QuandoClienteNaoTiverVeiculos()
    {
        // Arrange
        var clienteId = 1;

        _veiculoRepositoryMock
            .Setup(x => x.FindAsync(It.IsAny<Expression<Func<Veiculo, bool>>>()))
            .ReturnsAsync(new List<Veiculo>());

        // Act
        var result = await _service.ListarVeiculoPorClienteAsync(clienteId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ObterVeiculoPorPlacaEClienteAsync_DeveRetornarVeiculo_QuandoExistir()
    {
        // Arrange
        var clienteId = 1;
        var placa = "ABC1234";

        var veiculo = Veiculo.Criar(
            clienteId,
            placa,
            "Toyota",
            "Corolla",
            2024
        );

        _veiculoRepositoryMock
            .Setup(x => x.FindAsync(It.IsAny<Expression<Func<Veiculo, bool>>>()))
            .ReturnsAsync(new List<Veiculo> { veiculo });

        // Act
        var result = await _service.ObterVeiculoPorPlacaEClienteAsync(clienteId, placa);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Toyota", result.Marca);
        Assert.Equal("Corolla", result.Modelo);
    }

    [Fact]
    public async Task ObterVeiculoPorPlacaEClienteAsync_DeveLancarNotFoundException_QuandoNaoExistir()
    {
        // Arrange
        var clienteId = 1;
        var placa = "ABC1234";

        _veiculoRepositoryMock
            .Setup(x => x.FindAsync(It.IsAny<Expression<Func<Veiculo, bool>>>()))
            .ReturnsAsync(new List<Veiculo>());

        // Act / Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.ObterVeiculoPorPlacaEClienteAsync(clienteId, placa)
        );
    }
}