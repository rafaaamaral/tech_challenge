using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace tech_challenge.Domain.Common.Enums
{
    public enum StatusOrdemServico
    {
        [Description("Criada")]
        Criada = 1,
        [Description("Em Avaliação")]
        EmAvaliacao = 2,
        [Description("Avaliada")]
        Avaliada = 3,
        [Description("Em Aprovação")]
        EmAprovacao = 4,
        [Description("Aprovada")]
        Aprovada = 5,
        [Description("Reprovada")]
        Reprovada = 6,
        [Description("Em Execução")]
        EmExecucao = 7,
        [Description("Finalizada")]
        Finalizada = 8,
        [Description("Entregue")]
        Entregue = 9,
    }
}
