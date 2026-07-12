using Microsoft.Extensions.Logging;
using tech_challenge.Application.Common.Interfaces;
using tech_challenge.Application.Exceptions;
using tech_challenge.Application.Interfaces.Repositories;
using tech_challenge.Application.Interfaces.Services;
using tech_challenge.Application.Services.Base;
using tech_challenge.Application.Services.OrdemServicos.Model;
using tech_challenge.Domain.Aggregates.OrdemServicos;
using tech_challenge.Domain.Common.Enums;

namespace tech_challenge.Application.Services.OrdemServicos
{
    public class OrdemServicoService : ServiceBase<OrdemServicoService>, IOrdemServicoService
    {
        private readonly IOrdemServicoRepository _ordemServicoRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IVeiculoRepository _veiculoRepository;
        private readonly IServicoRepository _servicoRepository;
        private readonly IPecaInsumoRepository _pecaInsumoRepository;
        private readonly IUsuarioLogadoService _usuarioLogadoService;
        private readonly IEmailService _emailService;
        public OrdemServicoService(
            IOrdemServicoRepository ordemServicoRepository,
            IClienteRepository clienteRepository,
            IVeiculoRepository veiculoRepository,
            IServicoRepository servicoRepository,
            IPecaInsumoRepository pecaInsumoRepository,
            IUsuarioLogadoService usuarioLogadoService,
            IEmailService emailService,
            ILogger<OrdemServicoService> logger) : base(logger)
        {
            _ordemServicoRepository = ordemServicoRepository;
            _clienteRepository = clienteRepository;
            _veiculoRepository = veiculoRepository;
            _servicoRepository = servicoRepository;
            _pecaInsumoRepository = pecaInsumoRepository;
            _usuarioLogadoService = usuarioLogadoService;
            _emailService = emailService;
        }

        public async Task<OrdemServicoModel> CriarAsync(
            int clienteId,
            int veiculoId,
            List<CriarOrdemServicoItemServicoModel> itensServicos,
            List<CriarOrdemServicoItemPecaInsumoModel> itensPecasInsumos)
        {
            _logger.LogInformation("Criando ordem de serviço para o cliente {ClienteId} e veículo {VeiculoId}", clienteId, veiculoId);

            var cliente = await _clienteRepository.GetByIdAsync(clienteId);
            if (cliente == null)
                throw new NotFoundException("Cliente", clienteId);

            var veiculo = await _veiculoRepository.GetByIdAsync(veiculoId);
            if (veiculo == null)
                throw new NotFoundException("Veículo", veiculoId);

            if (veiculo.ClienteId != clienteId)
                throw new InvalidOperationException("O veículo informado não pertence ao cliente.");

            if (!itensServicos.Any() && !itensPecasInsumos.Any())
                throw new InvalidOperationException("A OS deve possuir ao menos um serviço, peça ou insumo.");

            var numero = await ObterProximoNumeroAsync();
            var ordemServico = OrdemServico.Criar(clienteId, veiculoId, numero);

            foreach (var item in itensServicos)
            {
                var servico = await _servicoRepository.GetByIdAsync(item.ServicoId);
                if (servico == null)
                    throw new NotFoundException("Serviço", item.ServicoId);

                ordemServico.AdicionarServico(servico.Id, servico.Nome, servico.PrecoBase);
            }

            foreach (var item in itensPecasInsumos)
            {
                var pecaInsumo = await _pecaInsumoRepository.GetByIdAsync(item.PecaInsumoId);
                if (pecaInsumo == null)
                    throw new NotFoundException("Peça/Insumo", item.PecaInsumoId);

                ordemServico.AdicionarPecaInsumo(pecaInsumo.Id, pecaInsumo.Nome, item.Quantidade, pecaInsumo.PrecoUnitario);
            }

            var result = await _ordemServicoRepository.AddAsync(ordemServico);

            _logger.LogInformation("Ordem de serviço {Numero} criada com UniqueCode {UniqueCode}", result.Numero, result.UniqueCode);

            return result.ToModel();
        }

        public async Task<List<OrdemServicoModel>> ListarTodosAsync()
        {
            var ordens = await _ordemServicoRepository.ListarComItensAsync();
            return ordens.Select(x => x.ToModel()).ToList();
        }

        public async Task<OrdemServicoModel> ObterPorIdAsync(int id)
        {
            var ordemServico = await ObterComItensOuFalharAsync(id);
            return ordemServico.ToModel();
        }

        public async Task<OrdemServicoModel> IniciarDiagnosticoAsync(int id)
        {
            var ordemServico = await ObterOuFalharAsync(id);
            ordemServico.IniciarDiagnostico();
            await _ordemServicoRepository.UpdateAsync(ordemServico);
            return await ObterPorIdAsync(id);
        }

        public async Task<OrdemServicoModel> GerarOrcamentoAsync(int id)
        {
            var ordemServico = await ObterComItensOuFalharAsync(id);
            ordemServico.GerarOrcamento();
            await _ordemServicoRepository.UpdateAsync(ordemServico);
            var orcamentoModel = await ObterPorIdAsync(id);
            try
            {
                await _emailService.EnviarEmailAprovacaoOrcamentoAsync(orcamentoModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar e-mail de aprovação da OS {Id}", ordemServico.Id);
            }
            return orcamentoModel;
        }

        public async Task<OrdemServicoModel> AprovarOrcamentoAsync(int id)
        {
            var ordemServico = await ObterOuFalharAsync(id);
            ordemServico.AprovarOrcamento();
            ordemServico.IniciarExecucao();
            await _ordemServicoRepository.UpdateAsync(ordemServico);
            return await ObterPorIdAsync(id);
        }

        public async Task<OrdemServicoModel> ReprovarOrcamentoAsync(int id)
        {
            var ordemServico = await ObterOuFalharAsync(id);
            ordemServico.ReprovarOrcamento();
            await _ordemServicoRepository.UpdateAsync(ordemServico);
            return await ObterPorIdAsync(id);
        }

        public async Task<OrdemServicoModel> IniciarExecucaoAsync(int id)
        {
            var ordemServico = await ObterOuFalharAsync(id);
            ordemServico.IniciarExecucao();
            await _ordemServicoRepository.UpdateAsync(ordemServico);
            return await ObterPorIdAsync(id);
        }

        public async Task<OrdemServicoModel> FinalizarExecucaoAsync(int id)
        {
            var ordemServico = await ObterOuFalharAsync(id);
            ordemServico.FinalizarExecucao();
            await _ordemServicoRepository.UpdateAsync(ordemServico);
            return await ObterPorIdAsync(id);
        }

        public async Task<OrdemServicoModel> EntregarAsync(int id)
        {
            var ordemServico = await ObterOuFalharAsync(id);
            ordemServico.Entregar();
            await _ordemServicoRepository.UpdateAsync(ordemServico);
            return await ObterPorIdAsync(id);
        }

        public async Task DeletarAsync(int id)
        {
            var ordemServico = await ObterOuFalharAsync(id);
            ordemServico.Delete();
            await _ordemServicoRepository.UpdateAsync(ordemServico);
        }

        private async Task<int> ObterProximoNumeroAsync()
        {
            var total = await _ordemServicoRepository.CountAsync();
            return total + 1;
        }

        private async Task<OrdemServico> ObterOuFalharAsync(int id)
        {
            var ordemServico = await _ordemServicoRepository.GetByIdAsync(id);
            if (ordemServico == null)
                throw new NotFoundException("Ordem de Serviço", id);

            return ordemServico;
        }

        private async Task<OrdemServico> ObterComItensOuFalharAsync(int id)
        {
            var ordemServico = await _ordemServicoRepository.ObterPorIdComItensAsync(id);
            if (ordemServico == null)
                throw new NotFoundException("Ordem de Serviço", id);

            return ordemServico;
        }

        public async Task<OrdemServicoModel> ConsultarStatusAsync(Guid uniqueCode)
        {
            var ordemServico = await _ordemServicoRepository.ObterPorUniqueCodeAsync(uniqueCode);
            if (ordemServico == null)
                throw new NotFoundException("Ordem de Serviço", uniqueCode);

            if (ordemServico.Cliente.UniqueCode != _usuarioLogadoService.UniqueCode)
                throw new UnauthorizedAccessException("Você não tem permissão para acessar esta ordem de serviço.");

            return ordemServico.ToModel();
        }

        public async Task<List<OrdemServicoModel>> ListarPorClienteAsync()
        {
            var clienteId = _usuarioLogadoService.UniqueCode ?? throw new UnauthorizedAccessException("Você não tem permissão para acessar esta ordem de serviço.");
            var ordensServico = await _ordemServicoRepository.ListarPorClienteAsync(clienteId);
            return ordensServico.Select(x => x.ToModel()).ToList();
        }

        public async Task AlterarStatusOrcamentoAsync(Guid uniqueCode, StatusOrcamento status)
        {
            var ordemServico = await _ordemServicoRepository.ObterPorUniqueCodeAsync(uniqueCode);
            if (ordemServico == null)
                throw new NotFoundException("Ordem de Serviço", uniqueCode);

            switch (status)
            {
                case StatusOrcamento.Aprovado:
                    ordemServico.AprovarOrcamento();
                    ordemServico.IniciarExecucao();
                    break;
                case StatusOrcamento.Reprovado:
                    ordemServico.ReprovarOrcamento();
                    break;
                default:
                    throw new InvalidOperationException("Status de orçamento inválido.");
            }
            await _ordemServicoRepository.UpdateAsync(ordemServico);
        }
    }
}
