using tech_challenge.Domain.Common.Enums;
using tech_challenge.Domain.Exceptions;

namespace tech_challenge.Domain.Aggregates.OrdemServicos
{
    public class Orcamento
    {
        public decimal ValorServicos { get; set; }
        public decimal ValorPecasInsumos { get; set; }
        public decimal ValorTotal { get; set; }
        public StatusOrcamento Status { get; set; } = StatusOrcamento.Pendente;

        public static Orcamento Criar(decimal valorServicos, decimal valorPecasInsumos)
        {
            var orcamento = new Orcamento
            {
                ValorServicos = valorServicos,
                ValorPecasInsumos = valorPecasInsumos,
                ValorTotal = valorServicos + valorPecasInsumos,
                Status = StatusOrcamento.Pendente
            };

            orcamento.Validar();
            return orcamento;
        }

        public void AtualizarValores(decimal valorServicos, decimal valorPecasInsumos)
        {
            ValorServicos = valorServicos;
            ValorPecasInsumos = valorPecasInsumos;
            ValorTotal = valorServicos + valorPecasInsumos;
            Validar();
        }

        public void Aprovar()
        {
            Status = StatusOrcamento.Aprovado;
        }

        public void Reprovar()
        {
            Status = StatusOrcamento.Reprovado;
        }

        private void Validar()
        {
            if (ValorServicos < 0)
                throw new DomainException("O valor dos serviços não pode ser negativo.");
            if (ValorPecasInsumos < 0)
                throw new DomainException("O valor das peças e insumos não pode ser negativo.");
            if (ValorTotal < 0)
                throw new DomainException("O valor total do orçamento não pode ser negativo.");
        }
    }
}
