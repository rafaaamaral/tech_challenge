# Design

Serilog -> JSON stdout -> Datadog Agent (coleta por anotacao do pod).
OpenTelemetry SDK -> OTLP gRPC no node:4317 -> Datadog Agent -> Datadog.
Agent DaemonSet coleta infraestrutura; Cluster Agent coleta estado do cluster.

Application usa ActivitySource/Meter do .NET e um decorator de IOrdemServicoService para cobrir todas as entradas sem duplicar instrumentacao nos controllers. Envio de email tem operacao propria pois a falha e tolerada pelo caso de uso.
IDs ficam em logs/spans, nunca como dimensoes de metricas. Endpoints de health nao geram traces de requisicao. SQL, corpos e cabecalhos de autenticacao nao sao adicionados pela instrumentacao.
API exporta somente quando Observability:Enabled=true. Terraform instala Agent apenas quando enable_datadog=true, referenciando Secret existente.
