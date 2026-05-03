namespace tech_challenge.API.Responses.Veiculos
{
    public class VeiculoResponse
    {
        public int Id { get; set; }
        public Guid UniqueCode { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public int? Ano { get; set; }
    }
}
