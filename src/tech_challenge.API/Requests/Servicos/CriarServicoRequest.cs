namespace tech_challenge.API.Requests.Servicos
{
    public class CriarServicoRequest
    {
        public required string Nome { get; set; } = string.Empty;
        public required string Codigo { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public decimal PrecoBase { get; set; }
        public int? TempoEstimado { get; set; }
    }
}
