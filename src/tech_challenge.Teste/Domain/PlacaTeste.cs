using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Domain.Aggregates.Veiculos;
using tech_challenge.Domain.Exceptions;

namespace tech_challenge.Teste.Domain
{
    public class PlacaTeste
    {
        [Theory]
        [InlineData("ABC1-234")]
        [InlineData("XYZ5678")]
        [InlineData("BRA-2E19")]
        public void Deve_Criar_Placa_Quando_Valida(string valor)
        {
            var placa = new Placa(valor);

            placa.Valor.Should().Be(valor);
        }

        [Fact]
        public void Deve_Lancar_Exception_Quando_Placa_For_Invalida()
        {
            var act = () => new Placa("123");
            act.Should()
               .Throw<DomainException>()
               .WithMessage("Placa inválida. Formato esperado: AAA1234 ou AAA1A23.");
        }

        [Fact]
        public void Deve_Lancar_Exception_Quando_Placa_For_Vazia()
        {
            var act = () => new Placa("");
            act.Should()
               .Throw<DomainException>()
               .WithMessage("Placa inválida.");
        }
    }
}
