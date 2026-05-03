using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Domain.Aggregates.Servicos;
using tech_challenge.Domain.Exceptions;

namespace tech_challenge.Teste.Domain
{
    public class ServicoTeste
    {
        [Fact]
        public void Deve_Criar_Servico_Valido()
        {
            var servico = Servico.Criar(
                "Troca de Óleo",
                "001",
                "Serviço de troca de óleo para carros",
                0.5M,
                1
            );
            servico.UniqueCode.Should().NotBeEmpty();
            servico.Nome.Should().Be("Troca de Óleo");
            servico.Descricao.Should().Be("Serviço de troca de óleo para carros");
        }

        [Fact]
        public void Deve_Atualizar_Servico()
        {
            var servico = Servico.Criar(
                "Troca de Óleo",
                "001",
                "Serviço de troca de óleo para carros",
                0.5M,
                1
            );
            servico.Atualizar(
                "Troca de Óleo Premium",
                "002",
                "Serviço de troca de óleo premium para carros",
                0.7M,
                2
            );
            servico.Nome.Should().Be("Troca de Óleo Premium");
            servico.Descricao.Should().Be("Serviço de troca de óleo premium para carros");
        }

        [Fact]
        public void Deve_Deletar_Servico()
        {
            var servico = Servico.Criar(
                "Troca de Óleo",
                "001",
                "Serviço de troca de óleo para carros",
                0.5M,
                1
            );
            servico.Deletar();
            servico.Ativo.Should().Be(false);
        }

        [Fact]
        public void Deve_Lancar_Exception_Quando_Nome_For_Vazio()
        {
            var act = () => Servico.Criar(
                "",
                "001",
                "Serviço de troca de óleo para carros",
                0.5M,
                1
            );
            act.Should()
               .Throw<DomainException>()
               .WithMessage("O nome é obrigatório.");
        }

        [Fact]
        public void Deve_Lancar_Exception_Quando_Codigo_For_Vazio()
        {
            var act = () => Servico.Criar(
                "Troca de Óleo",
                "",
                "Serviço de troca de óleo para carros",
                0.5M,
                1
            );
            act.Should()
               .Throw<DomainException>()
               .WithMessage("O código é obrigatório.");
        }

        [Fact]
        public void Deve_Lancar_Exception_Quando_PrecoBase_For_Menor_Que_Zero()
        {
            var act = () => Servico.Criar(
                "Troca de Óleo",
                "001",
                "Serviço de troca de óleo para carros",
                -0.1M,
                1
            );
            act.Should()
               .Throw<DomainException>()
               .WithMessage("O preço base não pode ser negativo.");
        }

        [Fact]
        public void Deve_Lancar_Exception_Quando_TempoEstimado_For_Menor_Que_Zero()
        {
            var act = () => Servico.Criar(
                "Troca de Óleo",
                "001",
                "Serviço de troca de óleo para carros",
                0.5M,
                -1
            );
            act.Should()
               .Throw<DomainException>()
               .WithMessage("O tempo estimado não pode ser negativo.");
        }
    }
}
