using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Domain.Common.Entities;

namespace tech_challenge.Domain.Aggregates.Servicos
{
    public class Servico : Audit
    {
        public required string Nome { get; set; }
        public string? Descricao { get; set; }
        public required decimal PrecoBase { get; set; } = 0;
        public int? TempoEstimado { get; set; }
    }
}
