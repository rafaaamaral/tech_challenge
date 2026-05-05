namespace tech_challenge.Application.Services.OrdemServicos.Model
{
    public class OrdemServicoItemPecaInsumoModel
    {
        public int Id { get; set; }
        public int PecaInsumoId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotal { get; set; }
    }
}
