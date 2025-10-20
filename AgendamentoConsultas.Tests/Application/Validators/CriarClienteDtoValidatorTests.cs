using AgendamentoConsultas.Application.DTOs.Cliente;
using AgendamentoConsultas.Application.Validators.Cliente;
using FluentAssertions;
using Xunit;

namespace AgendamentoConsultas.Tests.Application.Validators;

public class CriarClienteDtoValidatorTests
{
    private readonly CriarClienteDtoValidator _validator;

    public CriarClienteDtoValidatorTests()
    {
        _validator = new CriarClienteDtoValidator();
    }

    [Fact]
    public async Task Validate_ComDadosValidos_DeveSerValido()
    {
        var dto = new CriarClienteDto(
            "João Silva",
            "123.456.789-09",
            "joao@email.com",
            "(11) 98765-4321",
            new DateTime(1990, 5, 15)
        );

        var result = await _validator.ValidateAsync(dto);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("Jo")]
    public async Task Validate_ComNomeInvalido_DeveSerInvalido(string nome)
    {
        var dto = new CriarClienteDto(
            nome,
            "123.456.789-09",
            "joao@email.com",
            "(11) 98765-4321",
            new DateTime(1990, 5, 15)
        );

        var result = await _validator.ValidateAsync(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Nome");
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("111.111.111-11")]
    [InlineData("000.000.000-00")]
    public async Task Validate_ComCpfInvalido_DeveSerInvalido(string cpf)
    {
        var dto = new CriarClienteDto(
            "João Silva",
            cpf,
            "joao@email.com",
            "(11) 98765-4321",
            new DateTime(1990, 5, 15)
        );

        var result = await _validator.ValidateAsync(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Cpf");
    }

    [Theory]
    [InlineData("")]
    [InlineData("email-invalido")]
    [InlineData("@email.com")]
    public async Task Validate_ComEmailInvalido_DeveSerInvalido(string email)
    {
        var dto = new CriarClienteDto(
            "João Silva",
            "123.456.789-09",
            email,
            "(11) 98765-4321",
            new DateTime(1990, 5, 15)
        );

        var result = await _validator.ValidateAsync(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    public async Task Validate_ComTelefoneInvalido_DeveSerInvalido(string telefone)
    {
        var dto = new CriarClienteDto(
            "João Silva",
            "123.456.789-09",
            "joao@email.com",
            telefone,
            new DateTime(1990, 5, 15)
        );

        var result = await _validator.ValidateAsync(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Telefone");
    }

    [Fact]
    public async Task Validate_ComDataNascimentoFutura_DeveSerInvalido()
    {
        var dto = new CriarClienteDto(
            "João Silva",
            "123.456.789-09",
            "joao@email.com",
            "(11) 98765-4321",
            DateTime.Today.AddDays(1)
        );

        var result = await _validator.ValidateAsync(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DataNascimento");
    }
}
