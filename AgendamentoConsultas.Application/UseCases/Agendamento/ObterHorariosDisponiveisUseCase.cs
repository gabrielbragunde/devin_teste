using AgendamentoConsultas.Application.DTOs.Agendamento;
using AgendamentoConsultas.Application.Interfaces;

namespace AgendamentoConsultas.Application.UseCases.Agendamento;

public class ObterHorariosDisponiveisUseCase
{
    private readonly IAgendamentoRepository _agendamentoRepository;

    public ObterHorariosDisponiveisUseCase(IAgendamentoRepository agendamentoRepository)
    {
        _agendamentoRepository = agendamentoRepository;
    }

    public async Task<IEnumerable<HorarioDisponivelDto>> ExecutarAsync(DateTime data)
    {
        if (data.Date < DateTime.Today)
            throw new ArgumentException("Não é possível consultar horários de datas passadas");

        if (data.DayOfWeek == DayOfWeek.Saturday || data.DayOfWeek == DayOfWeek.Sunday)
            return Enumerable.Empty<HorarioDisponivelDto>();

        var agendamentos = await _agendamentoRepository.ObterPorDataAsync(data.Date);
        var horariosOcupados = agendamentos
            .Where(a => a.Status == Domain.Entities.StatusAgendamento.Agendado)
            .Select(a => a.DataHora)
            .ToHashSet();

        var horarios = new List<HorarioDisponivelDto>();
        var dataAtual = data.Date.AddHours(8);
        var dataFinal = data.Date.AddHours(18);

        while (dataAtual < dataFinal)
        {
            var disponivel = !horariosOcupados.Contains(dataAtual) && dataAtual > DateTime.Now;
            horarios.Add(new HorarioDisponivelDto(dataAtual, disponivel));
            dataAtual = dataAtual.AddMinutes(30);
        }

        return horarios;
    }
}
