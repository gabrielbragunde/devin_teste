using AgendamentoConsultas.Infrastructure.Repositories;
using FluentAssertions;
using Xunit;

namespace AgendamentoConsultas.Tests.Infrastructure.Repositories;

public class ClienteRepositoryTests
{
    [Fact]
    public async Task AdicionarAsync_DeveAdicionarCliente()
    {
        var repository = new ClienteRepository();
        var cliente = AgendamentoConsultas.Domain.Entities.Cliente.Criar(
            "João Silva",
            "123.456.789-09",
            "joao@email.com",
            "(11) 98765-4321",
            new DateTime(1990, 5, 15)
        ).Value;

        var result = await repository.AdicionarAsync(cliente);

        result.Should().NotBeNull();
        result.Id.Should().Be(cliente.Id);
    }

    [Fact]
    public async Task ObterPorIdAsync_ComIdExistente_DeveRetornarCliente()
    {
        var repository = new ClienteRepository();
        var cliente = AgendamentoConsultas.Domain.Entities.Cliente.Criar(
            "João Silva",
            "123.456.789-09",
            "joao@email.com",
            "(11) 98765-4321",
            new DateTime(1990, 5, 15)
        ).Value;

        await repository.AdicionarAsync(cliente);
        var result = await repository.ObterPorIdAsync(cliente.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(cliente.Id);
        result.Nome.Should().Be(cliente.Nome);
    }

    [Fact]
    public async Task ObterPorIdAsync_ComIdInexistente_DeveRetornarNull()
    {
        var repository = new ClienteRepository();
        var id = Guid.NewGuid();

        var result = await repository.ObterPorIdAsync(id);

        result.Should().BeNull();
    }

    [Fact]
    public async Task ObterPorCpfAsync_ComCpfExistente_DeveRetornarCliente()
    {
        var repository = new ClienteRepository();
        var cliente = AgendamentoConsultas.Domain.Entities.Cliente.Criar(
            "João Silva",
            "123.456.789-09",
            "joao@email.com",
            "(11) 98765-4321",
            new DateTime(1990, 5, 15)
        ).Value;

        await repository.AdicionarAsync(cliente);
        var result = await repository.ObterPorCpfAsync("123.456.789-09");

        result.Should().NotBeNull();
        result!.Cpf.Should().Be(cliente.Cpf);
    }

    [Fact]
    public async Task ObterPorCpfAsync_ComCpfInexistente_DeveRetornarNull()
    {
        var repository = new ClienteRepository();

        var result = await repository.ObterPorCpfAsync("999.999.999-99");

        result.Should().BeNull();
    }

    [Fact]
    public async Task AtualizarAsync_ComClienteExistente_DeveAtualizarERetornarTrue()
    {
        var repository = new ClienteRepository();
        var cliente = AgendamentoConsultas.Domain.Entities.Cliente.Criar(
            "João Silva",
            "123.456.789-09",
            "joao@email.com",
            "(11) 98765-4321",
            new DateTime(1990, 5, 15)
        ).Value;

        await repository.AdicionarAsync(cliente);
        cliente.Atualizar("João Silva Santos", "joao.santos@email.com", "(11) 99999-8888", new DateTime(1990, 5, 15));

        var result = await repository.AtualizarAsync(cliente);

        result.Should().BeTrue();

        var clienteAtualizado = await repository.ObterPorIdAsync(cliente.Id);
        clienteAtualizado!.Nome.Should().Be("João Silva Santos");
    }

    [Fact]
    public async Task AtualizarAsync_ComClienteInexistente_DeveRetornarFalse()
    {
        var repository = new ClienteRepository();
        var cliente = AgendamentoConsultas.Domain.Entities.Cliente.Criar(
            "João Silva",
            "123.456.789-09",
            "joao@email.com",
            "(11) 98765-4321",
            new DateTime(1990, 5, 15)
        ).Value;

        var result = await repository.AtualizarAsync(cliente);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task RemoverAsync_ComClienteExistente_DeveRemoverERetornarTrue()
    {
        var repository = new ClienteRepository();
        var cliente = AgendamentoConsultas.Domain.Entities.Cliente.Criar(
            "João Silva",
            "123.456.789-09",
            "joao@email.com",
            "(11) 98765-4321",
            new DateTime(1990, 5, 15)
        ).Value;

        await repository.AdicionarAsync(cliente);
        var result = await repository.RemoverAsync(cliente.Id);

        result.Should().BeTrue();

        var clienteRemovido = await repository.ObterPorIdAsync(cliente.Id);
        clienteRemovido.Should().BeNull();
    }

    [Fact]
    public async Task RemoverAsync_ComClienteInexistente_DeveRetornarFalse()
    {
        var repository = new ClienteRepository();
        var id = Guid.NewGuid();

        var result = await repository.RemoverAsync(id);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task ObterTodosAsync_DeveRetornarTodosClientes()
    {
        var repository = new ClienteRepository();
        var cliente1 = AgendamentoConsultas.Domain.Entities.Cliente.Criar(
            "João Silva",
            "123.456.789-09",
            "joao@email.com",
            "(11) 98765-4321",
            new DateTime(1990, 5, 15)
        ).Value;

        var cliente2 = AgendamentoConsultas.Domain.Entities.Cliente.Criar(
            "Maria Silva",
            "987.654.321-00",
            "maria@email.com",
            "(11) 98765-1234",
            new DateTime(1985, 3, 10)
        ).Value;

        await repository.AdicionarAsync(cliente1);
        await repository.AdicionarAsync(cliente2);

        var result = await repository.ObterTodosAsync();

        result.Should().HaveCount(c => c >= 2);
    }
}
