using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tech_challenge.API.Controllers.Base;
using tech_challenge.API.Model.Clientes;
using tech_challenge.API.Requests.Clientes;
using tech_challenge.API.Responses.Clientes;
using tech_challenge.Application.Interfaces.Services;

namespace tech_challenge.API.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de clientes: Consultas, Inclusão, Alteração e Exclusão.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Atendimento")]
    public class ClienteController : BaseController<ClienteController>
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService, ILogger<ClienteController> logger) : base(logger)
        {
            _clienteService = clienteService;
        }

        /// <summary>
        /// Lista todos os clientes.
        /// </summary>
        /// <returns>Lista de clientes.</returns>
        /// <response code="200">Lista de clientes retornada com sucesso.</response>
        /// <response code="500">Erro interno do servidor.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<ListaClienteResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ListarTodos()
        {
            _logger.LogInformation("Iniciando listagem de clientes.");

            var result = await _clienteService.ListarTodosAsync();

            var response = result.Select(c => new ListaClienteResponse
            {
                Id = c.Id,
                UniqueCode = c.UniqueCode,
                Nome = c.Nome,
                Documento = c.Documento
            }).ToList();

            return Ok(response);
        }

        /// <summary>
        /// Obter Cliente por Documento.
        /// </summary>
        /// <param name="request">Dados para consultar cliente.</param>
        /// <returns>Cliente encontrado.</returns>
        /// <response code="200">Cliente retornado com sucesso.</response>
        /// <response code="404">Cliente não encontrado.</response>
        /// <response code="500">Erro interno do servidor.</response>
        [HttpGet("por-documento")]
        [ProducesResponseType(typeof(ObterClientePorDocumentoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterPorDocumento([FromQuery] ObterClientePorDocumentoRequest request)
        {
            _logger.LogInformation("Iniciando consulta de cliente por documento.");

            var result = await _clienteService.ObterPorDocumentoAsync(request.Documento);

            var response = new ObterClientePorDocumentoResponse
            {
                UniqueCode = result.UniqueCode,
                Nome = result.Nome,
                Documento = result.Documento
            };

            return Ok(response);
        }

        /// <summary>
        /// Cadastra Novo Cliente.
        /// </summary>
        /// <param name="request">Dados para cadastro do cliente.</param>
        /// <returns>Cliente cadastrado.</returns>
        /// <response code="201">Cliente cadastrado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="500">Erro interno do servidor.</response>
        [HttpPost]
        [ProducesResponseType(typeof(CriarClienteResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Criar([FromBody] CriarClienteRequest request)
        {
            _logger.LogInformation("Iniciando criação de cliente.");

            var result = await _clienteService.AddAsync(request.Nome, request.Documento, request.Email, request.Telefone);

            var response = new CriarClienteResponse
            {
                UniqueCode = result.UniqueCode,
                Nome = result.Nome,
                Documento = result.Documento
            };

            return StatusCode(StatusCodes.Status201Created, response);
        }

        /// <summary>
        /// Deleta um cliente.
        /// </summary>
        /// <param name="id">Id do cliente.</param>
        /// <returns>Resultado da operação.</returns>
        /// <response code="204">Cliente deletado com sucesso.</response>
        /// <response code="404">Cliente não encontrado.</response>
        /// <response code="500">Erro interno do servidor.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            _logger.LogInformation("Iniciando exclusão de cliente.");
            await _clienteService.DeletarAsync(id);

            return NoContent();
        }
    }
}