using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Domain.Common.Entities;
using tech_challenge.Domain.Common.Enums;

namespace tech_challenge.Domain.Aggregates.PecaInsumos
{
    public class PecaInsumo : Audit
    {
        public required string Nome { get; set; }
        public required string Codigo { get; set; }
        public string? Descricao { get; set; }
        public TipoPecaInsumo Tipo { get; set; }
        public required decimal PrecoUnitario { get; set; }
        public int QuantidadeEstoque { get; set; }
    }
}
