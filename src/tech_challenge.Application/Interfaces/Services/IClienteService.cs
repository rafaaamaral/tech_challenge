using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Application.Services.Clientes.Model;

namespace tech_challenge.Application.Interfaces.Services
{
    public interface IClienteService
    {
        Task<ClienteModel> AddAsync(string nome, string documento, string email, string? telefone);
        Task<List<ClienteModel>> ListarTodosAsync();
        Task<ClienteModel> ObterPorDocumentoAsync(string documento);
        Task DeletarAsync(int id);
    }
}
