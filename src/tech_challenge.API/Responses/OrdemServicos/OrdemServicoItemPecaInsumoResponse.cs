namespace tech_challenge.API.Responses.OrdemServicos
{
    public class OrdemServicoItemPecaInsumoResponse
    {
        public int Id { get; set; }
        public int PecaInsumoId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotal { get; set; }
    }
}
