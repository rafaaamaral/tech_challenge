using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace tech_challenge.Application.Services.Servicos.Model
{
    [ExcludeFromCodeCoverage]
    public class ServicoModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public decimal PrecoBase { get; set; } = 0;
        public int? TempoEstimado { get; set; }
    }
}
