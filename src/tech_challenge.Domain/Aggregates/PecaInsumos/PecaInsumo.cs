using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Domain.Common.Entities;
using tech_challenge.Domain.Common.Enums;
using tech_challenge.Domain.Exceptions;

namespace tech_challenge.Domain.Aggregates.PecaInsumos
{
    public class PecaInsumo : Audit
    {
        public required string Nome { get; set; }
        public required string Codigo { get; set; }
        public string? Descricao { get; set; }
        public TipoPecaInsumo Tipo { get; set; }
        public required decimal PrecoUnitario { get; set; }
        public int QuantidadeEstoque { get; set; }

        public static PecaInsumo Criar(string nome, string codigo, string? descricao, TipoPecaInsumo tipo, decimal precoUnitario, int quantidadeEstoque)
        {
            var pecaInsumo =  new PecaInsumo
            {
                Nome = nome,
                Codigo = codigo,
                Descricao = descricao,
                Tipo = tipo,
                PrecoUnitario = precoUnitario,
                QuantidadeEstoque = quantidadeEstoque
            };

            pecaInsumo.Validar();

            return pecaInsumo;
        }

        public void Atualizar(string nome, string codigo, string? descricao, TipoPecaInsumo tipo, decimal precoUnitario, int quantidadeEstoque)
        {
            Nome = nome;
            Codigo = codigo;
            Descricao = descricao;
            Tipo = tipo;
            PrecoUnitario = precoUnitario;
        }

        public void AdiconarEstoque(int quantidade)
        {
            QuantidadeEstoque += quantidade;
        }

        public void BaixarEstoque(int quantidade)
        {
            if (quantidade > QuantidadeEstoque)
                throw new InvalidOperationException("Quantidade em estoque insuficiente.");
            QuantidadeEstoque -= quantidade;
        }

        public void Deletar()
        {
            Ativo = false;
        }

        public void Validar() 
        {
            if(string.IsNullOrWhiteSpace(Nome))
                throw new DomainException("O nome é obrigatório.");
            if (string.IsNullOrWhiteSpace(Codigo))
                throw new DomainException("O código é obrigatório.");
            if (PrecoUnitario < 0)
                throw new DomainException("O preço unitário não pode ser negativo.");
            if (QuantidadeEstoque < 0)
                throw new DomainException("A quantidade em estoque não pode ser negativa.");
        }
    }
}
