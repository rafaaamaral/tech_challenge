namespace tech_challenge.Application.Services.OrdemServicos.Model
{
    public class OrdemServicoItemServicoModel
    {
        public int Id { get; set; }
        public int ServicoId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
    }
}
