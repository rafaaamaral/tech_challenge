using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Domain.Common.Enums;

namespace tech_challenge.Domain.Aggregates.OrdemServicos
{
    public class Orcamento
    {
        public decimal ValorServicos { get; set; }
        public decimal ValorPecasInsumos { get; set; }
        public decimal ValorTotal { get; set; }
        public StatusOrcamento Status { get; set; }
    }
}
