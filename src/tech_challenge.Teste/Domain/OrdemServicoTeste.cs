using FluentAssertions;
using tech_challenge.Domain.Aggregates.OrdemServicos;
using tech_challenge.Domain.Common.Enums;
using tech_challenge.Domain.Exceptions;

namespace tech_challenge.Teste.Domain
{
    public class OrdemServicoTeste
    {
        [Fact]
        public void Deve_Criar_OrdemServico_Valida()
        {
            var ordemServico = OrdemServico.Criar(1, 2, 10);

            ordemServico.UniqueCode.Should().NotBeEmpty();
            ordemServico.ClienteId.Should().Be(1);
            ordemServico.VeiculoId.Should().Be(2);
            ordemServico.Numero.Should().Be(10);
            ordemServico.Status.Should().Be(StatusOrdemServico.Recebida);
            ordemServico.Orcamento.Status.Should().Be(StatusOrcamento.Pendente);
            ordemServico.ItensServicos.Should().BeEmpty();
            ordemServico.ItensPecasInsumos.Should().BeEmpty();
        }

        [Fact]
        public void Deve_Gerar_Orcamento_Com_Itens()
        {
            var ordemServico = OrdemServico.Criar(1, 2, 10);

            ordemServico.AdicionarServico(1, "Troca de oleo", 100);
            ordemServico.AdicionarPecaInsumo(1, "Oleo 5W30", 2, 50);
            ordemServico.GerarOrcamento();

            ordemServico.Status.Should().Be(StatusOrdemServico.AguardandoAprovacao);
            ordemServico.Orcamento.Status.Should().Be(StatusOrcamento.Pendente);
            ordemServico.Orcamento.ValorServicos.Should().Be(100);
            ordemServico.Orcamento.ValorPecasInsumos.Should().Be(100);
            ordemServico.Orcamento.ValorTotal.Should().Be(200);
        }

        [Fact]
        public void Deve_Executar_Fluxo_Principal_Ate_Entrega()
        {
            var ordemServico = OrdemServico.Criar(1, 2, 10);

            ordemServico.AdicionarServico(1, "Diagnóstico", 80);
            ordemServico.IniciarDiagnostico();
            ordemServico.GerarOrcamento();
            ordemServico.AprovarOrcamento();
            ordemServico.IniciarExecucao();
            ordemServico.FinalizarExecucao();
            ordemServico.Entregar();

            ordemServico.Status.Should().Be(StatusOrdemServico.Entregue);
            ordemServico.InicioExecucao.Should().NotBeNull();
            ordemServico.FimExecucao.Should().NotBeNull();
        }

        [Fact]
        public void Deve_Lancar_Exception_Quando_Cliente_For_Invalido()
        {
            var act = () => OrdemServico.Criar(0, 2, 10);

            act.Should()
                .Throw<DomainException>()
                .WithMessage("O cliente da OS é obrigatório.");
        }

        [Fact]
        public void Deve_Lancar_Exception_Quando_Iniciar_Execucao_Sem_Aprovacao()
        {
            var ordemServico = OrdemServico.Criar(1, 2, 10);

            var act = () => ordemServico.IniciarExecucao();

            act.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("A OS precisa estar aprovada para iniciar execução.");
        }

        [Fact]
        public void Deve_Lancar_Exception_Quando_Gerar_Orcamento_Sem_Itens()
        {
            var ordemServico = OrdemServico.Criar(1, 2, 10);

            var act = () => ordemServico.GerarOrcamento();

            act.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("A OS deve possuir ao menos um item para gerar orçamento.");
        }

        [Fact]
        public void Deve_Reprovar_Orcamento_Quando_OS_Estiver_Aguardando_Aprovacao()
        {
            var ordemServico = OrdemServico.Criar(1, 2, 10);
            ordemServico.AdicionarServico(1, "Diagnostico", 80);
            ordemServico.GerarOrcamento();

            ordemServico.ReprovarOrcamento();

            ordemServico.Status.Should().Be(StatusOrdemServico.AguardandoAprovacao);
            ordemServico.Orcamento.Status.Should().Be(StatusOrcamento.Reprovado);
        }

        [Fact]
        public void Deve_Lancar_Exception_Quando_Finalizar_OS_Fora_De_Execucao()
        {
            var ordemServico = OrdemServico.Criar(1, 2, 10);

            var act = () => ordemServico.FinalizarExecucao();

            act.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("A OS precisa estar em execução para ser finalizada.");
        }

        [Fact]
        public void Deve_Lancar_Exception_Quando_Entregar_OS_Nao_Finalizada()
        {
            var ordemServico = OrdemServico.Criar(1, 2, 10);

            var act = () => ordemServico.Entregar();

            act.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("A OS precisa estar finalizada para entregar o veículo.");
        }

        [Fact]
        public void Deve_Lancar_Exception_Quando_Valor_Do_Orcamento_For_Negativo()
        {
            var act = () => Orcamento.Criar(-1, 0);

            act.Should()
                .Throw<DomainException>()
                .WithMessage("O valor dos serviços não pode ser negativo.");
        }

        [Fact]
        public void Deve_Lancar_Exception_Quando_Item_Servico_For_Invalido()
        {
            var act = () => OrdemServicoItemServico.Criar(0, "Diagnostico", 80);

            act.Should()
                .Throw<DomainException>()
                .WithMessage("O serviço é obrigatório.");
        }

        [Fact]
        public void Deve_Lancar_Exception_Quando_Item_Peca_Insumo_Tiver_Quantidade_Invalida()
        {
            var act = () => OrdemServicoItemPecaInsumo.Criar(1, "Oleo 5W30", 0, 50);

            act.Should()
                .Throw<DomainException>()
                .WithMessage("A quantidade da peça ou insumo deve ser maior que zero.");
        }
    }
}
