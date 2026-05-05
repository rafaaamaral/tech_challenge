using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tech_challenge.API.Controllers.Base;
using tech_challenge.API.Requests.OrdemServicos;
using tech_challenge.API.Responses.OrdemServicos;
using tech_challenge.Application.Interfaces.Services;
using tech_challenge.Application.Services.OrdemServicos.Model;

namespace tech_challenge.API.Controllers
{
    /// <summary>
    /// Controller responsável pelo fluxo de ordem de serviço.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Atendimento")]
    public class OrdemServicoController : BaseController<OrdemServicoController>
    {
        private readonly IOrdemServicoService _ordemServicoService;

        public OrdemServicoController(IOrdemServicoService ordemServicoService, ILogger<OrdemServicoController> logger) : base(logger)
        {
            _ordemServicoService = ordemServicoService;
        }

        /// <summary>
        /// Lista todas as ordens de serviço.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<OrdemServicoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ListarTodos()
        {
            _logger.LogInformation("Iniciando listagem de ordens de serviço.");

            var result = await _ordemServicoService.ListarTodosAsync();
            return Ok(result.Select(MapearResponse).ToList());
        }

        /// <summary>
        /// Obter ordem de serviço por Id.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrdemServicoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterPorId(int id)
        {
            _logger.LogInformation("Iniciando consulta de ordem de serviço por Id.");

            var result = await _ordemServicoService.ObterPorIdAsync(id);
            return Ok(MapearResponse(result));
        }

        /// <summary>
        /// Cria uma nova ordem de serviço.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(OrdemServicoResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Criar([FromBody] CriarOrdemServicoRequest request)
        {
            _logger.LogInformation("Iniciando criação de ordem de serviço.");

            var result = await _ordemServicoService.CriarAsync(
                request.ClienteId,
                request.VeiculoId,
                request.ItensServicos.Select(x => new CriarOrdemServicoItemServicoModel
                {
                    ServicoId = x.ServicoId
                }).ToList(),
                request.ItensPecasInsumos.Select(x => new CriarOrdemServicoItemPecaInsumoModel
                {
                    PecaInsumoId = x.PecaInsumoId,
                    Quantidade = x.Quantidade
                }).ToList());

            return StatusCode(StatusCodes.Status201Created, MapearResponse(result));
        }

        /// <summary>
        /// Inicia o diagnóstico da ordem de serviço.
        /// </summary>
        [HttpPatch("{id}/iniciar-diagnostico")]
        [ProducesResponseType(typeof(OrdemServicoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> IniciarDiagnostico(int id)
        {
            var result = await _ordemServicoService.IniciarDiagnosticoAsync(id);
            return Ok(MapearResponse(result));
        }

        /// <summary>
        /// Gera o orçamento e coloca a ordem de serviço aguardando aprovação.
        /// </summary>
        [HttpPatch("{id}/gerar-orcamento")]
        [ProducesResponseType(typeof(OrdemServicoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GerarOrcamento(int id)
        {
            var result = await _ordemServicoService.GerarOrcamentoAsync(id);
            return Ok(MapearResponse(result));
        }

        /// <summary>
        /// Aprova o orçamento e inicia a execução da ordem de serviço.
        /// </summary>
        [HttpPatch("{id}/aprovar-orcamento")]
        [ProducesResponseType(typeof(OrdemServicoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AprovarOrcamento(int id)
        {
            var result = await _ordemServicoService.AprovarOrcamentoAsync(id);
            return Ok(MapearResponse(result));
        }

        /// <summary>
        /// Reprova o orçamento da ordem de serviço.
        /// </summary>
        [HttpPatch("{id}/reprovar-orcamento")]
        [ProducesResponseType(typeof(OrdemServicoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ReprovarOrcamento(int id)
        {
            var result = await _ordemServicoService.ReprovarOrcamentoAsync(id);
            return Ok(MapearResponse(result));
        }

        /// <summary>
        /// Inicia a execução da ordem de serviço aprovada.
        /// </summary>
        [HttpPatch("{id}/iniciar-execucao")]
        [ProducesResponseType(typeof(OrdemServicoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> IniciarExecucao(int id)
        {
            var result = await _ordemServicoService.IniciarExecucaoAsync(id);
            return Ok(MapearResponse(result));
        }

        /// <summary>
        /// Finaliza a execução da ordem de serviço.
        /// </summary>
        [HttpPatch("{id}/finalizar-execucao")]
        [ProducesResponseType(typeof(OrdemServicoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FinalizarExecucao(int id)
        {
            var result = await _ordemServicoService.FinalizarExecucaoAsync(id);
            return Ok(MapearResponse(result));
        }

        /// <summary>
        /// Registra a entrega do veículo.
        /// </summary>
        [HttpPatch("{id}/entregar")]
        [ProducesResponseType(typeof(OrdemServicoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Entregar(int id)
        {
            var result = await _ordemServicoService.EntregarAsync(id);
            return Ok(MapearResponse(result));
        }

        /// <summary>
        /// Deleta uma ordem de serviço.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Deletar(int id)
        {
            await _ordemServicoService.DeletarAsync(id);
            return NoContent();
        }

        private static OrdemServicoResponse MapearResponse(OrdemServicoModel model)
        {
            return new OrdemServicoResponse
            {
                Id = model.Id,
                UniqueCode = model.UniqueCode,
                ClienteId = model.ClienteId,
                VeiculoId = model.VeiculoId,
                Numero = model.Numero,
                Status = model.Status.ToString(),
                InicioExecucao = model.InicioExecucao,
                FimExecucao = model.FimExecucao,
                Orcamento = new OrcamentoResponse
                {
                    ValorServicos = model.Orcamento.ValorServicos,
                    ValorPecasInsumos = model.Orcamento.ValorPecasInsumos,
                    ValorTotal = model.Orcamento.ValorTotal,
                    Status = model.Orcamento.Status.ToString()
                },
                ItensServicos = model.ItensServicos.Select(x => new OrdemServicoItemServicoResponse
                {
                    Id = x.Id,
                    ServicoId = x.ServicoId,
                    Descricao = x.Descricao,
                    Valor = x.Valor
                }).ToList(),
                ItensPecasInsumos = model.ItensPecasInsumos.Select(x => new OrdemServicoItemPecaInsumoResponse
                {
                    Id = x.Id,
                    PecaInsumoId = x.PecaInsumoId,
                    Descricao = x.Descricao,
                    Quantidade = x.Quantidade,
                    ValorUnitario = x.ValorUnitario,
                    ValorTotal = x.ValorTotal
                }).ToList()
            };
        }
    }
}
