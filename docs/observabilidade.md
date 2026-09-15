# Observabilidade

## O papel de cada componente

**Serilog** implementa o destino de `ILogger`: cada evento sai como uma linha JSON no stdout, com campos pesquisaveis. **OpenTelemetry (OTel)** instrumenta chamadas e operacoes, gerando traces e metricas. **Datadog Agent** coleta os logs do container, recebe OTLP da API e coleta CPU/memoria/estado do Kubernetes. **Datadog** armazena e apresenta esses dados, com monitores e SLO.

Serilog e OTel sao escolhas da implementacao, nao exigencias exclusivas do Datadog. Nao e necessario instalar o profiler/tracer automatico Datadog junto desta instrumentacao OTel.

## Requisitos implementados

| Requisito | Implementacao |
| --- | --- |
| Integrar Datadog | OTel -> Agent via OTLP gRPC; JSON stdout -> Agent |
| Logs estruturados | Serilog com timestamp, status, message, exception e propriedades no primeiro nivel |
| Correlation ID | `X-Correlation-ID` recebido/gerado, resposta e HttpClient; `correlation_id` nos logs |
| Metricas e traces | HTTP servidor/cliente, runtime .NET, PostgreSQL e operacoes de OS |
| CPU, memoria e health | Agent/Kubernetes + `/health/live` e `/health/ready` |
| Uptime | HTTP checks, replica disponivel e SLO interno no root `monitoring/` do repositorio de infraestrutura |
| Erros/falhas de OS | Decorator do servico, metricas por resultado, spans de erro e logs; email tem operacao propria |

## Correlacao

`X-Correlation-ID` aceita 1 a 128 caracteres ASCII alfanumericos, `-`, `_` e `.`. Valor ausente, multiplo ou invalido e substituido por GUID. O mesmo valor retorna no header, inclusive em respostas 400/401/404/500. `correlation_id` identifica a requisicao nos logs; `trace_id` e `span_id` sao IDs W3C usados para relacionar logs e APM.

HttpClient criado por `IHttpClientFactory` propaga o Correlation ID. OTel/HttpClient propagam o contexto W3C `traceparent`; isso e diferente do Correlation ID. Nao adicione um cliente com `new HttpClient()` se precisar dessa propagacao de Correlation ID.

Nao ha captura adicionada de corpo, JWT, API Key ou parametros SQL. O processor remove texto SQL dos spans do Npgsql. Logs existentes da aplicacao continuam sendo emitidos; revise suas politicas de dados antes de usar dados pessoais reais. IDs de OS e correlacao ficam em logs/spans, nunca como tags de metricas.

## Metricas de OS

| Nome | Unidade | Tags |
| --- | --- | --- |
| `techchallenge.os.operations` | Contagem de operacoes concluidas | `operation`, `outcome` |
| `techchallenge.os.failures` | Contagem de operacoes nao concluidas | `operation`, `outcome` |
| `techchallenge.os.duration` | Segundos | `operation`, `outcome` |

`outcome`: `success`, `rejected` (validacao/acesso/entidade ausente), `error` (falha tecnica) ou `cancelled`. Leituras tambem sao operacoes; filtrar `operation:criar` para contar criacoes. Nomes de operacao sao fixos, como `gerar_orcamento`, `entregar` e `alterar_status_orcamento`.

`enviar_email_orcamento` gera metricas e span proprios. Se SMTP falhar, a operacao de email falha e gera alerta, mas a geracao do orcamento continua com sucesso, preservando a regra existente. A excecao original de operacoes que falham continua sendo propagada.

Os nomes de histogramas no Datadog podem receber sufixos `.count`, `.sum` e `.bucket` pela traducao OTLP; confirme no Metrics Explorer. Runtime .NET 10 publica metricas `dotnet.*`. CPU e memoria do pod/node vem do Agent.

## Healthchecks

- `/health` e `/health/live`: verificam que o processo responde, sem depender do banco.
- `/health/ready`: verifica conexao PostgreSQL; retorna 503 quando indisponivel e JSON sem detalhes de conexao.
- Startup probe permite a inicializacao/migracoes; readiness impede trafego ao pod sem banco. Falha de banco nao deve reiniciar pods por liveness.

As migracoes e o seed continuam na inicializacao, como antes. Traces HTTP de health sao filtrados; healthchecks continuam medidos pelo Agent.

## Configuracao

| Variavel | Valor no Kubernetes |
| --- | --- |
| `Observability__Enabled` | `true` no ConfigMap; padrao local `false` |
| `OTEL_SERVICE_NAME` | `tech-challenge-api` |
| `DD_ENV` | label `prod` (usar `hml` em homologacao) |
| `DD_VERSION` | SHA do commit, igual a tag da imagem |
| `OTEL_EXPORTER_OTLP_ENDPOINT` | `http://$(DD_AGENT_HOST):4317` |
| `OTEL_EXPORTER_OTLP_PROTOCOL` | `grpc` |
| `OTEL_EXPORTER_OTLP_METRICS_TEMPORALITY_PREFERENCE` | `delta` |
| `POD_UID` | UID via Downward API |
| `OTEL_RESOURCE_ATTRIBUTES` | `k8s.pod.uid=$(POD_UID)` |

Para ativar localmente com um Agent/Collector ja ouvindo em localhost:4317, defina `Observability__Enabled=true`, `OTEL_EXPORTER_OTLP_ENDPOINT=http://localhost:4317`, `OTEL_EXPORTER_OTLP_PROTOCOL=grpc` e `OTEL_EXPORTER_OTLP_METRICS_TEMPORALITY_PREFERENCE=delta` na sessao antes de iniciar a API. O banco/JWT continuam exigindo a configuracao existente. Serilog funciona mesmo com exportacao OTel desativada.

No deploy manual, defina `AWS_ACCOUNT_ID`, `AWS_REGION` e `IMAGE_TAG` de uma imagem publicada antes de renderizar o manifesto. Na pipeline, `IMAGE_TAG` ja e o SHA. Mantenha a lista explicita de variaveis no `envsubst` para preservar expressoes Kubernetes como `$(DD_AGENT_HOST)`.

A API nao recebe API Key Datadog. Veja `docs/datadog.md` em `tech-challenge-infra-k8s` para configurar Secret, Agent, monitores e SLO.

## Roteiro para a banca

1. Instalar Agent, publicar API e aplicar `monitoring/` conforme guia da infraestrutura.
2. Executar `curl.exe -i -H "X-Correlation-ID: tc3-10-demo" http://localhost:5175/health/ready` (substitua host/porta pela API utilizada). Mostrar header e JSON de health.
3. Autenticar pelo Swagger e criar uma OS valida. Nas chamadas autenticadas, enviar `X-Correlation-ID: tc3-10-os`.
4. Pesquisar `service:tech-challenge-api env:prod @correlation_id:tc3-10-os` nos Logs. Abrir o trace relacionado e mostrar spans HTTP, banco e OS.
5. No Metrics Explorer, mostrar `techchallenge.os.operations` por `operation` e `outcome`.
6. Solicitar uma transicao invalida ou consultar uma OS inexistente: demonstrar `outcome:rejected`, separado de erro tecnico.
7. Em homologacao, usar SMTP indisponivel e gerar orcamento: mostrar span `os.enviar_email_orcamento` com erro, contador de falhas e monitor. Nao provocar falhas em producao para demonstracao.
8. Mostrar CPU/memoria no Kubernetes Explorer, os monitores de health/replicas e o SLO com o historico efetivamente coletado.

Nao reutilize o mesmo Correlation ID em todas as requisicoes fora da demonstracao. Aguarde pelo menos um intervalo de exportacao de metricas (normalmente 60s). Se nao houver correlacao automatica, em preprocessing de logs JSON configure `trace_id` como Trace ID, preservando-o como string hexadecimal.

## Testes

```powershell
dotnet test tech_challenge_fase_1.slnx -c Release --filter 'FullyQualifiedName!~.Integration.'
```

Inclui testes de formato/propagacao/isolamento de Correlation ID, JSON, falhas de OS e email, readiness com banco indisponivel e exportacao OTLP para um receptor HTTP simulado. Os testes de integracao existentes exigem PostgreSQL configurado por `TECH_CHALLENGE_TEST_ADMIN_CONNECTION`; executar com banco disponivel para validar o fluxo completo. Testes locais nao comprovam ingestao real na conta Datadog.

Referencias: [Serilog ASP.NET Core](https://github.com/serilog/serilog-aspnetcore), [OTel .NET](https://opentelemetry.io/docs/languages/dotnet/), [OTLP no Agent](https://docs.datadoghq.com/opentelemetry/interoperability/otlp_ingest_in_the_agent/), [correlacao no Datadog](https://docs.datadoghq.com/opentelemetry/correlate/logs_and_traces/).
