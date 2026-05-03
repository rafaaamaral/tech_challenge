using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Domain.Aggregates.Veiculos;
using tech_challenge.Domain.Exceptions;

namespace tech_challenge.Teste.Domain
{
    public class VeiculoTeste
    {
        [Fact]
        public void Deve_Criar_Veiculo_Valido()
        {
            var veiculo = Veiculo.Criar(
                1,
                "ABC1-234",                
                "Volkswagen",
                "Fusca",
                1970
            );
            veiculo.UniqueCode.Should().NotBeEmpty();
            veiculo.Placa.Valor.Should().Be("ABC1-234");
            veiculo.Modelo.Should().Be("Fusca");
            veiculo.Marca.Should().Be("Volkswagen");
            veiculo.Ano.Should().Be(1970);
        }

        [Fact]
        public void Deve_Atualizar_Veiculo()
        {
            var veiculo = Veiculo.Criar(
                1,
                "ABC1-234",
                "Volkswagen",
                "Fusca",
                1970
            );

            veiculo.Atualizar(
                1,
                "XYZ5678",
                "Chevrolet",
                "Opala",
                1980
            );
            veiculo.Placa.Valor.Should().Be("XYZ5678");
            veiculo.Modelo.Should().Be("OPALA");
            veiculo.Marca.Should().Be("CHEVROLET");
            veiculo.Ano.Should().Be(1980);
        }

        [Fact]
        public void Deve_Deletar_Veiculo()
        {
            var veiculo = Veiculo.Criar(
                1,
                "ABC1-234",
                "Volkswagen",
                "Fusca",
                1970
            );
            veiculo.Delete();
            veiculo.Ativo.Should().Be(false);
        }

        [Fact]
        public void Deve_Lancar_Exception_Quando_Marca_For_Vazia()
        {
            var act = () => Veiculo.Criar(
                1,
                "ABC1-234",
                "",
                "Volkswagen",
                1970
            );
            act.Should()
               .Throw<DomainException>()
               .WithMessage("A marca do veículo é obrigatória.");
        }

        [Fact]
        public void Deve_Lancar_Exception_Quando_Modelo_For_Vazio()
        {
            var act = () => Veiculo.Criar(
                1,
                "ABC1-234",
                "Fusca",
                "",
                1970
            );
            act.Should()
               .Throw<DomainException>()
               .WithMessage("O modelo do veículo é obrigatório.");
        }

        [Fact]
        public void Deve_Lancar_Exception_Quando_Ano_For_Invalido()
        {
            var act = () => Veiculo.Criar(
                1,
                "ABC1-234",
                "Fusca",
                "Volkswagen",
                1800
            );
            act.Should()
               .Throw<DomainException>()
               .WithMessage("O ano do veículo é inválido.");
        }
    }
}
