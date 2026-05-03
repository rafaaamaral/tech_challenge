using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Application.Services.Veiculos.Model;

namespace tech_challenge.Application.Interfaces.Services
{
    public interface IVeiculoService
    {
        Task<List<VeiculoModel>> ListarVeiculoPorClienteAsync(int clienteId);
        Task<VeiculoModel> ObterVeiculoPorPlacaEClienteAsync(int clienteId, string placa);
        Task<VeiculoModel> AddAsync(int clienteId, string placa, string marca, string modelo, int? ano);
        Task DeletarAsync(int veiculoId);
    }
}
