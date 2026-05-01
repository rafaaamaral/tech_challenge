using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace tech_challenge.Domain.Common.Enums
{
    public enum StatusOrdemServico
    {
        [Description("Recebida")]
        Recebida = 1,
        [Description("Em diagnóstico")]
        EmDiagnostico = 2,
        [Description("Aguardando Aprovação")]
        AguardandoAprovacao = 3,
        [Description("Em Execução")]
        EmExecucao = 4,
        [Description("Finalizada")]
        Finalizada = 5,
        [Description("Entregue")]
        Entregue = 6,
    }
}
