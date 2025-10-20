using AgendamentoConsultas.Application.DTOs.Cliente;
using FluentValidation;

namespace AgendamentoConsultas.Application.Validators.Cliente;

public class AtualizarClienteDtoValidator : AbstractValidator<AtualizarClienteDto>
{
    public AtualizarClienteDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome não pode ser vazio")
            .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email não pode ser vazio")
            .EmailAddress().WithMessage("Email inválido")
            .MaximumLength(200).WithMessage("Email deve ter no máximo 200 caracteres");

        RuleFor(x => x.Telefone)
            .NotEmpty().WithMessage("Telefone não pode ser vazio")
            .MinimumLength(10).WithMessage("Telefone inválido")
            .MaximumLength(20).WithMessage("Telefone deve ter no máximo 20 caracteres");

        RuleFor(x => x.DataNascimento)
            .LessThan(DateTime.Today).WithMessage("Data de nascimento inválida")
            .GreaterThan(DateTime.Today.AddYears(-150)).WithMessage("Data de nascimento inválida");
    }
}
