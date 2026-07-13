using Microsoft.Extensions.Logging;
using Moq;
using tech_challenge.Application.Common.Interfaces;
using tech_challenge.Application.Exceptions;
using tech_challenge.Application.Interfaces.Repositories;
using tech_challenge.Application.Interfaces.Services;
using tech_challenge.Application.Services.OrdemServicos;
using tech_challenge.Application.Services.OrdemServicos.Model;
using tech_challenge.Domain.Aggregates.Clientes;
using tech_challenge.Domain.Aggregates.OrdemServicos;
using tech_challenge.Domain.Aggregates.PecaInsumos;
using tech_challenge.Domain.Aggregates.Servicos;
using tech_challenge.Domain.Aggregates.Veiculos;
using tech_challenge.Domain.Common.Enums;

namespace tech_challenge.Teste.Application.OrdemServicos
{
    public class OrdemServicoServiceTeste
    {
        private readonly Mock<IOrdemServicoRepository> _ordemServicoRepositoryMock;
        private readonly Mock<IClienteRepository> _clienteRepositoryMock;
        private readonly Mock<IVeiculoRepository> _veiculoRepositoryMock;
        private readonly Mock<IServicoRepository> _servicoRepositoryMock;
        private readonly Mock<IPecaInsumoRepository> _pecaInsumoRepositoryMock;
        private readonly Mock<IUsuarioLogadoService> _usuarioLogadoServiceMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<ILogger<OrdemServicoService>> _loggerMock;
        private readonly OrdemServicoService _service;

        public OrdemServicoServiceTeste()
        {
            _ordemServicoRepositoryMock = new Mock<IOrdemServicoRepository>();
            _clienteRepositoryMock = new Mock<IClienteRepository>();
            _veiculoRepositoryMock = new Mock<IVeiculoRepository>();
            _servicoRepositoryMock = new Mock<IServicoRepository>();
            _pecaInsumoRepositoryMock = new Mock<IPecaInsumoRepository>();
            _usuarioLogadoServiceMock = new Mock<IUsuarioLogadoService>();
            _emailServiceMock = new Mock<IEmailService>();
            _loggerMock = new Mock<ILogger<OrdemServicoService>>();

            _service = new OrdemServicoService(
                _ordemServicoRepositoryMock.Object,
                _clienteRepositoryMock.Object,
                _veiculoRepositoryMock.Object,
                _servicoRepositoryMock.Object,
                _pecaInsumoRepositoryMock.Object,
                _usuarioLogadoServiceMock.Object,
                _emailServiceMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task CriarAsync_DeveCriarOrdemServico_ComServicosEPecas()
        {
            // Arrange
            var cliente = CriarCliente(1);
            var veiculo = CriarVeiculo(2, cliente.Id);
            var servico = CriarServico(3);
            var pecaInsumo = CriarPecaInsumo(4);

            _clienteRepositoryMock.Setup(x => x.GetByIdAsync(cliente.Id)).ReturnsAsync(cliente);
            _veiculoRepositoryMock.Setup(x => x.GetByIdAsync(veiculo.Id)).ReturnsAsync(veiculo);
            _servicoRepositoryMock.Setup(x => x.GetByIdAsync(servico.Id)).ReturnsAsync(servico);
            _pecaInsumoRepositoryMock.Setup(x => x.GetByIdAsync(pecaInsumo.Id)).ReturnsAsync(pecaInsumo);
            _ordemServicoRepositoryMock.Setup(x => x.CountAsync()).ReturnsAsync(7);
            _ordemServicoRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<OrdemServico>()))
                .ReturnsAsync((OrdemServico ordemServico) => ordemServico);

            var itensServicos = new List<CriarOrdemServicoItemServicoModel>
            {
                new CriarOrdemServicoItemServicoModel { ServicoId = servico.Id }
            };
            var itensPecasInsumos = new List<CriarOrdemServicoItemPecaInsumoModel>
            {
                new CriarOrdemServicoItemPecaInsumoModel { PecaInsumoId = pecaInsumo.Id, Quantidade = 2 }
            };

            // Act
            var result = await _service.CriarAsync(cliente.Id, veiculo.Id, itensServicos, itensPecasInsumos);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cliente.Id, result.ClienteId);
            Assert.Equal(veiculo.Id, result.VeiculoId);
            Assert.Equal(8, result.Numero);
            Assert.Equal(StatusOrdemServico.Recebida, result.Status);
            Assert.Single(result.ItensServicos);
            Assert.Single(result.ItensPecasInsumos);
            Assert.Equal(100m, result.Orcamento.ValorServicos);
            Assert.Equal(100m, result.Orcamento.ValorPecasInsumos);
            Assert.Equal(200m, result.Orcamento.ValorTotal);

            _ordemServicoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<OrdemServico>()), Times.Once);
        }

        [Fact]
        public async Task CriarAsync_DeveLancarNotFoundException_QuandoClienteNaoExistir()
        {
            // Arrange
            _clienteRepositoryMock.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((Cliente?)null);

            // Act / Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.CriarAsync(
                    99,
                    1,
                    new List<CriarOrdemServicoItemServicoModel>
                    {
                        new CriarOrdemServicoItemServicoModel { ServicoId = 1 }
                    },
                    new List<CriarOrdemServicoItemPecaInsumoModel>()
                )
            );

            _ordemServicoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<OrdemServico>()), Times.Never);
        }

        [Fact]
        public async Task CriarAsync_DeveLancarInvalidOperationException_QuandoVeiculoNaoPertencerAoCliente()
        {
            // Arrange
            var cliente = CriarCliente(1);
            var veiculo = CriarVeiculo(2, 99);

            _clienteRepositoryMock.Setup(x => x.GetByIdAsync(cliente.Id)).ReturnsAsync(cliente);
            _veiculoRepositoryMock.Setup(x => x.GetByIdAsync(veiculo.Id)).ReturnsAsync(veiculo);

            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CriarAsync(
                    cliente.Id,
                    veiculo.Id,
                    new List<CriarOrdemServicoItemServicoModel>
                    {
                        new CriarOrdemServicoItemServicoModel { ServicoId = 1 }
                    },
                    new List<CriarOrdemServicoItemPecaInsumoModel>()
                )
            );

            // Assert
            Assert.Contains("não pertence ao cliente", exception.Message);
            _ordemServicoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<OrdemServico>()), Times.Never);
        }

        [Fact]
        public async Task CriarAsync_DeveLancarInvalidOperationException_QuandoNaoHouverItens()
        {
            // Arrange
            var cliente = CriarCliente(1);
            var veiculo = CriarVeiculo(2, cliente.Id);

            _clienteRepositoryMock.Setup(x => x.GetByIdAsync(cliente.Id)).ReturnsAsync(cliente);
            _veiculoRepositoryMock.Setup(x => x.GetByIdAsync(veiculo.Id)).ReturnsAsync(veiculo);

            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CriarAsync(
                    cliente.Id,
                    veiculo.Id,
                    new List<CriarOrdemServicoItemServicoModel>(),
                    new List<CriarOrdemServicoItemPecaInsumoModel>()
                )
            );

            // Assert
            Assert.Contains("ao menos um serviço, peça ou insumo", exception.Message);
            _ordemServicoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<OrdemServico>()), Times.Never);
        }

        [Fact]
        public async Task CriarAsync_DeveLancarNotFoundException_QuandoServicoNaoExistir()
        {
            // Arrange
            var cliente = CriarCliente(1);
            var veiculo = CriarVeiculo(2, cliente.Id);

            _clienteRepositoryMock.Setup(x => x.GetByIdAsync(cliente.Id)).ReturnsAsync(cliente);
            _veiculoRepositoryMock.Setup(x => x.GetByIdAsync(veiculo.Id)).ReturnsAsync(veiculo);
            _ordemServicoRepositoryMock.Setup(x => x.CountAsync()).ReturnsAsync(0);
            _servicoRepositoryMock.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((Servico?)null);

            // Act / Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.CriarAsync(
                    cliente.Id,
                    veiculo.Id,
                    new List<CriarOrdemServicoItemServicoModel>
                    {
                        new CriarOrdemServicoItemServicoModel { ServicoId = 99 }
                    },
                    new List<CriarOrdemServicoItemPecaInsumoModel>()
                )
            );

            _ordemServicoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<OrdemServico>()), Times.Never);
        }

        [Fact]
        public async Task CriarAsync_DeveLancarNotFoundException_QuandoPecaInsumoNaoExistir()
        {
            // Arrange
            var cliente = CriarCliente(1);
            var veiculo = CriarVeiculo(2, cliente.Id);

            _clienteRepositoryMock.Setup(x => x.GetByIdAsync(cliente.Id)).ReturnsAsync(cliente);
            _veiculoRepositoryMock.Setup(x => x.GetByIdAsync(veiculo.Id)).ReturnsAsync(veiculo);
            _ordemServicoRepositoryMock.Setup(x => x.CountAsync()).ReturnsAsync(0);
            _pecaInsumoRepositoryMock.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((PecaInsumo?)null);

            // Act / Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.CriarAsync(
                    cliente.Id,
                    veiculo.Id,
                    new List<CriarOrdemServicoItemServicoModel>(),
                    new List<CriarOrdemServicoItemPecaInsumoModel>
                    {
                        new CriarOrdemServicoItemPecaInsumoModel { PecaInsumoId = 99, Quantidade = 1 }
                    }
                )
            );

            _ordemServicoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<OrdemServico>()), Times.Never);
        }

        [Fact]
        public async Task ListarTodosAsync_DeveRetornarOrdensComItens()
        {
            // Arrange
            var ordemServico = CriarOrdemServicoComItens();
            _ordemServicoRepositoryMock
                .Setup(x => x.ListarComItensAsync())
                .ReturnsAsync(new List<OrdemServico> { ordemServico });

            // Act
            var result = await _service.ListarTodosAsync();

            // Assert
            Assert.Single(result);
            Assert.Single(result[0].ItensServicos);
            Assert.Single(result[0].ItensPecasInsumos);
        }

        [Fact]
        public async Task ObterPorIdAsync_DeveLancarNotFoundException_QuandoOrdemServicoNaoExistir()
        {
            // Arrange
            _ordemServicoRepositoryMock
                .Setup(x => x.ObterPorIdComItensAsync(99))
                .ReturnsAsync((OrdemServico?)null);

            // Act / Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.ObterPorIdAsync(99));
        }

        [Fact]
        public async Task GerarOrcamentoAsync_DeveAtualizarStatusParaAguardandoAprovacao()
        {
            // Arrange
            var ordemServico = CriarOrdemServicoComItens();
            _ordemServicoRepositoryMock
                .Setup(x => x.ObterPorIdComItensAsync(ordemServico.Id))
                .ReturnsAsync(ordemServico);

            // Act
            var result = await _service.GerarOrcamentoAsync(ordemServico.Id);

            // Assert
            Assert.Equal(StatusOrdemServico.AguardandoAprovacao, result.Status);
            Assert.Equal(StatusOrcamento.Pendente, result.Orcamento.Status);
            _ordemServicoRepositoryMock.Verify(x => x.UpdateAsync(ordemServico), Times.Once);
        }

        [Fact]
        public async Task IniciarDiagnosticoAsync_DeveAtualizarStatusParaEmDiagnostico()
        {
            // Arrange
            var ordemServico = CriarOrdemServicoComItens();

            _ordemServicoRepositoryMock.Setup(x => x.GetByIdAsync(ordemServico.Id)).ReturnsAsync(ordemServico);
            _ordemServicoRepositoryMock.Setup(x => x.ObterPorIdComItensAsync(ordemServico.Id)).ReturnsAsync(ordemServico);

            // Act
            var result = await _service.IniciarDiagnosticoAsync(ordemServico.Id);

            // Assert
            Assert.Equal(StatusOrdemServico.EmDiagnostico, result.Status);
            _ordemServicoRepositoryMock.Verify(x => x.UpdateAsync(ordemServico), Times.Once);
        }

        [Fact]
        public async Task AprovarOrcamentoAsync_DeveAprovarEIniciarExecucao()
        {
            // Arrange
            var ordemServico = CriarOrdemServicoAguardandoAprovacao();

            _ordemServicoRepositoryMock.Setup(x => x.GetByIdAsync(ordemServico.Id)).ReturnsAsync(ordemServico);
            _ordemServicoRepositoryMock.Setup(x => x.ObterPorIdComItensAsync(ordemServico.Id)).ReturnsAsync(ordemServico);

            // Act
            var result = await _service.AprovarOrcamentoAsync(ordemServico.Id);

            // Assert
            Assert.Equal(StatusOrdemServico.EmExecucao, result.Status);
            Assert.Equal(StatusOrcamento.Aprovado, result.Orcamento.Status);
            Assert.NotNull(result.InicioExecucao);
            _ordemServicoRepositoryMock.Verify(x => x.UpdateAsync(ordemServico), Times.Once);
        }

        [Fact]
        public async Task ReprovarOrcamentoAsync_DeveReprovarEManterAguardandoAprovacao()
        {
            // Arrange
            var ordemServico = CriarOrdemServicoAguardandoAprovacao();

            _ordemServicoRepositoryMock.Setup(x => x.GetByIdAsync(ordemServico.Id)).ReturnsAsync(ordemServico);
            _ordemServicoRepositoryMock.Setup(x => x.ObterPorIdComItensAsync(ordemServico.Id)).ReturnsAsync(ordemServico);

            // Act
            var result = await _service.ReprovarOrcamentoAsync(ordemServico.Id);

            // Assert
            Assert.Equal(StatusOrdemServico.AguardandoAprovacao, result.Status);
            Assert.Equal(StatusOrcamento.Reprovado, result.Orcamento.Status);
            _ordemServicoRepositoryMock.Verify(x => x.UpdateAsync(ordemServico), Times.Once);
        }

        [Fact]
        public async Task FinalizarExecucaoAsync_DeveFinalizarOrdemServico()
        {
            // Arrange
            var ordemServico = CriarOrdemServicoEmExecucao();

            _ordemServicoRepositoryMock.Setup(x => x.GetByIdAsync(ordemServico.Id)).ReturnsAsync(ordemServico);
            _ordemServicoRepositoryMock.Setup(x => x.ObterPorIdComItensAsync(ordemServico.Id)).ReturnsAsync(ordemServico);

            // Act
            var result = await _service.FinalizarExecucaoAsync(ordemServico.Id);

            // Assert
            Assert.Equal(StatusOrdemServico.Finalizada, result.Status);
            Assert.NotNull(result.FimExecucao);
            _ordemServicoRepositoryMock.Verify(x => x.UpdateAsync(ordemServico), Times.Once);
        }

        [Fact]
        public async Task IniciarExecucaoAsync_DeveIniciarOrdemServicoComOrcamentoAprovado()
        {
            // Arrange
            var ordemServico = CriarOrdemServicoAguardandoAprovacao();
            ordemServico.AprovarOrcamento();

            _ordemServicoRepositoryMock.Setup(x => x.GetByIdAsync(ordemServico.Id)).ReturnsAsync(ordemServico);
            _ordemServicoRepositoryMock.Setup(x => x.ObterPorIdComItensAsync(ordemServico.Id)).ReturnsAsync(ordemServico);

            // Act
            var result = await _service.IniciarExecucaoAsync(ordemServico.Id);

            // Assert
            Assert.Equal(StatusOrdemServico.EmExecucao, result.Status);
            Assert.NotNull(result.InicioExecucao);
            _ordemServicoRepositoryMock.Verify(x => x.UpdateAsync(ordemServico), Times.Once);
        }

        [Fact]
        public async Task EntregarAsync_DeveEntregarOrdemServicoFinalizada()
        {
            // Arrange
            var ordemServico = CriarOrdemServicoEmExecucao();
            ordemServico.FinalizarExecucao();

            _ordemServicoRepositoryMock.Setup(x => x.GetByIdAsync(ordemServico.Id)).ReturnsAsync(ordemServico);
            _ordemServicoRepositoryMock.Setup(x => x.ObterPorIdComItensAsync(ordemServico.Id)).ReturnsAsync(ordemServico);

            // Act
            var result = await _service.EntregarAsync(ordemServico.Id);

            // Assert
            Assert.Equal(StatusOrdemServico.Entregue, result.Status);
            _ordemServicoRepositoryMock.Verify(x => x.UpdateAsync(ordemServico), Times.Once);
        }

        [Fact]
        public async Task DeletarAsync_DeveInativarOrdemServico()
        {
            // Arrange
            var ordemServico = OrdemServico.Criar(1, 2, 10);
            ordemServico.Id = 1;

            _ordemServicoRepositoryMock.Setup(x => x.GetByIdAsync(ordemServico.Id)).ReturnsAsync(ordemServico);

            // Act
            await _service.DeletarAsync(ordemServico.Id);

            // Assert
            Assert.False(ordemServico.Ativo);
            _ordemServicoRepositoryMock.Verify(x => x.UpdateAsync(ordemServico), Times.Once);
        }

        [Fact]
        public async Task DeletarAsync_DeveLancarNotFoundException_QuandoOrdemServicoNaoExistir()
        {
            // Arrange
            _ordemServicoRepositoryMock.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((OrdemServico?)null);

            // Act / Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.DeletarAsync(99));

            _ordemServicoRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<OrdemServico>()), Times.Never);
        }

        private static Cliente CriarCliente(int id)
        {
            var cliente = Cliente.Criar("Rafael Amaral", "899.165.050-39", "rafael@email.com", "11999999999");
            cliente.Id = id;
            return cliente;
        }

        private static Veiculo CriarVeiculo(int id, int clienteId)
        {
            var veiculo = Veiculo.Criar(clienteId, "ABC1234", "Toyota", "Corolla", 2024);
            veiculo.Id = id;
            return veiculo;
        }

        private static Servico CriarServico(int id)
        {
            var servico = Servico.Criar("Troca de oleo", "SERV001", "Troca completa", 100m, 60);
            servico.Id = id;
            return servico;
        }

        private static PecaInsumo CriarPecaInsumo(int id)
        {
            var pecaInsumo = PecaInsumo.Criar("Oleo 5W30", "PEC001", "Oleo motor", TipoPecaInsumo.Peca, 50m, 10);
            pecaInsumo.Id = id;
            return pecaInsumo;
        }

        private static OrdemServico CriarOrdemServicoComItens()
        {
            var ordemServico = OrdemServico.Criar(1, 2, 10);
            ordemServico.Id = 1;
            ordemServico.AdicionarServico(3, "Troca de oleo", 100m);
            ordemServico.AdicionarPecaInsumo(4, "Oleo 5W30", 2, 50m);
            return ordemServico;
        }

        private static OrdemServico CriarOrdemServicoAguardandoAprovacao()
        {
            var ordemServico = CriarOrdemServicoComItens();
            ordemServico.GerarOrcamento();
            return ordemServico;
        }

        private static OrdemServico CriarOrdemServicoEmExecucao()
        {
            var ordemServico = CriarOrdemServicoAguardandoAprovacao();
            ordemServico.AprovarOrcamento();
            ordemServico.IniciarExecucao();
            return ordemServico;
        }
    }
}
