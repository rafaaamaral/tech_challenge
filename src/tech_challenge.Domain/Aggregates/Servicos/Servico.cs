using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Domain.Common.Entities;
using tech_challenge.Domain.Exceptions;

namespace tech_challenge.Domain.Aggregates.Servicos
{
    public class Servico : Audit
    {
        public required string Nome { get; set; }
        public required string Codigo { get; set; }
        public string? Descricao { get; set; }
        public required decimal PrecoBase { get; set; } = 0;
        public int? TempoEstimado { get; set; }

        public static Servico Criar(string nome, string codigo, string? descricao, decimal precoBase, int? tempoEstimado)
        {
            var servico = new Servico
            {
                Nome = nome,
                Codigo = codigo,
                Descricao = descricao,
                PrecoBase = precoBase,
                TempoEstimado = tempoEstimado
            };
            servico.Validar();
            return servico;
        }

        public void Atualizar(string nome, string codigo, string? descricao, decimal precoBase, int? tempoEstimado)
        {
            Nome = nome;
            Codigo = codigo;
            Descricao = descricao;
            PrecoBase = precoBase;
            TempoEstimado = tempoEstimado;
            Validar();
        }

        public void Deletar()
        {
            Ativo = false;
        }

        public void Validar()
        {
            if (string.IsNullOrWhiteSpace(Nome))
                throw new DomainException("O nome é obrigatório.");
            if (string.IsNullOrWhiteSpace(Codigo))
                throw new DomainException("O código é obrigatório.");
            if (PrecoBase < 0)
                throw new DomainException("O preço base não pode ser negativo.");
            if (TempoEstimado.HasValue && TempoEstimado.Value < 0)
                throw new DomainException("O tempo estimado não pode ser negativo.");
        }
    }
}
