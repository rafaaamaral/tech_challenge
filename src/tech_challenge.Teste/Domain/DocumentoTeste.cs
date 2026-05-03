using FluentAssertions;
using tech_challenge.Domain.Aggregates.Clientes;
using tech_challenge.Domain.Exceptions;

namespace tech_challenge.Teste.Domain
{
    public class DocumentoTeste
    {
        [Theory]
        [InlineData("608.361.410-40")]
        [InlineData("60836141040")]
        public void Deve_Criar_Documento_Quando_Cpf_For_Valido(string valor)
        {
            var documento = new Documento(valor);

            documento.Valor.Should().Be("60836141040");
        }

        [Theory]
        [InlineData("35.347.415/0001-10")]
        [InlineData("35347415000110")]
        public void Deve_Criar_Documento_Quando_Cnpj_For_Valido(string valor)
        {
            var documento = new Documento(valor);

            documento.Valor.Should().Be("35347415000110");
        }

        [Fact]
        public void Deve_Lancar_Exception_Quando_Documento_For_Invalido()
        {
            var act = () => new Documento("123");

            act.Should()
               .Throw<DomainException>()
               .WithMessage("Documento inválido.");
        }

        [Fact]
        public void Deve_Lancar_Exception_Quando_Documento_For_Vazio()
        {
            var act = () => new Documento("");

            act.Should()
               .Throw<DomainException>()
               .WithMessage("Documento obrigatório.");
        }
    }
}
