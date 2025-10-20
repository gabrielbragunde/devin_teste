using AgendamentoConsultas.Application.DTOs.Cliente;
using FluentValidation;

namespace AgendamentoConsultas.Application.Validators.Cliente;

public class CriarClienteDtoValidator : AbstractValidator<CriarClienteDto>
{
    public CriarClienteDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome não pode ser vazio")
            .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres");

        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("CPF não pode ser vazio")
            .Must(ValidarCpf).WithMessage("CPF inválido");

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

    private bool ValidarCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        cpf = cpf.Replace(".", "").Replace("-", "").Trim();
        
        if (cpf.Length != 11)
            return false;
        
        if (!cpf.All(char.IsDigit))
            return false;
        
        if (cpf.Distinct().Count() == 1)
            return false;

        int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        
        string tempCpf = cpf.Substring(0, 9);
        int soma = 0;

        for (int i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

        int resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;

        string digito = resto.ToString();
        tempCpf += digito;
        soma = 0;

        for (int i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;
        digito += resto.ToString();

        return cpf.EndsWith(digito);
    }
}
