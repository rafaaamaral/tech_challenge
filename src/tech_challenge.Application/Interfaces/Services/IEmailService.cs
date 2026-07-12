using tech_challenge.Application.Services.OrdemServicos.Model;

namespace tech_challenge.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task EnviarEmailAprovacaoOrcamentoAsync(OrdemServicoModel ordemServico, CancellationToken cancellationToken = default);
    }
}
