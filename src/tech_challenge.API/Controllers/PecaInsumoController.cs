using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tech_challenge.API.Controllers.Base;
using tech_challenge.API.Requests.PecaInsumos;
using tech_challenge.API.Requests.Veiculos;
using tech_challenge.API.Responses.PecaInsumos;
using tech_challenge.API.Responses.Veiculos;
using tech_challenge.Application.Interfaces.Services;
using tech_challenge.Application.Services.Veiculos;

namespace tech_challenge.API.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de Peças e Insumos: Consultas, Inclusão, Alteração e Exclusão.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Atendimento")]
    public class PecaInsumoController : BaseController<PecaInsumoController>
    {
        private readonly IPecaInsumoService _pecaInsumoService;
        public PecaInsumoController(IPecaInsumoService pecaInsumoService, ILogger<PecaInsumoController> logger) : base(logger)
        {
            _pecaInsumoService = pecaInsumoService;
        }

        /// <summary>
        /// Lista todas peças.
        /// </summary>
        /// <returns>Lista de peças.</returns>
        /// <response code="200">Lista de peças retornada com sucesso.</response>
        /// <response code="500">Erro interno do servidor.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<PecaInsumoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ListarPecas ()
        {
            _logger.LogInformation("Iniciando listagem de peças.");

            var result = await _pecaInsumoService.ObterTodosAsync();

            if (result == null || !result.Any())
            {
                _logger.LogInformation("Nenhuma peça encontrada.");
                return Ok(new List<PecaInsumoResponse>());
            }

            var response = result.Select(p => new PecaInsumoResponse
            {
                Id = p.Id,
                Nome = p.Nome,
                Codigo = p.Codigo,
                Descricao = p.Descricao,
                PrecoUnitario = p.PrecoUnitario,
                QuantidadeEstoque = p.QuantidadeEstoque
            }).ToList();

            return Ok(response);
        }

        /// <summary>
        /// Obter peça por código.
        /// </summary>
        /// <param name="request">Dados para busca de peça por código.</param>
        /// <returns>Retorna uma única peça.</returns>
        /// <response code="200">Peça retornada com sucesso.</response>
        /// <response code="404">Peça não encontrada.</response>
        /// <response code="500">Erro interno do servidor.</response>
        [HttpGet("por-codigo")]
        [ProducesResponseType(typeof(PecaInsumoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterPecaPorCodigo([FromQuery] ObterPecaPorCodigoRequest request)
        {
            _logger.LogInformation("Iniciando consulta de peça por código.");
            var result = await _pecaInsumoService.ObterPorCodigoAsync(request.Codigo);

            var response = new PecaInsumoResponse
            {
                Id = result.Id,
                Nome = result.Nome,
                Codigo = result.Codigo,
                Descricao = result.Descricao,
                PrecoUnitario = result.PrecoUnitario,
                QuantidadeEstoque = result.QuantidadeEstoque
            };

            return Ok(response);
        }

        /// <summary>
        /// Cadastra Nova Peça.
        /// </summary>
        /// <param name="request">Dados para cadastro da peça.</param>
        /// <returns>Peça cadastrada.</returns>
        /// <response code="201">Peça cadastrada com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="500">Erro interno do servidor.</response>
        [HttpPost]
        [ProducesResponseType(typeof(PecaInsumoResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CriarPeca([FromBody] CriarPecaInsumoRequest request)
        {
            _logger.LogInformation("Iniciando cadastro de nova peça.");
            var result = await _pecaInsumoService.CriarAsync(request.Nome, request.Codigo, request.Descricao, request.PrecoUnitario, request.QuantidadeEstoque);
            var response = new PecaInsumoResponse
            {
                Id = result.Id,
                Nome = result.Nome,
                Codigo = result.Codigo,
                Descricao = result.Descricao,
                PrecoUnitario = result.PrecoUnitario,
                QuantidadeEstoque = result.QuantidadeEstoque
            };

            return StatusCode(StatusCodes.Status201Created, response);
        }

        /// <summary>
        /// Deleta uma peça.
        /// </summary>
        /// <param name="id">Id da peça.</param>
        /// <returns>Resultado da operação.</returns>
        /// <response code="204">Peça deletada com sucesso.</response>
        /// <response code="404">Peça não encontrada.</response>
        /// <response code="500">Erro interno do servidor.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            _logger.LogInformation("Iniciando exclusão de peça.");
            await _pecaInsumoService.DeletarAsync(id);

            return NoContent();
        }
    }
}
