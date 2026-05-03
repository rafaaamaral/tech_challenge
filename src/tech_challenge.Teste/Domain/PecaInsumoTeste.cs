using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Domain.Aggregates.PecaInsumos;
using tech_challenge.Domain.Common.Enums;
using tech_challenge.Domain.Exceptions;

namespace tech_challenge.Teste.Domain
{
    public class PecaInsumoTeste
    {
        [Fact]
        public void Deve_Criar_PecaInsumo_Valida()
        {
            var pecaInsumo = PecaInsumo.Criar(
                "Pneu aro 15",
                "001",
                "Pneu de Carro",
                TipoPecaInsumo.Insumo,
                0.1M,
                2
            );
            pecaInsumo.UniqueCode.Should().NotBeEmpty();
            pecaInsumo.Nome.Should().Be("Pneu aro 15");
            pecaInsumo.Descricao.Should().Be("Pneu de Carro");
        }

        [Fact]
        public void Deve_Atualizar_PecaInsumo()
        {
            var pecaInsumo = PecaInsumo.Criar(
                "Pneu aro 15",
                "001",
                "Pneu de Carro",
                TipoPecaInsumo.Insumo,
                0.1M,
                2
            );
            pecaInsumo.Atualizar(
                "Pneu aro 16",
                "002",
                "Pneu de Carro Atualizado",
                TipoPecaInsumo.Peca,
                0.2M,
                3
            );
            pecaInsumo.Nome.Should().Be("Pneu aro 16");
            pecaInsumo.Descricao.Should().Be("Pneu de Carro Atualizado");
        }

        [Fact]
        public void Deve_Deletar_PecaInsumo()
        {
            var pecaInsumo = PecaInsumo.Criar(
                "Pneu aro 15",
                "001",
                "Pneu de Carro",
                TipoPecaInsumo.Insumo,
                0.1M,
                2
            );
            pecaInsumo.Deletar();
            pecaInsumo.Ativo.Should().Be(false);
        }

        [Fact]
        public void Deve_Lancar_Exception_Quando_Nome_For_Vazio()
        {
            var act = () => PecaInsumo.Criar(
                "",
                "001",
                "Pneu de Carro",
                TipoPecaInsumo.Insumo,
                0.1M,
                2
            );
            act.Should()
               .Throw<DomainException>()
               .WithMessage("O nome é obrigatório.");
        }

        [Fact]
        public void Deve_Lancar_Exception_Quando_Codigo_For_Vazio()
        {
            var act = () => PecaInsumo.Criar(
                "Pneu aro 15",
                "",
                "Pneu de Carro",
                TipoPecaInsumo.Insumo,
                0.1M,
                2
            );
            act.Should()
               .Throw<DomainException>()
               .WithMessage("O código é obrigatório.");
        }

        [Fact]
        public void Deve_Lancar_Exception_Quando_PrecoUnitario_For_Negativo()
        {
            var act = () => PecaInsumo.Criar(
                "Pneu aro 15",
                "001",
                "Pneu de Carro",
                TipoPecaInsumo.Insumo,
                -0.1M,
                2
            );
            act.Should()
               .Throw<DomainException>()
               .WithMessage("O preço unitário não pode ser negativo.");
        }

        [Fact]
        public void Deve_Lancar_Exception_Quando_QuantidadeEstoque_For_Negativa()
        {
            var act = () => PecaInsumo.Criar(
                "Pneu aro 15",
                "001",
                "Pneu de Carro",
                TipoPecaInsumo.Insumo,
                0.1M,
                -2
            );
            act.Should()
               .Throw<DomainException>()
               .WithMessage("A quantidade em estoque não pode ser negativa.");
        }

        [Fact]
        public void Deve_Adicionar_Estoque()
        {
            var pecaInsumo = PecaInsumo.Criar(
                "Pneu aro 15",
                "001",
                "Pneu de Carro",
                TipoPecaInsumo.Insumo,
                0.1M,
                2
            );
            pecaInsumo.AdiconarEstoque(3);
            pecaInsumo.QuantidadeEstoque.Should().Be(5);
        }

        [Fact]
        public void Deve_Baixar_Estoque()
        {
            var pecaInsumo = PecaInsumo.Criar(
                "Pneu aro 15",
                "001",
                "Pneu de Carro",
                TipoPecaInsumo.Insumo,
                0.1M,
                5
            );
            pecaInsumo.BaixarEstoque(3);
            pecaInsumo.QuantidadeEstoque.Should().Be(2);
        }

        [Fact]
        public void Deve_Lancar_Exception_Quando_Baixar_Estoque_Maior_Que_Quantidade()
        {
            var pecaInsumo = PecaInsumo.Criar(
                "Pneu aro 15",
                "001",
                "Pneu de Carro",
                TipoPecaInsumo.Insumo,
                0.1M,
                2
            );
            var act = () => pecaInsumo.BaixarEstoque(3);
            act.Should()
               .Throw<InvalidOperationException>()
               .WithMessage("Quantidade em estoque insuficiente.");
        }
    }
}
