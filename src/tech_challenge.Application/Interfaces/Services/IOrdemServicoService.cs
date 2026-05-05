using tech_challenge.Application.Services.OrdemServicos.Model;

namespace tech_challenge.Application.Interfaces.Services
{
    public interface IOrdemServicoService
    {
        Task<OrdemServicoModel> CriarAsync(
            int clienteId,
            int veiculoId,
            List<CriarOrdemServicoItemServicoModel> itensServicos,
            List<CriarOrdemServicoItemPecaInsumoModel> itensPecasInsumos);

        Task<List<OrdemServicoModel>> ListarTodosAsync();
        Task<OrdemServicoModel> ObterPorIdAsync(int id);
        Task<OrdemServicoModel> IniciarDiagnosticoAsync(int id);
        Task<OrdemServicoModel> GerarOrcamentoAsync(int id);
        Task<OrdemServicoModel> AprovarOrcamentoAsync(int id);
        Task<OrdemServicoModel> ReprovarOrcamentoAsync(int id);
        Task<OrdemServicoModel> IniciarExecucaoAsync(int id);
        Task<OrdemServicoModel> FinalizarExecucaoAsync(int id);
        Task<OrdemServicoModel> EntregarAsync(int id);
        Task DeletarAsync(int id);
    }
}
