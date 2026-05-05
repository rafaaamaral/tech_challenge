using tech_challenge.Domain.Common.Enums;

namespace tech_challenge.Application.Services.OrdemServicos.Model
{
    public class OrcamentoModel
    {
        public decimal ValorServicos { get; set; }
        public decimal ValorPecasInsumos { get; set; }
        public decimal ValorTotal { get; set; }
        public StatusOrcamento Status { get; set; }
    }
}
