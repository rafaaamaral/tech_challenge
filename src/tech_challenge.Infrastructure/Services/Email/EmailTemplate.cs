using System;
using System.Collections.Generic;
using System.Text;

namespace tech_challenge.Infrastructure.Services.Email
{
    public static class EmailTemplate
    {
        public static string GerarEmailAprovacaoOrcamento(
            int ordemServicoId,
            string nomeCliente,
            string urlAprovacao,
            string urlReprovacao)
        {
            return $"""
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset="UTF-8">
        </head>
        <body style="font-family: Arial, Helvetica, sans-serif;">
            <h2>Olá, {nomeCliente}!</h2>

            <p>
                O orçamento da sua Ordem de Serviço
                <strong>{ordemServicoId}</strong>
                está aguardando sua aprovação.
            </p>

            <p>
                Escolha uma das opções abaixo:
            </p>

            <p>
                <a href="{urlAprovacao}">
                    ✅ Aprovar orçamento
                </a>
            </p>

            <p>
                <a href="{urlReprovacao}">
                    ❌ Reprovar orçamento
                </a>
            </p>

            <hr/>

            <small>
                Oficina Mecânica - Tech Challenge
            </small>

        </body>
        </html>
        """;
        }
    }
}
