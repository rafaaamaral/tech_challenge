# Diagramas DDD

Este documento apresenta uma visão inicial dos diagramas de Domain-Driven Design (DDD) para o MVP do Sistema Integrado de Atendimento e Execução de Serviços da Oficina.

Os nomes usados aqui seguem a linguagem ubíqua definida em [`linguagem-ubiqua-template.md`](./linguagem-ubiqua-template.md).

## Context Map

O contexto principal do MVP é a gestão da Ordem de Serviço (OS). Os demais contextos apoiam esse fluxo com dados cadastrais, catálogo, estoque, autenticação e acompanhamento do cliente.

```mermaid
flowchart LR
    Cliente[Cliente]
    Atendimento[Atendimento]
    Administrativo[Administrativo]

    subgraph ClientesVeiculos[Contexto: Clientes e Veículos]
        ClienteCadastro[Cadastro de Cliente]
        VeiculoCadastro[Cadastro de Veículo]
    end

    subgraph OrdemServico[Contexto Principal: Ordem de Serviço]
        OS[Ordem de Serviço]
        Diagnostico[Diagnóstico]
        Orcamento[Orçamento]
        Aprovacao[Aprovação do Orçamento]
        Execucao[Execução de Serviços]
        Entrega[Entrega do Veículo]
    end

    subgraph CatalogoEstoque[Contexto: Catálogo e Estoque]
        CatalogoServicos[Catálogo de Serviços]
        PecasInsumos[Peças e Insumos]
        Estoque[Controle de Estoque]
    end

    subgraph Acompanhamento[Contexto: Acompanhamento do Cliente]
        ConsultaStatus[Consulta de Progresso da OS]
        AutorizacaoCliente[Autorização de Reparos Adicionais]
    end

    subgraph Seguranca[Contexto: Segurança]
        JWT[Autenticação JWT]
        APIAdmin[API Administrativa]
    end

    Cliente --> Atendimento
    Atendimento --> ClienteCadastro
    Atendimento --> VeiculoCadastro
    Atendimento --> OS

    OS --> Diagnostico
    Diagnostico --> Orcamento
    Orcamento --> Aprovacao
    Aprovacao --> Execucao
    Execucao --> Entrega

    OS --> CatalogoServicos
    OS --> PecasInsumos
    PecasInsumos --> Estoque

    Cliente --> ConsultaStatus
    Cliente --> AutorizacaoCliente
    ConsultaStatus --> OS
    AutorizacaoCliente --> Aprovacao

    Administrativo --> APIAdmin
    APIAdmin --> JWT
    APIAdmin --> ClientesVeiculos
    APIAdmin --> OrdemServico
    APIAdmin --> CatalogoEstoque
```

## Modelo de Domínio

Neste MVP, `OrdemDeServico` é o principal agregado. Ela referencia cliente, veículo, serviços, peças, insumos, orçamento e controla o status da OS.

```mermaid
classDiagram
    class Cliente {
        +Id
        +Nome
        +CpfCnpj
        +Contato
    }

    class Veiculo {
        +Id
        +Placa
        +Marca
        +Modelo
        +Ano
    }

    class OrdemDeServico {
        +Id
        +Numero
        +Status
        +DataCriacao
        +Criar()
        +IniciarDiagnostico()
        +EnviarOrcamento()
        +AprovarOrcamento()
        +IniciarExecucao()
        +Finalizar()
        +Entregar()
    }

    class ItemServico {
        +ServicoId
        +Descricao
        +Valor
    }

    class ItemPecaInsumo {
        +PecaInsumoId
        +Descricao
        +Quantidade
        +ValorUnitario
    }

    class Orcamento {
        +Id
        +ValorServicos
        +ValorPecasInsumos
        +ValorTotal
        +StatusAprovacao
        +Gerar()
        +Aprovar()
        +Rejeitar()
    }

    class Servico {
        +Id
        +Nome
        +Descricao
        +PrecoBase
        +TempoEstimado
    }

    class PecaInsumo {
        +Id
        +Nome
        +Tipo
        +PrecoUnitario
        +QuantidadeEmEstoque
    }

    class Estoque {
        +VerificarDisponibilidade()
        +Reservar()
        +Baixar()
        +Repor()
    }

    Cliente "1" --> "0..*" Veiculo : possui
    Cliente "1" --> "0..*" OrdemDeServico : solicita
    Veiculo "1" --> "0..*" OrdemDeServico : recebe atendimento
    OrdemDeServico "1" *-- "1..*" ItemServico : contem
    OrdemDeServico "1" *-- "0..*" ItemPecaInsumo : utiliza
    OrdemDeServico "1" *-- "1" Orcamento : gera
    ItemServico --> Servico : referencia
    ItemPecaInsumo --> PecaInsumo : referencia
    PecaInsumo --> Estoque : controlado por
```

## Camadas DDD

A API deve orquestrar casos de uso por meio da camada de aplicação. As regras de negócio ficam no domínio, e detalhes técnicos permanecem na infraestrutura.

```mermaid
flowchart TB
    subgraph Interface[Interface / API]
        Controllers[Controllers REST]
        DTOs[Requests e Responses]
        Auth[JWT e Policies]
    end

    subgraph Aplicacao[Aplicação]
        UseCases[Casos de Uso]
        AppServices[Application Services]
        Ports[Interfaces de Repositório e Serviços]
    end

    subgraph Dominio[Domínio]
        Aggregates[Agregados]
        Entities[Entidades]
        ValueObjects[Value Objects]
        DomainServices[Serviços de Domínio]
        DomainEvents[Eventos de Domínio]
    end

    subgraph Infra[Infraestrutura]
        Repositories[Repositórios]
        Database[(Banco de Dados)]
        JwtProvider[Provedor JWT]
        ExternalServices[Integrações Futuras]
    end

    Controllers --> DTOs
    Controllers --> Auth
    Controllers --> UseCases
    UseCases --> AppServices
    AppServices --> Ports
    AppServices --> Aggregates
    Aggregates --> Entities
    Aggregates --> ValueObjects
    Aggregates --> DomainEvents
    DomainServices --> Aggregates
    Repositories --> Ports
    Repositories --> Database
    JwtProvider --> Auth
    ExternalServices --> Ports
```

## Ciclo de Vida da Ordem de Serviço

Os status oficiais da OS devem ser usados exatamente como definidos na linguagem ubíqua.

```mermaid
stateDiagram-v2
    [*] --> Recebida: Criar OS
    Recebida --> EmDiagnostico: Iniciar diagnóstico
    EmDiagnostico --> AguardandoAprovacao: Gerar e enviar orçamento
    AguardandoAprovacao --> EmExecucao: Cliente aprova orçamento
    AguardandoAprovacao --> AguardandoAprovacao: Cliente solicita ajuste ou há reparo adicional
    EmExecucao --> AguardandoAprovacao: Reparo adicional identificado
    EmExecucao --> Finalizada: Finalizar execução
    Finalizada --> Entregue: Entregar veículo
    Entregue --> [*]

    state "Em diagnóstico" as EmDiagnostico
    state "Aguardando aprovação" as AguardandoAprovacao
    state "Em execução" as EmExecucao
```

## Fluxo Principal da OS

Este fluxo representa a jornada principal desde a identificação do cliente até a entrega do veículo.

```mermaid
flowchart TD
    A[Identificar Cliente por CPF/CNPJ]
    B[Cadastrar ou localizar Veículo]
    C[Incluir Serviços Solicitados]
    D[Incluir Peças e Insumos necessários]
    E[Gerar Orçamento]
    F[Enviar Orçamento ao Cliente]
    G{Cliente aprovou?}
    H[Iniciar Execução]
    I[Atualizar Progresso da OS]
    J{Reparo adicional?}
    K[Gerar Orçamento Adicional]
    L[Finalizar Serviços]
    M[Entregar Veículo]
    N[Manter OS Aguardando Aprovação]

    A --> B
    B --> C
    C --> D
    D --> E
    E --> F
    F --> G
    G -- Sim --> H
    G -- Não --> N
    H --> I
    I --> J
    J -- Sim --> K
    K --> F
    J -- Não --> L
    L --> M
```

## Sequência: Criação e Aprovação da OS

```mermaid
sequenceDiagram
    actor Atendente
    actor Cliente
    participant API as API Administrativa
    participant App as Aplicação
    participant OS as Agregado OrdemDeServico
    participant Estoque as Estoque
    participant Notificacao as API de Acompanhamento

    Atendente->>API: Criar OS com cliente, veículo e serviços
    API->>App: Executar caso de uso Criar Ordem de Serviço
    App->>OS: Criar OS
    OS-->>App: OS Recebida

    Atendente->>API: Incluir peças e insumos
    API->>App: Atualizar itens da OS
    App->>Estoque: Verificar disponibilidade
    Estoque-->>App: Disponibilidade confirmada
    App->>OS: Adicionar itens e gerar orçamento
    OS-->>App: Orçamento Gerado

    App->>Notificacao: Disponibilizar orçamento ao cliente
    Cliente->>Notificacao: Consultar orçamento
    Cliente->>Notificacao: Aprovar orçamento
    Notificacao->>App: Registrar aprovação
    App->>OS: Aprovar orçamento
    OS-->>App: OS Em execução
```

## Eventos e Comandos

```mermaid
flowchart LR
    subgraph Comandos[Comandos]
        C1[Identificar Cliente]
        C2[Cadastrar Veículo]
        C3[Criar Ordem de Serviço]
        C4[Gerar Orçamento]
        C5[Aprovar Orçamento]
        C6[Iniciar Execução]
        C7[Finalizar Execução]
        C8[Entregar Veículo]
    end

    subgraph Eventos[Eventos de Domínio]
        E1[Cliente Identificado]
        E2[Veículo Cadastrado]
        E3[Ordem de Serviço Criada]
        E4[Orçamento Gerado]
        E5[Orçamento Aprovado]
        E6[Execução Iniciada]
        E7[Execução Finalizada]
        E8[Veículo Entregue]
    end

    C1 --> E1
    C2 --> E2
    C3 --> E3
    C4 --> E4
    C5 --> E5
    C6 --> E6
    C7 --> E7
    C8 --> E8
```

## Imagens PNG Geradas

- [Context Map](./diagramas-ddd-01-context-map.png)
- [Modelo de Domínio](./diagramas-ddd-02-modelo-de-dominio.png)
- [Camadas DDD](./diagramas-ddd-03-camadas-ddd.png)
- [Ciclo de Vida da OS](./diagramas-ddd-04-ciclo-de-vida-da-os.png)
- [Fluxo Principal da OS](./diagramas-ddd-05-fluxo-principal-da-os.png)
- [Sequência: Criação e Aprovação da OS](./diagramas-ddd-06-sequencia-criacao-aprovacao-os.png)
- [Eventos e Comandos](./diagramas-ddd-07-eventos-e-comandos.png)
