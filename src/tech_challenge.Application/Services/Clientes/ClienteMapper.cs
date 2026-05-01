using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Application.Services.Clientes.Model;
using tech_challenge.Domain.Aggregates.Clientes;

namespace tech_challenge.Application.Services.Clientes
{
    public static class ClienteMapper
    {
        public static ClienteModel ToModel(this Cliente cliente)
        {
            return new ClienteModel
            {
                UniqueCode = cliente.UniqueCode,
                Nome = cliente.Nome,
                Documento = cliente.Documento.Valor,
                Email = cliente.Email,
                Telefone = cliente.Telefone,
                Ativo = cliente.Ativo
            };
        }
    }
}
