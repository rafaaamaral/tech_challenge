using tech_challenge.Domain.Common.Entities;

namespace tech_challenge.Domain.Aggregates.Veiculos
{
    public class Veiculo : Audit
    {
        public required int ClienteId { get; set; }
        public required Placa Placa { get; set; }
        public required string Marca { get; set; }
        public required string Modelo { get; set; }
        public int? Ano { get; set; }
    }
}
