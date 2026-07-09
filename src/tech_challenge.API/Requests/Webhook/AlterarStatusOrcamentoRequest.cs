using tech_challenge.Domain.Common.Enums;

namespace tech_challenge.API.Requests.Webhook
{
    public class AlterarStatusOrcamentoRequest
    {
        public Guid UniqueCode { get; set; }
        public StatusOrcamento Status { get; set; }
    }
}
