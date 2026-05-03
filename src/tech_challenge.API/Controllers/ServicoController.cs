using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tech_challenge.API.Controllers.Base;
using tech_challenge.API.Requests.Servicos;
using tech_challenge.API.Responses.Servicos;
using tech_challenge.Application.Interfaces.Services;

namespace tech_challenge.API.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de serviços: Consultas, Inclusão, Alteração e Exclusão.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Atendimento")]
    public class ServicoController : BaseController<ServicoController>
    {
        private readonly IServicoService _servicoService;
        public ServicoController(IServicoService servicoService, ILogger<ServicoController> logger) : base(logger)
        {
            _servicoService = servicoService;
        }

        /// <summary>
        /// Lista todos serviços.
        /// </summary>
        /// <returns>Lista de serviços.</returns>
        /// <response code="200">Lista de serviços retornada com sucesso.</response>
        /// <response code="500">Erro interno do servidor.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<ServicoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ListarServicos()
        {
            _logger.LogInformation("Iniciando listagem de serviços.");

            var result = await _servicoService.ObterTodosAsync();
            if (result == null || !result.Any())
            {
                _logger.LogInformation("Nenhum serviço encontrado.");
                return Ok(new List<ServicoResponse>());
            }

            var response = result.Select(p => new ServicoResponse
            {
                Id = p.Id,
                Nome = p.Nome,
                Codigo = p.Codigo,
                Descricao = p.Descricao,
                PrecoBase = p.PrecoBase,
                TempoEstimado = p.TempoEstimado
            }).ToList();

            return Ok(response);
        }

        /// <summary>
        /// Obter serviço por código.
        /// </summary>
        /// <param name="request">Dados para busca de serviço por código.</param>
        /// <returns>Retorna um único serviço.</returns>
        /// <response code="200">Serviço retornado com sucesso.</response>
        /// <response code="404">Serviço não encontrado.</response>
        /// <response code="500">Erro interno do servidor.</response>
        [HttpGet("por-codigo")]
        [ProducesResponseType(typeof(ServicoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterServicoPorCodigo([FromQuery] ObterServicoPorCodigoRequest request)
        {
            _logger.LogInformation("Iniciando consulta de serviço por código.");
            var result = await _servicoService.ObterPorCodigoAsync(request.Codigo);

            var response = new ServicoResponse
            {
                Id = result.Id,
                Nome = result.Nome,
                Codigo = result.Codigo,
                Descricao = result.Descricao,
                PrecoBase = result.PrecoBase,
                TempoEstimado = result.TempoEstimado
            };

            return Ok(response);
        }

        /// <summary>
        /// Cadastra Novo Serviço.
        /// </summary>
        /// <param name="request">Dados para cadastro do serviço.</param>
        /// <returns>Serviço cadastrado.</returns>
        /// <response code="201">Serviço cadastrado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="500">Erro interno do servidor.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ServicoResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CriarServico([FromBody] CriarServicoRequest request)
        {
            _logger.LogInformation("Iniciando cadastro de novo serviço.");
            var result = await _servicoService.CriarAsync(request.Nome, request.Codigo, request.Descricao, request.PrecoBase, request.TempoEstimado);
            var response = new ServicoResponse
            {
                Id = result.Id,
                Nome = result.Nome,
                Codigo = result.Codigo,
                Descricao = result.Descricao,
                PrecoBase = result.PrecoBase,
                TempoEstimado = result.TempoEstimado
            };

            return StatusCode(StatusCodes.Status201Created, response);
        }

        /// <summary>
        /// Deleta um serviço.
        /// </summary>
        /// <param name="id">Id do serviço.</param>
        /// <returns>Resultado da operação.</returns>
        /// <response code="204">Serviço deletado com sucesso.</response>
        /// <response code="404">Serviço não encontrado.</response>
        /// <response code="500">Erro interno do servidor.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            _logger.LogInformation("Iniciando exclusão de serviço.");
            await _servicoService.DeletarAsync(id);

            return NoContent();
        }
    }
}
