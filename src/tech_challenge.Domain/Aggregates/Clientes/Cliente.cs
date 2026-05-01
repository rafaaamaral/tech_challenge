using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using tech_challenge.Domain.Aggregates.Veiculos;
using tech_challenge.Domain.Common.Entities;
using tech_challenge.Domain.Exceptions;

namespace tech_challenge.Domain.Aggregates.Clientes
{
    public class Cliente : Audit
    {
        public Cliente() { }

        public required string Nome { get; set; }
        public required Documento Documento { get; set; }
        public required string Email { get; set; }
        public string? Telefone { get; set; } = null;

        public static Cliente Criar(string nome, string documento, string email, string? telefone)
        {
            var cliente = new Cliente
            {
                UniqueCode = Guid.NewGuid(),
                Nome = nome,
                Documento = new Documento(documento),
                Email = email,
                Telefone = telefone
            };

            cliente.Validate();
            return cliente;
        }

        public void Delete()
        {
            Ativo = false;
        }

        public void Alterar(string nome, string documento, string email, string? telefone)
        {
            Nome = nome;
            Documento = new Documento(documento);
            Email = email;
            Telefone = telefone;
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Nome))
                throw new DomainException("O nome do cliente é obrigatório.");
            if (string.IsNullOrWhiteSpace(Email))
                throw new DomainException("O email do cliente é obrigatório.");
        }
    }
}
