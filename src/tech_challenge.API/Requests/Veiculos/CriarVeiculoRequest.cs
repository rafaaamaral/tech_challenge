namespace tech_challenge.API.Requests.Veiculos
{
    public class CriarVeiculoRequest
    {
        public int ClienteId { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int? Ano { get; set; }
    }
}
