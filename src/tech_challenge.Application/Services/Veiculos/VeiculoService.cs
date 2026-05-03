using Microsoft.Extensions.Logging;
using tech_challenge.Application.Exceptions;
using tech_challenge.Application.Interfaces.Repositories;
using tech_challenge.Application.Interfaces.Services;
using tech_challenge.Application.Services.Base;
using tech_challenge.Application.Services.Veiculos.Model;
using tech_challenge.Domain.Aggregates.Veiculos;

namespace tech_challenge.Application.Services.Veiculos
{
    public class VeiculoService : ServiceBase<VeiculoService>, IVeiculoService
    {
        private readonly IVeiculoRepository _veiculoRepository;
        public VeiculoService(IVeiculoRepository veiculoRepository, ILogger<VeiculoService> logger) : base(logger)
        {
            _veiculoRepository = veiculoRepository;
        }

        public async Task<VeiculoModel> AddAsync(int clienteId, string placa, string marca, string modelo, int? ano)
        {
            _logger.LogInformation("Iniciando processo de adição de veículo para o cliente {ClienteId} com placa {Placa}", clienteId, placa);

            var veiculo = Veiculo.Criar(clienteId, placa, marca, modelo, ano);

            var veiculoExistente = await _veiculoRepository.FindAsync(x => x.Placa.Valor == veiculo.Placa.Valor);

            if (veiculoExistente.Any())
            {
                _logger.LogWarning("Veículo com placa {Placa} já existe", placa);
                throw new InvalidOperationException($"Veículo com placa {placa} já existe.");
            }

            var veiculoAdicionado = _veiculoRepository.AddAsync(veiculo).Result;

            _logger.LogInformation("Veículo adicionado com sucesso para o cliente {ClienteId} com placa {Placa}", clienteId, placa);

            return veiculo.ToModel();   
        }

        public async Task DeletarAsync(int veiculoId)
        {
            var veiculo = _veiculoRepository.GetByIdAsync(veiculoId).Result;
            if (veiculo == null)
            {
                _logger.LogWarning("Veículo com ID {VeiculoId} não encontrado para exclusão", veiculoId);
                throw new NotFoundException("Veículo", veiculoId);
            }

            veiculo.Delete();

            await _veiculoRepository.UpdateAsync(veiculo);
        }

        public async Task<List<VeiculoModel>> ListarVeiculoPorClienteAsync(int clienteId)
        {
            _logger.LogInformation("Iniciando consulta de veículos para o cliente {ClienteId}", clienteId);

            var veiculos = await _veiculoRepository.FindAsync(x => x.ClienteId == clienteId);
            var veiculoModels = new List<VeiculoModel>();

            foreach (var veiculo in veiculos)
            {
                veiculoModels.Add(veiculo.ToModel());
            }

            return veiculoModels;
        }

        public async Task<VeiculoModel> ObterVeiculoPorPlacaEClienteAsync(int clienteId, string placa)
        {
            var veiculo = await _veiculoRepository.FindAsync(x => x.ClienteId == clienteId 
            && x.Placa.Valor.ToUpper() == placa.ToUpper());



            if (!veiculo.Any())
            {
                _logger.LogWarning("Veículo com placa {Placa} para o cliente {ClienteId} não encontrado", placa, clienteId);
                throw new NotFoundException("Veículo", $"Placa: {placa}, ClienteId: {clienteId}");
            }

            var veiculoUnico = veiculo.First();
            return veiculoUnico.ToModel();
        }
    }
}
