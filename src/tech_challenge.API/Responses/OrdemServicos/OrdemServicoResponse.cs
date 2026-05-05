namespace tech_challenge.API.Responses.OrdemServicos
{
    public class OrdemServicoResponse
    {
        public int Id { get; set; }
        public Guid UniqueCode { get; set; }
        public int ClienteId { get; set; }
        public int VeiculoId { get; set; }
        public int Numero { get; set; }
        public string Status { get; set; } = string.Empty;
        public OrcamentoResponse Orcamento { get; set; } = new OrcamentoResponse();
        public DateTime? InicioExecucao { get; set; }
        public DateTime? FimExecucao { get; set; }
        public List<OrdemServicoItemServicoResponse> ItensServicos { get; set; } = new List<OrdemServicoItemServicoResponse>();
        public List<OrdemServicoItemPecaInsumoResponse> ItensPecasInsumos { get; set; } = new List<OrdemServicoItemPecaInsumoResponse>();
    }
}
