using FluentAssertions;
using tech_challenge.Domain.Aggregates.Clientes;
using tech_challenge.Domain.Exceptions;

namespace tech_challenge.Teste.Domain
{
    public class ClienteTeste
    {
        [Fact]
        public void Deve_Criar_Cliente_Valido()
        {
            var cliente = Cliente.Criar(
                "Rafael Amaral",
                "570.798.160-84",
                "rafael@email.com",
                "11999999999"
            );

            cliente.UniqueCode.Should().NotBeEmpty();
            cliente.Nome.Should().Be("Rafael Amaral");
            cliente.Documento.Valor.Should().Be("57079816084");
            cliente.Email.Should().Be("rafael@email.com");
            cliente.Telefone.Should().Be("11999999999");
        }

        [Fact]
        public void Deve_Deletar_Cliente()
        {
            var cliente = Cliente.Criar(
                "Rafael Amaral",
                "570.798.160-84",
                "rafael@email.com",
                "11999999999"
            );

            cliente.Delete();

            cliente.Ativo.Should().Be(false);
        }

        [Fact]
        public void Deve_Lancar_Exception_Quando_Nome_For_Vazio()
        {
            var act = () => Cliente.Criar(
                "",
                "570.798.160-84",
                "rafael@email.com",
                "11999999999"
            );

            act.Should()
               .Throw<DomainException>()
               .WithMessage("O nome do cliente é obrigatório.");
        }

        [Fact]
        public void Deve_Lancar_Exception_Quando_Email_For_Vazio()
        {
            var act = () => Cliente.Criar(
                "Rafael Amaral",
                "570.798.160-84",
                "",
                "11999999999"
            );

            act.Should()
               .Throw<DomainException>()
               .WithMessage("O email do cliente é obrigatório.");
        }
    }
}
