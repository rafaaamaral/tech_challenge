namespace tech_challenge.API.Requests.PecaInsumos
{
    public class CriarPecaInsumoRequest
    {
        public required string Nome { get; set; } = string.Empty;
        public required string Codigo { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public required decimal PrecoUnitario { get; set; }
        public int QuantidadeEstoque { get; set; } = 0;
    }
}
