using Microsoft.Extensions.Logging;
using tech_challenge.Application.Interfaces.Services;
using tech_challenge.Application.Observability;
using tech_challenge.Application.Services.OrdemServicos.Model;
using tech_challenge.Domain.Common.Enums;

namespace tech_challenge.Application.Services.OrdemServicos;

public sealed class ObservedOrdemServicoService(
    OrdemServicoService inner,
    ILogger<ObservedOrdemServicoService> logger) : IOrdemServicoService
{
    public Task<OrdemServicoModel> CriarAsync(int clienteId, int veiculoId,
        List<CriarOrdemServicoItemServicoModel> itensServicos,
        List<CriarOrdemServicoItemPecaInsumoModel> itensPecasInsumos) =>
        OrdemServicoTelemetry.ExecuteAsync("criar", () => inner.CriarAsync(clienteId, veiculoId, itensServicos, itensPecasInsumos), logger);

    public Task<List<OrdemServicoModel>> ListarTodosAsync() =>
        OrdemServicoTelemetry.ExecuteAsync("listar", inner.ListarTodosAsync, logger);

    public Task<OrdemServicoModel> ObterPorIdAsync(int id) =>
        OrdemServicoTelemetry.ExecuteAsync("obter", () => inner.ObterPorIdAsync(id), logger, id);

    public Task<OrdemServicoModel> IniciarDiagnosticoAsync(int id) =>
        OrdemServicoTelemetry.ExecuteAsync("iniciar_diagnostico", () => inner.IniciarDiagnosticoAsync(id), logger, id);

    public Task<OrdemServicoModel> GerarOrcamentoAsync(int id) =>
        OrdemServicoTelemetry.ExecuteAsync("gerar_orcamento", () => inner.GerarOrcamentoAsync(id), logger, id);

    public Task<OrdemServicoModel> AprovarOrcamentoAsync(int id) =>
        OrdemServicoTelemetry.ExecuteAsync("aprovar_orcamento", () => inner.AprovarOrcamentoAsync(id), logger, id);

    public Task<OrdemServicoModel> ReprovarOrcamentoAsync(int id) =>
        OrdemServicoTelemetry.ExecuteAsync("reprovar_orcamento", () => inner.ReprovarOrcamentoAsync(id), logger, id);

    public Task<OrdemServicoModel> IniciarExecucaoAsync(int id) =>
        OrdemServicoTelemetry.ExecuteAsync("iniciar_execucao", () => inner.IniciarExecucaoAsync(id), logger, id);

    public Task<OrdemServicoModel> FinalizarExecucaoAsync(int id) =>
        OrdemServicoTelemetry.ExecuteAsync("finalizar_execucao", () => inner.FinalizarExecucaoAsync(id), logger, id);

    public Task<OrdemServicoModel> EntregarAsync(int id) =>
        OrdemServicoTelemetry.ExecuteAsync("entregar", () => inner.EntregarAsync(id), logger, id);

    public Task DeletarAsync(int id) =>
        OrdemServicoTelemetry.ExecuteAsync("deletar", () => inner.DeletarAsync(id), logger, id);

    public Task<OrdemServicoModel> ConsultarStatusAsync(Guid uniqueCode) =>
        OrdemServicoTelemetry.ExecuteAsync("consultar_status", () => inner.ConsultarStatusAsync(uniqueCode), logger);

    public Task<List<OrdemServicoModel>> ListarPorClienteAsync() =>
        OrdemServicoTelemetry.ExecuteAsync("listar_cliente", inner.ListarPorClienteAsync, logger);

    public Task AlterarStatusOrcamentoAsync(Guid uniqueCode, StatusOrcamento status) =>
        OrdemServicoTelemetry.ExecuteAsync("alterar_status_orcamento", () => inner.AlterarStatusOrcamentoAsync(uniqueCode, status), logger);
}
