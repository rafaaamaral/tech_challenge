using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using tech_challenge.Application.Interfaces.Services;
using tech_challenge.Application.Services.OrdemServicos.Model;
using tech_challenge.Domain.Common.Enums;

namespace tech_challenge.Infrastructure.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }
        public async Task EnviarEmailAprovacaoOrcamentoAsync(OrdemServicoModel ordemServico, CancellationToken cancellationToken = default)
        {
            var urlAprovacao = $"{_settings.ApiBaseUrl}/api/ordemservico/orcamentos/aprovacao?uniqueCode={ordemServico.UniqueCode}&status={StatusOrcamento.Aprovado}";

            var urlReprovacao = $"{_settings.ApiBaseUrl}/api/ordemservico/orcamentos/aprovacao?uniqueCode={ordemServico.UniqueCode}&status={StatusOrcamento.Reprovado}";

            var body = EmailTemplate.GerarEmailAprovacaoOrcamento(
                    ordemServico.Numero,
                    ordemServico.Cliente.Nome,
                    urlAprovacao,
                    urlReprovacao);

            using var message = new MailMessage
            {
                From = new MailAddress(
                    _settings.From,
                    _settings.DisplayName),

                Subject = $"Orçamento da Ordem de Serviço #{ordemServico.Numero}",

                Body = body,

                IsBodyHtml = true
            };

            message.To.Add(ordemServico.Cliente.Email);

            using var smtp = new SmtpClient(_settings.Host, _settings.Port)
            {
                Credentials = new NetworkCredential(
                    _settings.UserName,
                    _settings.Password),

                EnableSsl = _settings.EnableSsl
            };

            await smtp.SendMailAsync(message, cancellationToken);
        }
    }
}
