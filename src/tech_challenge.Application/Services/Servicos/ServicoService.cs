using Microsoft.Extensions.Logging;
using tech_challenge.Application.Exceptions;
using tech_challenge.Application.Interfaces.Repositories;
using tech_challenge.Application.Interfaces.Services;
using tech_challenge.Application.Services.Base;
using tech_challenge.Application.Services.Servicos.Model;
using tech_challenge.Domain.Aggregates.Servicos;

namespace tech_challenge.Application.Services.Servicos
{
    public class ServicoService : ServiceBase<ServicoService>, IServicoService
    {
        private readonly IServicoRepository _servicoRepository;
        public ServicoService(IServicoRepository servicoRepository, ILogger<ServicoService> logger) : base(logger)
        {
            _servicoRepository = servicoRepository;
        }

        public async Task<ServicoModel> CriarAsync(string nome, string codigo, string? descricao, decimal precoBase, int? tempoEstimado)
        {
            _logger.LogInformation("Criando serviço: {Nome} com código {Codigo}", nome, codigo);

            var servicoExistente = await _servicoRepository.FindAsync(x => x.Codigo == codigo);

            if (servicoExistente.Any())
            {
                _logger.LogWarning("Serviço com código {Codigo} já existe", codigo);
                throw new InvalidOperationException($"Já existe um serviço com o código '{codigo}'.");
            }

            var servico = Servico.Criar(nome, codigo, descricao, precoBase, tempoEstimado);

            _logger.LogInformation("Salvando serviço no repositório");

            var servicoCriado = await _servicoRepository.AddAsync(servico);

            return servico.ToModel();
        }

        public async Task DeletarAsync(int id)
        {
            _logger.LogInformation("Deletando serviço com ID {Id}", id);

            var servico = await _servicoRepository.GetByIdAsync(id);

            if (servico == null)
            {
                _logger.LogWarning("Serviço com ID {Id} não encontrado para deleção", id);
                throw new NotFoundException("Serviço", id);
            }

            servico.Deletar();

            await _servicoRepository.UpdateAsync(servico);
        }

        public async Task<ServicoModel> ObterPorCodigoAsync(string codigo)
        {
            _logger.LogInformation($"Obter serviço por código: {codigo}");

            var servico = await _servicoRepository.FindAsync(x => x.Codigo == codigo);

            if (!servico.Any())
            {
                _logger.LogWarning("Serviço com código {Codigo} não encontrado", codigo);
                throw new NotFoundException("Serviço", codigo);
            }

            var servicoUnico = servico.First();

            return servicoUnico.ToModel();
        }

        public async Task<List<ServicoModel>> ObterTodosAsync()
        {
            _logger.LogInformation("Obtendo todos os serviços");
            
            var servicos = await _servicoRepository.GetAllAsync();
            var servicoModels = new List<ServicoModel>();

            foreach (var servico in servicos)
            {
                servicoModels.Add(servico.ToModel());
            }

            return servicoModels;
        }
    }
}
