using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using tech_challenge.Application.Services.Veiculos.Model;
using tech_challenge.Domain.Aggregates.Veiculos;

namespace tech_challenge.Application.Services.Veiculos
{
    [ExcludeFromCodeCoverage]
    public static class VeiculoMapper
    {
        public static VeiculoModel ToModel(this Veiculo veiculo)
        {
            return new VeiculoModel
            {
                Id = veiculo.Id,
                UniqueCode = veiculo.UniqueCode,
                Placa = veiculo.Placa.Valor,
                Marca = veiculo.Marca,
                Modelo = veiculo.Modelo,
                Ano = veiculo.Ano
            };
        }
    }
}
