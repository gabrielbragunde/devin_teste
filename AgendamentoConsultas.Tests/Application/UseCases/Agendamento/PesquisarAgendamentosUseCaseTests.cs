using AgendamentoConsultas.Application.DTOs.Agendamento;
using AgendamentoConsultas.Application.Interfaces;
using AgendamentoConsultas.Application.UseCases.Agendamento;
using AgendamentoConsultas.Domain.Entities;
using DomainCliente = AgendamentoConsultas.Domain.Entities.Cliente;
using DomainAgendamento = AgendamentoConsultas.Domain.Entities.Agendamento;
using FluentAssertions;
using Moq;

namespace AgendamentoConsultas.Tests.Application.UseCases.Agendamento;

public class PesquisarAgendamentosUseCaseTests
{
    private readonly Mock<IAgendamentoRepository> _agendamentoRepositoryMock;
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly PesquisarAgendamentosUseCase _useCase;

    public PesquisarAgendamentosUseCaseTests()
    {
        _agendamentoRepositoryMock = new Mock<IAgendamentoRepository>();
        _clienteRepositoryMock = new Mock<IClienteRepository>();
        _useCase = new PesquisarAgendamentosUseCase(
            _agendamentoRepositoryMock.Object,
            _clienteRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecutarAsync_ComPeriodoInvalido_DeveFalhar()
    {
        var filtro = new PesquisarAgendamentosFiltroDto(
            null,
            DateTime.Today,
            DateTime.Today.AddDays(-1),
            null,
            null);

        var resultado = await _useCase.ExecutarAsync(filtro);

        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().Contain("Período de datas inválido");
    }

    [Fact]
    public async Task ExecutarAsync_ComStatusInvalido_DeveFalhar()
    {
        var filtro = new PesquisarAgendamentosFiltroDto(
            null,
            null,
            null,
            "Invalido",
            null);

        var resultado = await _useCase.ExecutarAsync(filtro);

        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().Contain("Status inválido");
    }

    [Fact]
    public async Task ExecutarAsync_ComClienteInexistente_DeveFalhar()
    {
        var clienteId = Guid.NewGuid();
        _clienteRepositoryMock
            .Setup(x => x.ObterPorIdAsync(clienteId))
            .ReturnsAsync((DomainCliente?)null);

        var filtro = new PesquisarAgendamentosFiltroDto(
            clienteId,
            null,
            null,
            null,
            null);

        var resultado = await _useCase.ExecutarAsync(filtro);

        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().Contain("Cliente não encontrado");
    }

    [Fact]
    public async Task ExecutarAsync_ComFiltroValido_DeveRetornarResultadosMapeados()
    {
        var clienteId = Guid.NewGuid();
        var cliente = DomainCliente.Criar(
            "Maria Silva",
            "987.654.321-00",
            "maria@email.com",
            "(11) 91234-5678",
            new DateTime(1985, 1, 1)).Value;

        _clienteRepositoryMock
            .Setup(x => x.ObterPorIdAsync(clienteId))
            .ReturnsAsync(cliente);

        var agendamento = DomainAgendamento.Criar(
            clienteId,
            DateTime.Now.AddDays(1).Date.AddHours(10),
            "Retorno").Value;

        _agendamentoRepositoryMock
            .Setup(x => x.PesquisarAsync(clienteId, null, null, null, null))
            .ReturnsAsync(new List<DomainAgendamento> { agendamento });

        _clienteRepositoryMock
            .Setup(x => x.ObterPorIdAsync(agendamento.ClienteId))
            .ReturnsAsync(cliente);

        var filtro = new PesquisarAgendamentosFiltroDto(
            clienteId,
            null,
            null,
            null,
            null);

        var resultado = await _useCase.ExecutarAsync(filtro);

        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().HaveCount(1);
        resultado.Value.First().NomeCliente.Should().Be("Maria Silva");
    }
}
