namespace tech_challenge.API.Responses.PecaInsumos
{
    public class PecaInsumoResponse
    {
        public int Id { get; set; }
        public required string Nome { get; set; } = string.Empty;
        public required string Codigo { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public required decimal PrecoUnitario { get; set; }
        public required int QuantidadeEstoque { get; set; } = 0;
    }
}
