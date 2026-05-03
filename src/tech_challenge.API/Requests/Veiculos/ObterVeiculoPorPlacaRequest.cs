namespace tech_challenge.API.Requests.Veiculos
{
    public class ObterVeiculoPorPlacaRequest
    {
        public int ClienteId { get; set; }
        public required string Placa { get; set; }
    }
}
