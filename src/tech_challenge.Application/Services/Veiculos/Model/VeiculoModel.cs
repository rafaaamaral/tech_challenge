using System.Diagnostics.CodeAnalysis;

namespace tech_challenge.Application.Services.Veiculos.Model
{
    [ExcludeFromCodeCoverage]
    public class VeiculoModel
    {
        public int Id { get; set; }
        public Guid UniqueCode { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public int? Ano { get; set; }
    }
}
