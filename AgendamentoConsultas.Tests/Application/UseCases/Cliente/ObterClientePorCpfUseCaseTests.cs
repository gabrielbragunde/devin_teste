using AgendamentoConsultas.Application.Interfaces;
using AgendamentoConsultas.Application.UseCases.Cliente;
using FluentAssertions;
using Moq;
using Xunit;

namespace AgendamentoConsultas.Tests.Application.UseCases.Cliente;

public class ObterClientePorCpfUseCaseTests
{
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly ObterClientePorCpfUseCase _useCase;

    public ObterClientePorCpfUseCaseTests()
    {
        _clienteRepositoryMock = new Mock<IClienteRepository>();
        _useCase = new ObterClientePorCpfUseCase(_clienteRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecutarAsync_ComCpfExistente_DeveRetornarCliente()
    {
        var cpf = "123.456.789-09";
        var cliente = AgendamentoConsultas.Domain.Entities.Cliente.Criar(
            "João Silva",
            cpf,
            "joao@email.com",
            "(11) 98765-4321",
            new DateTime(1990, 5, 15)
        ).Value;

        _clienteRepositoryMock
            .Setup(x => x.ObterPorCpfAsync(cpf))
            .ReturnsAsync(cliente);

        var result = await _useCase.ExecutarAsync(cpf);

        result.IsSuccess.Should().BeTrue();
        result.Value.Cpf.Should().Be(cpf);
        result.Value.Nome.Should().Be(cliente.Nome);

        _clienteRepositoryMock.Verify(x => x.ObterPorCpfAsync(cpf), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComCpfInexistente_DeveRetornarFalha()
    {
        var cpf = "999.999.999-99";

        _clienteRepositoryMock
            .Setup(x => x.ObterPorCpfAsync(cpf))
            .ReturnsAsync((AgendamentoConsultas.Domain.Entities.Cliente?)null);

        var result = await _useCase.ExecutarAsync(cpf);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Cliente não encontrado");

        _clienteRepositoryMock.Verify(x => x.ObterPorCpfAsync(cpf), Times.Once);
    }
}
