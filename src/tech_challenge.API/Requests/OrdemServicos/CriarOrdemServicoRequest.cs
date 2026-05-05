namespace tech_challenge.API.Requests.OrdemServicos
{
    public class CriarOrdemServicoRequest
    {
        public int ClienteId { get; set; }
        public int VeiculoId { get; set; }
        public List<CriarOrdemServicoItemServicoRequest> ItensServicos { get; set; } = new List<CriarOrdemServicoItemServicoRequest>();
        public List<CriarOrdemServicoItemPecaInsumoRequest> ItensPecasInsumos { get; set; } = new List<CriarOrdemServicoItemPecaInsumoRequest>();
    }
}
