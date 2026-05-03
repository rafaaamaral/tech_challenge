using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Domain.Aggregates.Clientes;
using tech_challenge.Domain.Aggregates.Veiculos;
using tech_challenge.Domain.Common.Entities;
using tech_challenge.Domain.Common.Enums;

namespace tech_challenge.Domain.Aggregates.OrdemServicos
{
    public class OrdemServico : Audit
    {
        public required int ClienteId { get; set; }
        public required int VeiculoId { get; set; }
        public required int Numero { get; set; }
        public StatusOrdemServico Status { get; set; }
        public Orcamento Orcamento { get; set; } = new Orcamento();

        public DateTime? InicioExecucao { get; private set; }
        public DateTime? FimExecucao { get; private set; }

        public ICollection<OrdemServicoItemServico> ItensServicos { get; set; }
        public ICollection<OrdemServicoItemPecaInsumo> ItensPecasInsumos { get; set; }

        public void IniciarExecucao()
        {
            if (Orcamento.Status != StatusOrcamento.Aprovado)
                throw new InvalidOperationException("A OS precisa estar aprovada para iniciar execução.");

            Status = StatusOrdemServico.EmExecucao;
            InicioExecucao = DateTime.UtcNow;
        }

        public void FinalizarExecucao()
        {
            if (Status != StatusOrdemServico.EmExecucao)
                throw new InvalidOperationException("A OS precisa estar em execução para ser finalizada.");
            Status = StatusOrdemServico.Finalizada;
            FimExecucao = DateTime.UtcNow;
        }
        public TimeSpan? ObterTempoExecucao()
        {
            if (InicioExecucao is null || FimExecucao is null)
                return null;

            return FimExecucao.Value - InicioExecucao.Value;
        }
    }
}
