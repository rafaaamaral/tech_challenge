# Linguagem Ubíqua

> Documento atualizado com a linguagem ubíqua do MVP do Sistema Integrado de Atendimento e Execução de Serviços da Oficina.

## Objetivo

Este documento centraliza os termos da linguagem ubíqua do domínio da oficina mecânica para alinhar negócio, produto, desenvolvimento, testes e documentação.

A linguagem aqui definida deve ser usada em conversas, histórias de usuário, APIs, entidades de domínio, testes, eventos, comandos e documentação técnica. Quando um novo termo surgir, ele deve ser validado com o negócio antes de ser adotado no código.

## Contexto do Domínio

- **Produto/Sistema:** Sistema Integrado de Atendimento e Execução de Serviços da Oficina
- **Área de negócio:** Atendimento, diagnóstico, orçamento, execução e entrega de serviços automotivos
- **Responsáveis pela validação:** Representantes da oficina, equipe de produto e equipe técnica
- **Última atualização:** 2026-05-05

## Glossário de Termos

| Termo | Definição | Sinônimos/Não usar | Exemplo de uso | Observações |
| --- | --- | --- | --- | --- |
| Cliente | Pessoa física ou jurídica que solicita atendimento para um veículo. | Não usar `usuário` quando o contexto for de negócio. | `Cliente identificado por CPF/CNPJ.` | Pode acompanhar a OS via API/app. |
| CPF/CNPJ | Documento usado para identificar unicamente o cliente. | Não usar `documento` isolado quando for regra de identificação. | `Identificar cliente por CPF/CNPJ.` | Deve ser validado. |
| Veículo | Automóvel atendido pela oficina, vinculado a um cliente. | Preferir `veículo` em vez de `carro`, pois o domínio pode aceitar outros tipos no futuro. | `Veículo cadastrado com placa, marca, modelo e ano.` | No MVP, os dados obrigatórios são placa, marca, modelo e ano. |
| Placa | Identificador oficial do veículo. | Não usar `identificador do carro`. | `Consultar veículo pela placa.` | Deve ser validada conforme formato aceito pelo sistema. |
| Ordem de Serviço (OS) | Registro central do atendimento, contendo cliente, veículo, serviços, peças, orçamento, aprovações, execução e entrega. | Não usar `pedido`, `chamado` ou `ticket`. | `Criar Ordem de Serviço para o veículo informado.` | É o principal agregado do domínio. |
| Serviço | Tipo de trabalho executado pela oficina, como troca de óleo, alinhamento ou diagnóstico. | Não usar `tarefa` para serviço comercializado. | `Incluir serviço de alinhamento na OS.` | Deve ter preço base para cálculo do orçamento. |
| Serviço Solicitado | Serviço informado inicialmente pelo cliente na abertura da OS. | Não usar `problema` quando o cliente já pediu um serviço específico. | `Cliente solicitou troca de óleo.` | Pode ser complementado após diagnóstico. |
| Diagnóstico | Avaliação técnica realizada pela oficina para identificar necessidades de reparo, peças ou serviços adicionais. | Não usar `análise` de forma genérica. | `OS entrou em diagnóstico.` | Pode gerar necessidade de orçamento adicional. |
| Peça | Item físico usado em um reparo ou manutenção. | Não usar `produto` quando o item for aplicado na OS. | `Adicionar filtro de óleo à OS.` | Deve consumir estoque quando vinculado/confirmado conforme regra do domínio. |
| Insumo | Material consumível usado na execução do serviço, como óleo, fluido ou graxa. | Não misturar com `peça` quando houver controle separado. | `Incluir óleo 5W30 como insumo da OS.` | Também participa do orçamento e do estoque. |
| Estoque | Quantidade disponível de peças e insumos para uso nas ordens de serviço. | Não usar `inventário` se o sistema usar `estoque`. | `Verificar estoque antes da execução.` | Deve evitar baixa indevida ou uso sem disponibilidade. |
| Orçamento | Composição financeira gerada a partir dos serviços, peças e insumos previstos para a OS. | Não usar `cotação` ou `proposta`. | `Orçamento gerado automaticamente.` | No fluxo implementado, gerar orçamento coloca a OS em `Aguardando aprovação`. |
| Aprovação do Orçamento | Autorização do cliente para execução dos serviços, peças e insumos orçados. | Não usar `confirmação` quando for autorização formal do cliente. | `Cliente aprovou o orçamento.` | Sem aprovação, a OS não deve avançar para execução. |
| Reparo Adicional | Serviço, peça ou insumo identificado após diagnóstico ou durante execução e que não fazia parte do orçamento inicial. | Não usar `extra` sem qualificar. | `Reparo adicional enviado para aprovação.` | Deve gerar nova autorização do cliente. |
| Status da OS | Situação atual da Ordem de Serviço no fluxo operacional. | Não usar `fase` como nome principal. | `Status da OS alterado para Em execução.` | Valores oficiais definidos na seção de regras. |
| Recebida | Status inicial da OS após abertura e registro dos dados mínimos. | Não usar `aberta` como status oficial. | `OS recebida pela oficina.` | Indica que o atendimento foi registrado. |
| Em diagnóstico | Status que indica que a avaliação técnica do veículo está em andamento. | Não usar `em análise`. | `OS em diagnóstico pelo mecânico.` | Pode anteceder orçamento ou reparo adicional. |
| Aguardando aprovação | Status que indica que existe orçamento pendente de autorização do cliente. | Não usar `pendente` isolado. | `OS aguardando aprovação do cliente.` | A execução deve ficar bloqueada. |
| Em execução | Status que indica que os serviços autorizados estão sendo realizados. | Não usar `em manutenção` como status oficial. | `OS em execução.` | Deve ocorrer somente após aprovação necessária. |
| Finalizada | Status que indica que os serviços foram concluídos, mas o veículo ainda não foi entregue. | Não usar `concluída` se o status oficial for `Finalizada`. | `OS finalizada e pronta para entrega.` | Ainda permite processo de entrega. |
| Entregue | Status final que indica que o veículo foi devolvido ao cliente. | Não usar `fechada` como status oficial. | `OS entregue ao cliente.` | Encerra o fluxo principal da OS. |
| Mecânico | Profissional responsável por avaliar e executar serviços no veículo. | Não usar `técnico` se a oficina validar `mecânico`. | `Mecânico realizou o diagnóstico.` | Pode estar associado a diagnóstico, execução e tempo médio. |
| Atendimento | Etapa operacional em que a oficina recebe cliente/veículo e registra a demanda. | Não usar `recepção` como processo principal sem validação. | `Atendimento criou a OS.` | No Event Storming existente, aparece como ator/processo central. |
| Tempo Médio de Execução | Métrica administrativa que mede a duração média dos serviços executados. | Não usar `SLA` sem regra formal de prazo. | `Monitorar tempo médio de execução dos serviços.` | Pode ser calculado por tipo de serviço ou por OS. |
| API Administrativa | API usada por operadores internos para gestão de clientes, veículos, serviços, peças, insumos e OS. | Não confundir com API de acompanhamento do cliente. | `API administrativa exige autenticação JWT.` | Deve exigir JWT no MVP. |
| API de Acompanhamento | API usada pelo cliente para consultar o andamento da OS. | Não usar `portal` se a entrega for somente API no MVP. | `Cliente consulta progresso da OS pela API.` | Pode expor apenas dados necessários ao cliente. |
| JWT | Token de autenticação usado para proteger APIs administrativas. | Não usar `login` como sinônimo de token. | `Requisição administrativa autenticada com JWT.` | Obrigatório para APIs administrativas. |

## Regras de Linguagem

- Usar `Ordem de Serviço` ou `OS` como termo oficial para o fluxo de atendimento e execução.
- Usar `Veículo` em vez de `Carro` na documentação, no código e nas APIs.
- Usar `Peça` para item físico aplicado ao veículo e `Insumo` para material consumível.
- Usar `Orçamento` para o valor calculado a partir de serviços, peças e insumos.
- Usar `Aprovação do Orçamento` para a autorização do cliente antes da execução.
- Usar exatamente estes status oficiais da OS: `Recebida`, `Em diagnóstico`, `Aguardando aprovação`, `Em execução`, `Finalizada`, `Entregue`.
- Evitar siglas não explicadas. A primeira ocorrência de `OS` deve vir como `Ordem de Serviço (OS)`.
- Evitar termos genéricos como `pedido`, `chamado`, `ticket`, `produto`, `item`, `fase` e `pendente` quando houver termo oficial definido.
- Validar CPF/CNPJ e placa como conceitos de domínio, não apenas como campos de formulário.
- Separar a linguagem da API administrativa da linguagem da API de acompanhamento do cliente.

## Eventos do Domínio Relacionados

| Evento | Descrição | Termos envolvidos |
| --- | --- | --- |
| Cliente Identificado | O cliente foi localizado ou registrado a partir do CPF/CNPJ. | Cliente, CPF/CNPJ |
| Veículo Cadastrado | O veículo foi cadastrado com placa, marca, modelo e ano. | Veículo, Placa |
| Ordem de Serviço Criada | A OS foi aberta com cliente, veículo e serviços solicitados. | Ordem de Serviço, Cliente, Veículo, Serviço Solicitado |
| Serviço Incluído na OS | Um serviço foi adicionado à OS. | Ordem de Serviço, Serviço |
| Peça ou Insumo Incluído na OS | Uma peça ou insumo foi associado à OS. | Ordem de Serviço, Peça, Insumo, Estoque |
| Orçamento Gerado | O orçamento foi calculado automaticamente com base nos serviços, peças e insumos. | Orçamento, Serviço, Peça, Insumo |
| Orçamento Enviado ao Cliente | O orçamento foi disponibilizado para aprovação do cliente. Evento previsto fora do fluxo atual da API. | Orçamento, Cliente, Aprovação do Orçamento |
| Orçamento Aprovado | O cliente autorizou a execução do orçamento. | Aprovação do Orçamento, Cliente, Ordem de Serviço |
| Orçamento Reprovado | O cliente recusou o orçamento apresentado. | Orçamento, Cliente, Ordem de Serviço |
| OS Recebida | A OS entrou no status `Recebida`. | Ordem de Serviço, Status da OS |
| Diagnóstico Iniciado | A OS entrou em avaliação técnica. | Diagnóstico, Mecânico, Status da OS |
| OS Aguardando Aprovação | A OS aguarda autorização do cliente. | Status da OS, Aprovação do Orçamento |
| Execução Iniciada | Os serviços aprovados começaram a ser executados. | Ordem de Serviço, Serviço, Mecânico |
| Reparo Adicional Identificado | Foi encontrada necessidade de serviço, peça ou insumo adicional. | Diagnóstico, Reparo Adicional, Orçamento |
| Execução Finalizada | Os serviços da OS foram concluídos. | Ordem de Serviço, Status da OS |
| Veículo Entregue | O veículo foi devolvido ao cliente e a OS chegou ao status final. | Veículo, Cliente, Entregue |
| Estoque Baixado | A quantidade de peça ou insumo foi reduzida após uso na OS. Evento previsto fora do fluxo atual da API. | Estoque, Peça, Insumo |

## Comandos ou Ações Relacionadas

| Ação | Descrição | Termos envolvidos |
| --- | --- | --- |
| Identificar Cliente | Localizar ou cadastrar cliente por CPF/CNPJ. | Cliente, CPF/CNPJ |
| Cadastrar Cliente | Criar registro de cliente. | Cliente |
| Atualizar Cliente | Alterar dados cadastrais do cliente. | Cliente |
| Cadastrar Veículo | Registrar veículo vinculado a um cliente. | Veículo, Cliente, Placa |
| Atualizar Veículo | Alterar dados do veículo. | Veículo |
| Criar Ordem de Serviço | Abrir uma nova OS para um cliente e veículo, informando ao menos um serviço, peça ou insumo no fluxo atual da API. | Ordem de Serviço, Cliente, Veículo |
| Incluir Serviço | Adicionar serviço solicitado ou identificado à OS. No fluxo atual, isso ocorre na criação da OS. | Serviço, Ordem de Serviço |
| Incluir Peça ou Insumo | Adicionar peça ou insumo necessário à OS. No fluxo atual, isso ocorre na criação da OS. | Peça, Insumo, Estoque, Ordem de Serviço |
| Gerar Orçamento | Calcular o valor da OS com base em serviços, peças e insumos. | Orçamento, Serviço, Peça, Insumo |
| Enviar Orçamento | Disponibilizar orçamento ao cliente para aprovação. No fluxo atual, não há endpoint separado; a OS fica `Aguardando aprovação` ao gerar orçamento. | Orçamento, Cliente |
| Aprovar Orçamento | Registrar autorização do cliente. | Aprovação do Orçamento, Cliente |
| Reprovar Orçamento | Registrar recusa do cliente. | Orçamento, Cliente |
| Iniciar Diagnóstico | Alterar a OS para `Em diagnóstico`. | Diagnóstico, Status da OS |
| Iniciar Execução | Alterar a OS para `Em execução` após aprovação necessária. | Execução, Status da OS |
| Finalizar Execução | Alterar a OS para `Finalizada`. | Status da OS, Ordem de Serviço |
| Entregar Veículo | Registrar entrega do veículo e alterar OS para `Entregue`. | Veículo, Cliente, Status da OS |
| Consultar Progresso da OS | Permitir que o cliente acompanhe o status e progresso da OS. Fora do fluxo atual da API administrativa. | API de Acompanhamento, Status da OS |
| Gerenciar Estoque | Criar, atualizar, listar e controlar peças e insumos disponíveis. | Estoque, Peça, Insumo |
| Monitorar Tempo Médio | Consultar métrica de duração média da execução dos serviços. | Tempo Médio de Execução, Serviço |

## Fluxo Implementado na API

O fluxo atual da API administrativa de Ordem de Serviço (OS) usa a rota base `api/OrdemServico`.

| Ordem | Ação implementada | Resultado |
| --- | --- | --- |
| 1 | Criar Ordem de Serviço | Cria a OS como `Recebida`, validando cliente, veículo e vínculo entre eles. Os itens são informados na criação. |
| 2 | Iniciar Diagnóstico | Altera a OS de `Recebida` para `Em diagnóstico`. |
| 3 | Gerar Orçamento | Calcula valores e altera a OS para `Aguardando aprovação`. |
| 4 | Aprovar Orçamento | Aprova o orçamento e inicia a execução da OS. |
| 5 | Reprovar Orçamento | Reprova o orçamento; a OS permanece em `Aguardando aprovação`. |
| 6 | Iniciar Execução | Disponível para OS com orçamento já aprovado. |
| 7 | Finalizar Execução | Altera a OS para `Finalizada`. |
| 8 | Entregar Veículo | Altera a OS para `Entregue`. |

Fora do escopo implementado neste momento: endpoints separados para adicionar itens após a criação da OS, baixa/reserva de estoque e API de acompanhamento do cliente.

## Dúvidas em Aberto

- A placa deve aceitar somente o padrão Mercosul ou também o padrão antigo brasileiro?
- A baixa de estoque deve ocorrer ao incluir peça/insumo na OS, ao aprovar orçamento ou ao iniciar/finalizar execução?
- O cliente pode reprovar parcialmente um orçamento, aprovando alguns serviços e recusando outros?
- Reparos adicionais devem gerar um novo orçamento separado ou uma nova versão do orçamento da mesma OS?
- A API de acompanhamento do cliente exigirá autenticação própria ou consulta por identificador seguro da OS?
- O tempo médio de execução será calculado por serviço, por mecânico, por OS ou por combinação desses critérios?
