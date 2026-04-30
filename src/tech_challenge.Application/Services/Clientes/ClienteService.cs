using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Application.Interfaces.Repositories;
using tech_challenge.Application.Interfaces.Services;

namespace tech_challenge.Application.Services.Clientes
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task AddAsync(string nome, string documento, string contato)
        {
            var cliente = new Domain.Aggregates.Clientes.Cliente
            {
                UniqueCode = Guid.NewGuid(),
                Nome = nome,
                Documento = documento,
                Contato = contato
            };

            await _clienteRepository.AddAsync(cliente);
        }
    }
}
