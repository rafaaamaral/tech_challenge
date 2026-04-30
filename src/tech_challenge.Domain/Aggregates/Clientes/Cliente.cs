using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using tech_challenge.Domain.Common.Entities;

namespace tech_challenge.Domain.Aggregates.Clientes
{
    public class Cliente : Audit
    {
        public required string Nome { get; set; }
        public required string Documento { get; set; }
        public required string Contato { get; set; }

        public void Add(Guid uniqueCode, string nome, string documento, string contato)
        {
            UniqueCode = uniqueCode;
            Nome = nome;
            Documento = documento;
            Contato = contato;

            Validate();
        }

        public void Update(string nome, string documento, string contato)
        {
            Nome = nome;
            Documento = documento;
            Contato = contato;
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Nome))
                throw new ArgumentException("O nome do cliente é obrigatório.");
            if (string.IsNullOrWhiteSpace(Documento))
                throw new ArgumentException("O documento do cliente é obrigatório.");
            if (string.IsNullOrWhiteSpace(Contato))
                throw new ArgumentException("O contato do cliente é obrigatório.");
        }
    }
}
