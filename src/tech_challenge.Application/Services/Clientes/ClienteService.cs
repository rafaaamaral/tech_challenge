using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Application.Exceptions;
using tech_challenge.Application.Interfaces.Repositories;
using tech_challenge.Application.Interfaces.Services;
using tech_challenge.Application.Services.Base;
using tech_challenge.Application.Services.Clientes.Model;
using tech_challenge.Domain.Aggregates.Clientes;
using tech_challenge.Domain.Aggregates.Usuarios;
using tech_challenge.Domain.Common.Enums;

namespace tech_challenge.Application.Services.Clientes
{
    public class ClienteService : ServiceBase<ClienteService>, IClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public ClienteService(IClienteRepository clienteRepository, IUsuarioRepository usuarioRepository, ILogger<ClienteService> logger) : base(logger)
        {
            _clienteRepository = clienteRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<ClienteModel> AddAsync(string nome, string documento, string email, string? telefone)
        {
            _logger.LogInformation("Adicionando cliente: {Nome}", nome);

            var cliente = Cliente.Criar(nome, documento, email, telefone);

            var clienteExistente = await _clienteRepository.FindAsync(c => c.Documento.Valor == cliente.Documento.Valor);

            if (clienteExistente.Any())
            {
                _logger.LogWarning("Cliente com documento: {Documento} já existe", documento);
                throw new InvalidOperationException($"Cliente com documento {documento} já existe.");
            }

            var result = await _clienteRepository.AddAsync(cliente);

            _logger.LogInformation("Cliente adicionado com UniqueCode: {UniqueCode}", result.UniqueCode);

            _logger.LogInformation("Criando usuário para o cliente com UniqueCode: {UniqueCode}", result.UniqueCode);

            var usuario = Usuario.Criar(cliente.UniqueCode, cliente.Nome, cliente.Email, PerfilUsuario.Cliente);

            await _usuarioRepository.AddAsync(usuario);

            return result.ToModel();
        }

        public async Task DeletarAsync(int id)
        {
            _logger.LogInformation("Deletando cliente com Id: {Id}", id);

            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null)
            {
                _logger.LogWarning("Cliente com Id: {Id} não encontrado", id);
                throw new NotFoundException("Cliente", id);
            }

            cliente.Delete();

            await _clienteRepository.UpdateAsync(cliente);
        }

        public async Task<List<ClienteModel>> ListarTodosAsync()
        {
            _logger.LogInformation("Listando todos os clientes");

            try
            {
                var clientes = await _clienteRepository.GetAllAsync();
                var clienteModels = new List<ClienteModel>();

                foreach (var cliente in clientes)
                {
                    clienteModels.Add(cliente.ToModel());
                }

                return clienteModels;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar clientes");
                throw;
            }
        }

        public async Task<ClienteModel> ObterPorDocumentoAsync(string documento)
        {
            _logger.LogInformation("Obtendo cliente por documento: {Documento}", documento);

            var cliente = await _clienteRepository.FindAsync(c => c.Documento.Valor == documento);
            if (!cliente.Any())
            {
                _logger.LogWarning("Cliente com documento: {Documento} não encontrado", documento);
                throw new NotFoundException("Cliente", documento);
            }

            var clienteEntity = cliente.First();

            return clienteEntity.ToModel();
        }
    }
}
