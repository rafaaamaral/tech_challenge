namespace tech_challenge.API.Responses.OrdemServicos
{
    public class OrdemServicoItemServicoResponse
    {
        public int Id { get; set; }
        public int ServicoId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
    }
}
