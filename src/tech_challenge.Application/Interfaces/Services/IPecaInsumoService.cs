using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Application.Services.PecaInsumos.Model;

namespace tech_challenge.Application.Interfaces.Services
{
    public interface IPecaInsumoService
    {
        Task<List<PecaInsumoModel>> ObterTodosAsync();
        Task<PecaInsumoModel> CriarAsync(string nome, string codigo, string? descricao, decimal precoUnitario, int quantidadeEstoque);
        Task DeletarAsync(int id);
        Task<PecaInsumoModel> ObterPorCodigoAsync(string codigo);
    }
}
