using tech_challenge.Domain.Common.Entities;
using tech_challenge.Domain.Exceptions;

namespace tech_challenge.Domain.Aggregates.Veiculos
{
    public class Veiculo : Audit
    {
        public Veiculo() { }
        public required int ClienteId { get; set; }
        public required Placa Placa { get; set; }
        public required string Marca { get; set; }
        public required string Modelo { get; set; }
        public int? Ano { get; set; }

        public static Veiculo Criar(int clienteId, string placa, string marca, string modelo, int? ano)
        {
            var veiculo = new Veiculo()
            {
                UniqueCode = Guid.NewGuid(),
                ClienteId = clienteId,
                Placa = new Placa(placa),
                Marca = marca,
                Modelo = modelo,
                Ano = ano
            };

            veiculo.Validar();

            return veiculo;
        }

        public void Atualizar(int clienteId, string placa, string marca, string modelo, int? ano)
        {
            ClienteId = clienteId;
            Placa = new Placa(placa);
            Marca = marca.ToUpper();
            Modelo = modelo.ToUpper();
            Ano = ano;
            Validar();
        }
        public void Delete()
        {
            Ativo = false;
        }

        private void Validar()
        {
            if (string.IsNullOrWhiteSpace(Marca))
                throw new DomainException("A marca do veículo é obrigatória.");
            if (string.IsNullOrWhiteSpace(Modelo))
                throw new DomainException("O modelo do veículo é obrigatório.");
            if (Ano.HasValue && (Ano < 1886 || Ano > DateTime.Now.Year + 1))
                throw new DomainException("O ano do veículo é inválido.");
        }   
    }
}
