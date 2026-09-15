# TC3-10 Observabilidade

- R1: Serilog publica logs JSON em stdout com service, env e version.
- R2: X-Correlation-ID validado/gerado, devolvido inclusive em erros e propagado em HttpClient; logs incluem trace_id e span_id.
- R3: OpenTelemetry exporta traces HTTP, PostgreSQL e operacoes de OS, metricas HTTP/runtime e contadores/duracao de OS via OTLP ao Agent local.
- R4: Liveness independente de dependencias; readiness verifica PostgreSQL. Agent coleta CPU/memoria e executa HTTP checks para disponibilidade.
- R5: Falhas inesperadas, rejeicoes de negocio e falhas de envio de orcamento ficam distinguiveis em logs/metricas/traces.
- R6: Instalacao reproduzivel no EKS e guia de monitores/evidencias sem credenciais versionadas.

Limite: comprovacao de ingestao e criacao de monitores na conta requerem credenciais/configuracao do usuario. Nao executar deploy remoto nesta implementacao.
