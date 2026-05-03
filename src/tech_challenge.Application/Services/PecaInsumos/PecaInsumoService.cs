using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Application.Exceptions;
using tech_challenge.Application.Interfaces.Repositories;
using tech_challenge.Application.Interfaces.Services;
using tech_challenge.Application.Services.Base;
using tech_challenge.Application.Services.PecaInsumos.Model;
using tech_challenge.Domain.Aggregates.PecaInsumos;
using tech_challenge.Domain.Common.Enums;

namespace tech_challenge.Application.Services.PecaInsumos
{
    public class PecaInsumoService : ServiceBase<PecaInsumoService>, IPecaInsumoService
    {
        private readonly IPecaInsumoRepository _pecaInsumoRepository;
        public PecaInsumoService(IPecaInsumoRepository pecaInsumoRepository, ILogger<PecaInsumoService> logger) : base(logger)
        {
            _pecaInsumoRepository = pecaInsumoRepository;
        }

        public async Task<PecaInsumoModel> CriarAsync(string nome, string codigo, string? descricao, decimal precoUnitario, int quantidadeEstoque)
        {
            _logger.LogInformation("Criando insumo de peça: {Nome} com código {Codigo}", nome, codigo);

            var insumoExistente = await _pecaInsumoRepository.FindAsync(x => x.Codigo == codigo);

            if (insumoExistente.Any())
            {
                _logger.LogWarning("Insumo de peça com código {Codigo} já existe", codigo);
                throw new InvalidOperationException($"Insumo de peça com código {codigo} já existe.");
            }

            var pecaInsumo = PecaInsumo.Criar(nome, codigo, descricao, TipoPecaInsumo.Peca, precoUnitario, quantidadeEstoque);

            await _pecaInsumoRepository.AddAsync(pecaInsumo);

            return pecaInsumo.ToModel();
        }

        public async Task DeletarAsync(int id)
        {
            _logger.LogInformation("Deletando insumo de peça com ID {Id}", id);

            var pecaInsumo = await _pecaInsumoRepository.GetByIdAsync(id);

            if (pecaInsumo == null)
            {
                _logger.LogWarning("Insumo de peça com ID {Id} não encontrado", id);
                throw new NotFoundException("Insumo de peça", id);
            }

            pecaInsumo.Deletar();

            await _pecaInsumoRepository.UpdateAsync(pecaInsumo);
        }

        public async Task<PecaInsumoModel> ObterPorCodigoAsync(string codigo)
        {
            _logger.LogInformation($"Obter insumo de peça por código: {codigo}");

            var insumo = await _pecaInsumoRepository.FindAsync(x => x.Codigo == codigo);

            if (!insumo.Any())
            {
                _logger.LogWarning("Insumo de peça com código {Codigo} não encontrado", codigo);
                throw new NotFoundException("Insumo de peça", codigo);
            }

            var insumoUnico = insumo.First();
            return insumoUnico.ToModel();
        }

        public async Task<List<PecaInsumoModel>> ObterTodosAsync()
        {
            _logger.LogInformation("Obtendo todos os insumos de peças.");
            var insumos = await _pecaInsumoRepository.GetAllAsync();

            var insumoModels = new List<PecaInsumoModel>();
            foreach (var insumo in insumos)
            {
                insumoModels.Add(insumo.ToModel());
            }

            return insumoModels;
        }
    }
}
