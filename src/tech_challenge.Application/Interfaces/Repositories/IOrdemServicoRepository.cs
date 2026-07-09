using tech_challenge.Application.Interfaces.Repositories.Base;
using tech_challenge.Application.Services.OrdemServicos.Model;
using tech_challenge.Domain.Aggregates.OrdemServicos;

namespace tech_challenge.Application.Interfaces.Repositories
{
    public interface IOrdemServicoRepository : IBaseRepository<OrdemServico>
    {
        Task<List<OrdemServico>> ListarComItensAsync();
        Task<OrdemServico?> ObterPorIdComItensAsync(int id);
        Task<OrdemServico?> ObterPorUniqueCodeAsync(Guid uniqueCode);
        Task<List<OrdemServico>> ListarPorClienteAsync(Guid clienteUniqueCode);
    }
}
