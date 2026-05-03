namespace tech_challenge.API.Responses.Servicos
{
    public class ServicoResponse
    {
        public int Id { get; set; }
        public required string Nome { get; set; } = string.Empty;
        public required string Codigo { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public required decimal PrecoBase { get; set; }
        public int? TempoEstimado { get; set; }
    }
}
