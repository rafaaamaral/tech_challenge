using tech_challenge.Domain.Common.Enums;

namespace tech_challenge.Application.Services.OrdemServicos.Model
{
    public class OrdemServicoModel
    {
        public int Id { get; set; }
        public Guid UniqueCode { get; set; }
        public int ClienteId { get; set; }
        public int VeiculoId { get; set; }
        public int Numero { get; set; }
        public StatusOrdemServico Status { get; set; }
        public OrcamentoModel Orcamento { get; set; } = new OrcamentoModel();
        public DateTime? InicioExecucao { get; set; }
        public DateTime? FimExecucao { get; set; }
        public List<OrdemServicoItemServicoModel> ItensServicos { get; set; } = new List<OrdemServicoItemServicoModel>();
        public List<OrdemServicoItemPecaInsumoModel> ItensPecasInsumos { get; set; } = new List<OrdemServicoItemPecaInsumoModel>();
    }
}
