using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Domain.Common.Entities;

namespace tech_challenge.Domain.Aggregates.OrdemServicos
{
    public class OrdemServicoItemPecaInsumo : Audit
    {
        public required int OrdemServicoId { get; set; }
        public required int PecaInsumoId { get; set; }
        public required string Descricao { get; set; } = string.Empty;
        public required int Quantidade { get; set; }
        public required decimal ValorUnitario { get; set; }
    }
}
