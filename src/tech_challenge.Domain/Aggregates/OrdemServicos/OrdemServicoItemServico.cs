using tech_challenge.Domain.Common.Entities;
using tech_challenge.Domain.Exceptions;

namespace tech_challenge.Domain.Aggregates.OrdemServicos
{
    public class OrdemServicoItemServico : Audit
    {
        public int OrdemServicoId { get; set; }
        public required int ServicoId { get; set; }
        public required string Descricao { get; set; } = string.Empty;
        public required decimal Valor { get; set; }

        public static OrdemServicoItemServico Criar(int servicoId, string descricao, decimal valor)
        {
            var item = new OrdemServicoItemServico
            {
                UniqueCode = Guid.NewGuid(),
                ServicoId = servicoId,
                Descricao = descricao,
                Valor = valor
            };

            item.Validar();
            return item;
        }

        private void Validar()
        {
            if (ServicoId <= 0)
                throw new DomainException("O serviço é obrigatório.");
            if (string.IsNullOrWhiteSpace(Descricao))
                throw new DomainException("A descrição do serviço é obrigatória.");
            if (Valor < 0)
                throw new DomainException("O valor do serviço não pode ser negativo.");
        }
    }
}
