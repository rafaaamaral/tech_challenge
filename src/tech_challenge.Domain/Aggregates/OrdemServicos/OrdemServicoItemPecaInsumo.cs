using tech_challenge.Domain.Common.Entities;
using tech_challenge.Domain.Exceptions;

namespace tech_challenge.Domain.Aggregates.OrdemServicos
{
    public class OrdemServicoItemPecaInsumo : Audit
    {
        public int OrdemServicoId { get; set; }
        public required int PecaInsumoId { get; set; }
        public required string Descricao { get; set; } = string.Empty;
        public required int Quantidade { get; set; }
        public required decimal ValorUnitario { get; set; }

        public static OrdemServicoItemPecaInsumo Criar(int pecaInsumoId, string descricao, int quantidade, decimal valorUnitario)
        {
            var item = new OrdemServicoItemPecaInsumo
            {
                UniqueCode = Guid.NewGuid(),
                PecaInsumoId = pecaInsumoId,
                Descricao = descricao,
                Quantidade = quantidade,
                ValorUnitario = valorUnitario
            };

            item.Validar();
            return item;
        }

        public decimal ObterValorTotal() => Quantidade * ValorUnitario;

        private void Validar()
        {
            if (PecaInsumoId <= 0)
                throw new DomainException("A peça ou insumo é obrigatória.");
            if (string.IsNullOrWhiteSpace(Descricao))
                throw new DomainException("A descrição da peça ou insumo é obrigatória.");
            if (Quantidade <= 0)
                throw new DomainException("A quantidade da peça ou insumo deve ser maior que zero.");
            if (ValorUnitario < 0)
                throw new DomainException("O valor unitário da peça ou insumo não pode ser negativo.");
        }
    }
}
