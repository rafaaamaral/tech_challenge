using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Application.Services.Servicos.Model;

namespace tech_challenge.Application.Interfaces.Services
{
    public interface IServicoService
    {
        Task<List<ServicoModel>> ObterTodosAsync();
        Task<ServicoModel> CriarAsync(string nome, string codigo, string? descricao, decimal precoBase, int? tempoEstimado);
        Task DeletarAsync(int id);
        Task<ServicoModel> ObterPorCodigoAsync(string codigo);
    }
}
