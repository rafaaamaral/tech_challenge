using tech_challenge.Domain.Common.Entities;
using tech_challenge.Domain.Common.Enums;
using tech_challenge.Domain.Exceptions;

namespace tech_challenge.Domain.Aggregates.OrdemServicos
{
    public class OrdemServico : Audit
    {
        public OrdemServico() { }

        public required int ClienteId { get; set; }
        public required int VeiculoId { get; set; }
        public required int Numero { get; set; }
        public StatusOrdemServico Status { get; set; } = StatusOrdemServico.Recebida;
        public Orcamento Orcamento { get; set; } = Orcamento.Criar(0, 0);

        public DateTime? InicioExecucao { get; private set; }
        public DateTime? FimExecucao { get; private set; }

        public ICollection<OrdemServicoItemServico> ItensServicos { get; set; } = new List<OrdemServicoItemServico>();
        public ICollection<OrdemServicoItemPecaInsumo> ItensPecasInsumos { get; set; } = new List<OrdemServicoItemPecaInsumo>();

        public static OrdemServico Criar(int clienteId, int veiculoId, int numero)
        {
            var ordemServico = new OrdemServico
            {
                UniqueCode = Guid.NewGuid(),
                ClienteId = clienteId,
                VeiculoId = veiculoId,
                Numero = numero,
                Status = StatusOrdemServico.Recebida,
                Orcamento = Orcamento.Criar(0, 0)
            };

            ordemServico.Validar();
            return ordemServico;
        }

        public void AdicionarServico(int servicoId, string descricao, decimal valor)
        {
            ItensServicos.Add(OrdemServicoItemServico.Criar(servicoId, descricao, valor));
            AtualizarOrcamento();
        }

        public void AdicionarPecaInsumo(int pecaInsumoId, string descricao, int quantidade, decimal valorUnitario)
        {
            ItensPecasInsumos.Add(OrdemServicoItemPecaInsumo.Criar(pecaInsumoId, descricao, quantidade, valorUnitario));
            AtualizarOrcamento();
        }

        public void IniciarDiagnostico()
        {
            if (Status != StatusOrdemServico.Recebida)
                throw new InvalidOperationException("A OS precisa estar recebida para iniciar diagnóstico.");

            Status = StatusOrdemServico.EmDiagnostico;
        }

        public void GerarOrcamento()
        {
            if (!ItensServicos.Any() && !ItensPecasInsumos.Any())
                throw new InvalidOperationException("A OS deve possuir ao menos um item para gerar orçamento.");

            if (Status != StatusOrdemServico.Recebida
                && Status != StatusOrdemServico.EmDiagnostico
                && Status != StatusOrdemServico.EmExecucao)
                throw new InvalidOperationException("A OS precisa estar recebida, em diagnóstico ou em execução para gerar orçamento.");

            AtualizarOrcamento();
            Orcamento.Status = StatusOrcamento.Pendente;
            Status = StatusOrdemServico.AguardandoAprovacao;
        }

        public void AprovarOrcamento()
        {
            if (Status != StatusOrdemServico.AguardandoAprovacao)
                throw new InvalidOperationException("A OS precisa estar aguardando aprovação para aprovar o orçamento.");

            Orcamento.Aprovar();
        }

        public void ReprovarOrcamento()
        {
            if (Status != StatusOrdemServico.AguardandoAprovacao)
                throw new InvalidOperationException("A OS precisa estar aguardando aprovação para reprovar o orçamento.");

            Orcamento.Reprovar();
        }

        public void IniciarExecucao()
        {
            if (Orcamento.Status != StatusOrcamento.Aprovado)
                throw new InvalidOperationException("A OS precisa estar aprovada para iniciar execução.");

            if (Status != StatusOrdemServico.AguardandoAprovacao)
                throw new InvalidOperationException("A OS precisa estar aguardando aprovação para iniciar execução.");

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

        public void Entregar()
        {
            if (Status != StatusOrdemServico.Finalizada)
                throw new InvalidOperationException("A OS precisa estar finalizada para entregar o veículo.");

            Status = StatusOrdemServico.Entregue;
        }

        public void Delete()
        {
            Ativo = false;
        }

        public TimeSpan? ObterTempoExecucao()
        {
            if (InicioExecucao is null || FimExecucao is null)
                return null;

            return FimExecucao.Value - InicioExecucao.Value;
        }

        private void AtualizarOrcamento()
        {
            var valorServicos = ItensServicos.Sum(x => x.Valor);
            var valorPecasInsumos = ItensPecasInsumos.Sum(x => x.ObterValorTotal());

            Orcamento.AtualizarValores(valorServicos, valorPecasInsumos);
        }

        private void Validar()
        {
            if (ClienteId <= 0)
                throw new DomainException("O cliente da OS é obrigatório.");
            if (VeiculoId <= 0)
                throw new DomainException("O veículo da OS é obrigatório.");
            if (Numero <= 0)
                throw new DomainException("O número da OS é obrigatório.");
        }
    }
}
