using Microsoft.AspNetCore.Mvc;
using tech_challenge.API.Controllers.Base;
using tech_challenge.API.Extensions;
using tech_challenge.API.Requests.Webhook;
using tech_challenge.Application.Interfaces.Services;

namespace tech_challenge.API.Controllers
{
    /// <summary>
    /// Controller responsável por integrações externas via webhook.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : BaseController<WebhookController>
    {
        private readonly IOrdemServicoService _ordemServicoService;
        private readonly IConfiguration _configuration;
        public WebhookController(IOrdemServicoService ordemServicoService,
            IConfiguration configuration,
            ILogger<WebhookController> logger) : base(logger)
        {
            _ordemServicoService = ordemServicoService;
            _configuration = configuration;
        }

        /// <summary>
        /// Alterar o status da ordem de serviço.
        /// </summary>
        [HttpPost("integracoes/orcamentos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AlterarStatusOrdemServico([FromBody] AlterarStatusOrcamentoRequest request)
        {
            if (!Request.IsValidWebhook(_configuration))
            {
                _logger.LogWarning("Webhook recebido com API Key inválida.");
                return Unauthorized();
            }

            _logger.LogInformation("Webhook recebido para Ordem de Serviço {UniqueCode} alterando status para {Status}.", request.UniqueCode, request.Status);

            await _ordemServicoService.AlterarStatusOrcamentoAsync(request.UniqueCode, request.Status);
            return NoContent();
        }
    }
}
