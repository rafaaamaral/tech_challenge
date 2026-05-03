using System;
using System.Collections.Generic;
using System.Text;

namespace tech_challenge.Application.Services.PecaInsumos.Model
{
    public class PecaInsumoModel
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Codigo { get; set; }
        public string? Descricao { get; set; }
        public required decimal PrecoUnitario { get; set; }
        public required int QuantidadeEstoque { get; set; } = 0;

    }
}
