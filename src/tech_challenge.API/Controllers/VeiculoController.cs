using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tech_challenge.API.Controllers.Base;
using tech_challenge.API.Requests.Veiculos;
using tech_challenge.API.Responses.Veiculos;
using tech_challenge.Application.Interfaces.Services;
using tech_challenge.Application.Services.Clientes;

namespace tech_challenge.API.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de veículos: Consultas, Inclusão, Alteração e Exclusão.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Atendimento")]
    public class VeiculoController : BaseController<VeiculoController>
    {
        private readonly IVeiculoService _veiculoService;

        public VeiculoController(IVeiculoService veiculoService, ILogger<VeiculoController> logger) : base(logger)
        {
            _veiculoService = veiculoService;
        }

        /// <summary>
        /// Lista todos os veículos por cliente.
        /// </summary>
        /// <param name="request">Dados para listagem do veículo.</param>
        /// <returns>Lista de veículos.</returns>
        /// <response code="200">Lista de veículos retornada com sucesso.</response>
        /// <response code="500">Erro interno do servidor.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<VeiculoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ListarVeiculoPorCliente([FromQuery] ListaVeiculoRequest request)
        {
            _logger.LogInformation("Iniciando listagem de veículos para o cliente {ClienteId}", request.ClienteId);

            var result = await _veiculoService.ListarVeiculoPorClienteAsync(request.ClienteId);

            if (result == null || !result.Any())
            {
                _logger.LogInformation("Nenhum veículo encontrado para o cliente {ClienteId}", request.ClienteId);
                return Ok(new List<VeiculoResponse>());
            }

            var response = result.Select(v => new VeiculoResponse
            {
                Id = v.Id,
                UniqueCode = v.UniqueCode,
                Placa = v.Placa,
                Marca = v.Marca,
                Modelo = v.Modelo,
                Ano = v.Ano
            }).ToList();

            return Ok(response);
        }

        /// <summary>
        /// Obter Veículo por Placa e Cliente.
        /// </summary>
        /// <param name="request">Dados para busca de veículo por placa e cliente.</param>
        /// <returns>Retorna um unico veículo.</returns>
        /// <response code="200">Veículo retornado com sucesso.</response>
        /// <response code="404">Veículo não encontrado.</response>
        /// <response code="500">Erro interno do servidor.</response>
        [HttpGet("por-placa-cliente")]
        [ProducesResponseType(typeof(List<VeiculoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterVeiculoPorPlacaECliente([FromQuery] ObterVeiculoPorPlacaRequest request)
        {
            _logger.LogInformation("Iniciando consulta de veículo por placa e cliente.");

            var result = await _veiculoService.ObterVeiculoPorPlacaEClienteAsync(request.ClienteId, request.Placa);

            var response = new VeiculoResponse
            {
                Id = result.Id,
                UniqueCode = result.UniqueCode,
                Placa = result.Placa,
                Marca = result.Marca,
                Modelo = result.Modelo,
                Ano = result.Ano
            };

            return Ok(response);
        }

        /// <summary>
        /// Cadastra Novo Veículo.
        /// </summary>
        /// <param name="request">Dados para cadastro do veículo.</param>
        /// <returns>Veículo cadastrado.</returns>
        /// <response code="201">Veículo     cadastrado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="500">Erro interno do servidor.</response>
        [HttpPost]
        [ProducesResponseType(typeof(VeiculoResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CriarVeiculo([FromBody] CriarVeiculoRequest request)
        {
            _logger.LogInformation("Iniciando cadastro de novo veículo para o cliente {ClienteId}", request.ClienteId);
            var result = await _veiculoService.AddAsync(request.ClienteId, request.Placa, request.Marca, request.Modelo, request.Ano);
            var response = new VeiculoResponse
            {
                Id = result.Id,
                UniqueCode = result.UniqueCode,
                Placa = result.Placa,
                Marca = result.Marca,
                Modelo = result.Modelo,
                Ano = result.Ano
            };

            return StatusCode(StatusCodes.Status201Created, response);
        }

        /// <summary>
        /// Deleta um veículo.
        /// </summary>
        /// <param name="id">Id do veículo.</param>
        /// <returns>Resultado da operação.</returns>
        /// <response code="204">Veículo deletado com sucesso.</response>
        /// <response code="404">Veículo não encontrado.</response>
        /// <response code="500">Erro interno do servidor.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            _logger.LogInformation("Iniciando exclusão de veículo.");
            await _veiculoService.DeletarAsync(id);

            return NoContent();
        }
    }
}
