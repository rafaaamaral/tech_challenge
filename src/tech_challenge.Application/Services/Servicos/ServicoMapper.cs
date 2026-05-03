using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Application.Services.Servicos.Model;
using tech_challenge.Domain.Aggregates.Servicos;

namespace tech_challenge.Application.Services.Servicos
{
    public static class ServicoMapper
    {
        public static ServicoModel ToModel(this Servico servico)
        {
            return new ServicoModel
            {
                Id = servico.Id,
                Nome = servico.Nome,
                Codigo = servico.Codigo,
                Descricao = servico.Descricao,
                PrecoBase = servico.PrecoBase,
                TempoEstimado = servico.TempoEstimado
            };
        }
    }
}
