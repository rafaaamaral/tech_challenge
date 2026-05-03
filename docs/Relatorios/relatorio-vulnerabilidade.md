dotnet list package --vulnerable --include-transitive > docs\scan-nuget.txt# Relatório de Análise de Vulnerabilidades

## 1. Objetivo

Este relatório apresenta a análise de vulnerabilidades realizada no projeto Tech Challenge, considerando as dependências da aplicação .NET e a imagem Docker gerada para execução da API.

## 2. Ferramentas utilizadas

- .NET CLI
- Trivy

## 3. Scan de pacotes NuGet

### Comando executado

```bash
dotnet list package --vulnerable --include-transitive > docs\Relatorios\scan-nuget.txt 
```

### Resultado

O resultado completo está disponível em:
```
docs\Relatorios\scan-nuget.txt
```

### Análise

Não foram encontradas vulnerabilidades conhecidas nas dependências NuGet diretas ou transitivas no momento da análise.

## 4. Scan da imagem Docker

### Comando executado
```
trivy image tech-challenge-api:latest > docs\Relatorios\scan-docker.txt
```

### Resultado
O resultado completo está disponível em:
```
docs\Relatorios\scan-docker.txt
```

### Análise

O scan da imagem Docker identificou um total de 21 vulnerabilidades, distribuídas da seguinte forma:

- CRITICAL: 0
- HIGH: 0
- MEDIUM: 17
- LOW: 4

Não foram identificadas vulnerabilidades críticas ou de alto risco, indicando que a aplicação não apresenta riscos imediatos de segurança.

As vulnerabilidades encontradas estão concentradas em bibliotecas do sistema operacional base da imagem (Ubuntu), como glibc, util-linux e tar, não estando diretamente relacionadas ao código da aplicação desenvolvida em .NET.

Essas vulnerabilidades são comuns em imagens base e, em sua maioria, dependem de atualizações do sistema operacional fornecidas pelos mantenedores da imagem oficial.

Para o contexto do MVP, essas vulnerabilidades são consideradas de baixo impacto, uma vez que não afetam diretamente a lógica da aplicação nem expõem falhas críticas de segurança.

## 5. Considerações Finais

Apesar da presença de vulnerabilidades de nível médio e baixo, não há evidências de exploração direta no contexto da aplicação.

O uso de imagens oficiais e atualizadas da Microsoft para .NET contribui para reduzir riscos, garantindo que a aplicação esteja alinhada com boas práticas de segurança.