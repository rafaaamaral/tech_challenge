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

        public ICollection<OrdemServicoItemServico> ItensServicos { get; set; }
        public ICollection<OrdemServicoItemPecaInsumo> ItensPecasInsumos { get; set; }
    }
}
