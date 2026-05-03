using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using tech_challenge.Application.Services.PecaInsumos.Model;
using tech_challenge.Domain.Aggregates.PecaInsumos;

namespace tech_challenge.Application.Services.PecaInsumos
{
    [ExcludeFromCodeCoverage]
    public static class PecaInsumoMapper
    {
        public static PecaInsumoModel ToModel(this PecaInsumo pecaInsumo)
        {
            return new PecaInsumoModel
            {
                Id = pecaInsumo.Id,
                Nome = pecaInsumo.Nome,
                Codigo = pecaInsumo.Codigo,
                Descricao = pecaInsumo.Descricao,
                PrecoUnitario = pecaInsumo.PrecoUnitario,
                QuantidadeEstoque = pecaInsumo.QuantidadeEstoque
            };
        }
    }
}
