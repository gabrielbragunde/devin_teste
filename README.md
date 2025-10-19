# API de Agendamento de Consultas

Sistema de agendamento de consultas médicas desenvolvido em .NET 8 com Minimal API, seguindo os princípios de Clean Architecture, Clean Code e SOLID.

## Arquitetura

O projeto está organizado em camadas seguindo Clean Architecture:

- **Domain**: Contém as entidades de negócio (Cliente, Agendamento) e regras de domínio
- **Application**: Contém DTOs, interfaces de repositórios e casos de uso
- **Infrastructure**: Implementação dos repositórios com banco de dados em memória
- **API**: Camada de apresentação com Minimal API endpoints

## Tecnologias

- .NET 8
- ASP.NET Core Minimal API
- Swagger/OpenAPI
- Banco de dados em memória (InMemory)

## Padrões de Design Implementados

### Result Pattern
O Result Pattern é utilizado para encapsular o resultado de operações, permitindo retornar sucesso ou falha de forma explícita, sem uso de exceções para controle de fluxo.

**Benefícios:**
- Elimina exceções para controle de fluxo de negócio
- Torna explícito quando uma operação pode falhar
- Facilita o tratamento de erros de forma funcional
- Melhora a performance ao evitar stack unwinding de exceções

**Exemplo de uso:**
```csharp
var clienteResult = Cliente.Criar(nome, cpf, email, telefone, dataNascimento);
if (clienteResult.IsFailure)
    return Result.Failure<ClienteDto>(clienteResult.Error);

return Result.Success(clienteResult.Value.ToDto());
```

### Notification Pattern
O Notification Pattern é usado para coletar múltiplos erros de validação antes de retornar, permitindo que o usuário veja todos os problemas de uma vez.

**Benefícios:**
- Coleta múltiplos erros de validação
- Melhora a experiência do usuário ao mostrar todos os erros de uma vez
- Separa a lógica de validação da lógica de negócio
- Facilita testes unitários

**Exemplo de uso:**
```csharp
var notification = new Notification();
ValidarDados(nome, cpf, email, telefone, dataNascimento, notification);

if (notification.HasErrors)
    return Result.Failure<Cliente>(notification.GetErrorsAsString());
```

### Extension Methods
Métodos de extensão são utilizados para converter entidades de domínio em DTOs, mantendo a separação de responsabilidades e evitando código duplicado.

**Benefícios:**
- Elimina código duplicado de mapeamento
- Mantém a lógica de conversão centralizada
- Facilita manutenção e testes
- Melhora a legibilidade do código

**Exemplo de uso:**
```csharp
public static ClienteDto ToDto(this Cliente cliente)
{
    return new ClienteDto(
        cliente.Id,
        cliente.Nome,
        cliente.Cpf,
        cliente.Email,
        cliente.Telefone,
        cliente.DataNascimento,
        cliente.CriadoEm,
        cliente.AtualizadoEm
    );
}
```

## Estrutura do Projeto

```
AgendamentoConsultas/
├── AgendamentoConsultas.Domain/
│   ├── Common/
│   │   └── BaseEntity.cs
│   ├── Entities/
│   │   ├── Cliente.cs
│   │   └── Agendamento.cs
│   └── Patterns/
│       ├── Result.cs
│       └── Notification.cs
├── AgendamentoConsultas.Application/
│   ├── DTOs/
│   │   ├── Cliente/
│   │   └── Agendamento/
│   ├── Extensions/
│   │   ├── ClienteExtensions.cs
│   │   └── AgendamentoExtensions.cs
│   ├── Interfaces/
│   │   ├── IClienteRepository.cs
│   │   └── IAgendamentoRepository.cs
│   └── UseCases/
│       ├── Cliente/
│       └── Agendamento/
├── AgendamentoConsultas.Infrastructure/
│   ├── Data/
│   │   └── InMemoryDatabase.cs
│   └── Repositories/
│       ├── ClienteRepository.cs
│       └── AgendamentoRepository.cs
└── AgendamentoConsultas.API/
    ├── Endpoints/
    │   ├── ClienteEndpoints.cs
    │   └── AgendamentoEndpoints.cs
    └── Program.cs
```

## Como Executar

### Pré-requisitos

- .NET 8 SDK instalado

### Executando a aplicação

```bash
cd AgendamentoConsultas.API
dotnet run
```

A API estará disponível em `http://localhost:5227` (ou outra porta indicada no console).

Acesse a documentação Swagger em: `http://localhost:5227/swagger`

## Endpoints da API

### Clientes

#### POST /api/clientes
Criar um novo cliente

**Request Body:**
```json
{
  "nome": "João Silva",
  "cpf": "123.456.789-09",
  "email": "joao@email.com",
  "telefone": "(11) 98765-4321",
  "dataNascimento": "1990-05-15"
}
```

**Response:** 201 Created
```json
{
  "id": "guid",
  "nome": "João Silva",
  "cpf": "123.456.789-09",
  "email": "joao@email.com",
  "telefone": "(11) 98765-4321",
  "dataNascimento": "1990-05-15T00:00:00",
  "criadoEm": "2025-10-19T23:30:02.8184492Z",
  "atualizadoEm": null
}
```

#### GET /api/clientes/cpf/{cpf}
Obter cliente por CPF

**Response:** 200 OK
```json
{
  "id": "guid",
  "nome": "João Silva",
  "cpf": "123.456.789-09",
  "email": "joao@email.com",
  "telefone": "(11) 98765-4321",
  "dataNascimento": "1990-05-15T00:00:00",
  "criadoEm": "2025-10-19T23:30:02.8184492Z",
  "atualizadoEm": null
}
```

#### PUT /api/clientes/{id}
Atualizar dados do cliente (não permite alterar CPF)

**Request Body:**
```json
{
  "nome": "João Silva Santos",
  "email": "joao.santos@email.com",
  "telefone": "(11) 99999-8888",
  "dataNascimento": "1990-05-15"
}
```

**Response:** 200 OK

### Agendamentos

#### POST /api/agendamentos
Criar um novo agendamento

**Request Body:**
```json
{
  "clienteId": "guid",
  "dataHora": "2025-10-20T10:00:00",
  "observacoes": "Primeira consulta"
}
```

**Response:** 201 Created
```json
{
  "id": "guid",
  "clienteId": "guid",
  "nomeCliente": "João Silva",
  "dataHora": "2025-10-20T10:00:00",
  "status": "Agendado",
  "observacoes": "Primeira consulta",
  "criadoEm": "2025-10-19T23:30:21.3165988Z",
  "atualizadoEm": null
}
```

#### GET /api/agendamentos/cliente/{clienteId}
Obter todos os agendamentos de um cliente

**Response:** 200 OK
```json
[
  {
    "id": "guid",
    "clienteId": "guid",
    "nomeCliente": "João Silva",
    "dataHora": "2025-10-20T10:00:00",
    "status": "Agendado",
    "observacoes": "Primeira consulta",
    "criadoEm": "2025-10-19T23:30:21.3165988Z",
    "atualizadoEm": null
  }
]
```

#### GET /api/agendamentos/horarios-disponiveis?data={data}
Obter horários disponíveis para uma data específica

**Query Parameters:**
- `data`: Data no formato ISO 8601 (ex: 2025-10-20T08:00:00)

**Response:** 200 OK
```json
[
  {
    "dataHora": "2025-10-20T08:00:00",
    "disponivel": true
  },
  {
    "dataHora": "2025-10-20T08:30:00",
    "disponivel": true
  },
  {
    "dataHora": "2025-10-20T09:00:00",
    "disponivel": false
  }
]
```

#### PUT /api/agendamentos/{id}/reagendar
Reagendar um agendamento existente

**Request Body:**
```json
{
  "novaDataHora": "2025-10-20T14:30:00"
}
```

**Response:** 200 OK

#### PUT /api/agendamentos/{id}/cancelar
Cancelar um agendamento

**Response:** 200 OK
```json
{
  "id": "guid",
  "clienteId": "guid",
  "nomeCliente": "João Silva",
  "dataHora": "2025-10-20T14:30:00",
  "status": "Cancelado",
  "observacoes": "Primeira consulta",
  "criadoEm": "2025-10-19T23:30:21.3165988Z",
  "atualizadoEm": "2025-10-19T23:30:40.2476662Z"
}
```

## Regras de Negócio

### Cliente
- CPF é obrigatório e deve ser válido
- Não é permitido cadastrar dois clientes com o mesmo CPF
- CPF não pode ser alterado após o cadastro
- Data de nascimento deve ser anterior à data atual

### Agendamento
- Horários de atendimento: 08:00 às 18:00
- Agendamentos em intervalos de 30 minutos (ex: 08:00, 08:30, 09:00)
- Não há atendimento aos finais de semana
- Não é possível agendar em horários passados
- Não é possível ter dois agendamentos no mesmo horário
- Agendamentos cancelados não podem ser reagendados
- Agendamentos concluídos não podem ser cancelados ou reagendados

## Status de Agendamento

- **Agendado**: Agendamento ativo
- **Concluído**: Consulta realizada
- **Cancelado**: Agendamento cancelado

## Observações

- A aplicação utiliza banco de dados em memória, portanto os dados são perdidos quando a aplicação é reiniciada
- Esta é uma implementação de prova de conceito (POC)
- Para produção, recomenda-se implementar persistência com banco de dados real (SQL Server, PostgreSQL, etc.)

## Princípios Aplicados

### Clean Architecture
- Separação clara de responsabilidades em camadas
- Dependências apontam para o centro (Domain)
- Domain não possui dependências externas

### SOLID
- **S**ingle Responsibility: Cada classe tem uma única responsabilidade
- **O**pen/Closed: Entidades abertas para extensão, fechadas para modificação
- **L**iskov Substitution: Interfaces bem definidas para repositórios
- **I**nterface Segregation: Interfaces específicas e coesas
- **D**ependency Inversion: Dependência de abstrações, não de implementações concretas

### Clean Code
- Nomes descritivos e significativos
- Métodos pequenos e focados
- Validações no domínio
- Uso de Result Pattern ao invés de exceções para controle de fluxo
- Notification Pattern para validações
- Extension Methods para mapeamento de objetos
- Código auto-explicativo

## Testes

Para testar a API, você pode usar:
- Swagger UI (disponível em `/swagger`)
- Postman
- cURL
- Qualquer cliente HTTP

## Licença

Este projeto foi desenvolvido para fins educacionais e de demonstração.
