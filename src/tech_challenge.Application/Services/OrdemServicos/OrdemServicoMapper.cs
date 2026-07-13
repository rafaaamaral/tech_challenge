using System.Diagnostics.CodeAnalysis;
using tech_challenge.Application.Services.Clientes.Model;
using tech_challenge.Application.Services.OrdemServicos.Model;
using tech_challenge.Domain.Aggregates.OrdemServicos;

namespace tech_challenge.Application.Services.OrdemServicos
{
    [ExcludeFromCodeCoverage]
    public static class OrdemServicoMapper
    {
        public static OrdemServicoModel ToModel(this OrdemServico ordemServico)
        {
            return new OrdemServicoModel
            {
                Id = ordemServico.Id,
                UniqueCode = ordemServico.UniqueCode,
                ClienteId = ordemServico.ClienteId,
                VeiculoId = ordemServico.VeiculoId,
                Numero = ordemServico.Numero,
                Status = ordemServico.Status,
                InicioExecucao = ordemServico.InicioExecucao,
                FimExecucao = ordemServico.FimExecucao,
                Orcamento = new OrcamentoModel
                {
                    ValorServicos = ordemServico.Orcamento.ValorServicos,
                    ValorPecasInsumos = ordemServico.Orcamento.ValorPecasInsumos,
                    ValorTotal = ordemServico.Orcamento.ValorTotal,
                    Status = ordemServico.Orcamento.Status
                },
                ItensServicos = ordemServico.ItensServicos.Select(x => new OrdemServicoItemServicoModel
                {
                    Id = x.Id,
                    ServicoId = x.ServicoId,
                    Descricao = x.Descricao,
                    Valor = x.Valor
                }).ToList(),
                ItensPecasInsumos = ordemServico.ItensPecasInsumos.Select(x => new OrdemServicoItemPecaInsumoModel
                {
                    Id = x.Id,
                    PecaInsumoId = x.PecaInsumoId,
                    Descricao = x.Descricao,
                    Quantidade = x.Quantidade,
                    ValorUnitario = x.ValorUnitario,
                    ValorTotal = x.ObterValorTotal()
                }).ToList(),
                Cliente = ordemServico.Cliente == null
                    ? new ClienteModel { Id = ordemServico.ClienteId }
                    : new ClienteModel
                    {
                        Id = ordemServico.Cliente.Id,
                        Nome = ordemServico.Cliente.Nome,
                        Email = ordemServico.Cliente.Email,
                        Telefone = ordemServico.Cliente.Telefone
                    }
            };
        }
    }
}
