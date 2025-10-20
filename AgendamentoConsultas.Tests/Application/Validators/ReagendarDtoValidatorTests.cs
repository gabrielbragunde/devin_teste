using AgendamentoConsultas.Application.DTOs.Agendamento;
using AgendamentoConsultas.Application.Validators.Agendamento;
using FluentAssertions;
using Xunit;

namespace AgendamentoConsultas.Tests.Application.Validators;

public class ReagendarDtoValidatorTests
{
    private readonly ReagendarDtoValidator _validator;

    public ReagendarDtoValidatorTests()
    {
        _validator = new ReagendarDtoValidator();
    }

    [Fact]
    public async Task Validate_ComDataHoraValida_DeveSerValido()
    {
        var dto = new ReagendarDto(
            DateTime.Now.AddDays(1).Date.AddHours(10)
        );

        var result = await _validator.ValidateAsync(dto);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ComDataHoraPassada_DeveSerInvalido()
    {
        var dto = new ReagendarDto(
            DateTime.Now.AddDays(-1)
        );

        var result = await _validator.ValidateAsync(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "NovaDataHora");
    }

    [Theory]
    [InlineData(15)]
    [InlineData(45)]
    public async Task Validate_ComMinutosInvalidos_DeveSerInvalido(int minutos)
    {
        var dto = new ReagendarDto(
            DateTime.Now.AddDays(1).Date.AddHours(10).AddMinutes(minutos)
        );

        var result = await _validator.ValidateAsync(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "NovaDataHora");
    }

    [Theory]
    [InlineData(7)]
    [InlineData(18)]
    public async Task Validate_ComHorarioForaDoAtendimento_DeveSerInvalido(int hora)
    {
        var dto = new ReagendarDto(
            DateTime.Now.AddDays(1).Date.AddHours(hora)
        );

        var result = await _validator.ValidateAsync(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "NovaDataHora");
    }

    [Fact]
    public async Task Validate_NoFinalDeSemana_DeveSerInvalido()
    {
        var dataHora = GetNextSaturday().AddHours(10);
        var dto = new ReagendarDto(dataHora);

        var result = await _validator.ValidateAsync(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "NovaDataHora");
    }

    private DateTime GetNextSaturday()
    {
        var date = DateTime.Now.AddDays(1);
        while (date.DayOfWeek != DayOfWeek.Saturday)
        {
            date = date.AddDays(1);
        }
        return date.Date;
    }
}
