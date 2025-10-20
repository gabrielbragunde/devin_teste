using AgendamentoConsultas.Application.DTOs.Agendamento;
using FluentValidation;

namespace AgendamentoConsultas.Application.Validators.Agendamento;

public class CriarAgendamentoDtoValidator : AbstractValidator<CriarAgendamentoDto>
{
    public CriarAgendamentoDtoValidator()
    {
        RuleFor(x => x.ClienteId)
            .NotEmpty().WithMessage("ClienteId não pode ser vazio");

        RuleFor(x => x.DataHora)
            .GreaterThan(DateTime.Now).WithMessage("Data e hora do agendamento deve ser futura")
            .Must(ValidarHorario).WithMessage("Agendamentos devem ser em horários de 30 em 30 minutos")
            .Must(ValidarHorarioAtendimento).WithMessage("Horário de atendimento é das 08:00 às 18:00")
            .Must(ValidarDiaSemana).WithMessage("Não há atendimento aos finais de semana");

        RuleFor(x => x.Observacoes)
            .MaximumLength(500).WithMessage("Observações devem ter no máximo 500 caracteres");
    }

    private bool ValidarHorario(DateTime dataHora)
    {
        return dataHora.Minute == 0 || dataHora.Minute == 30;
    }

    private bool ValidarHorarioAtendimento(DateTime dataHora)
    {
        return dataHora.Hour >= 8 && dataHora.Hour < 18;
    }

    private bool ValidarDiaSemana(DateTime dataHora)
    {
        return dataHora.DayOfWeek != DayOfWeek.Saturday && dataHora.DayOfWeek != DayOfWeek.Sunday;
    }
}
