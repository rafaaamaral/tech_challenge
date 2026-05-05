namespace tech_challenge.API.Responses.OrdemServicos
{
    public class OrcamentoResponse
    {
        public decimal ValorServicos { get; set; }
        public decimal ValorPecasInsumos { get; set; }
        public decimal ValorTotal { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
