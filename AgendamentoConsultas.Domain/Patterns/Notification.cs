namespace AgendamentoConsultas.Domain.Patterns;

public class Notification
{
    private readonly List<string> _errors;

    public Notification()
    {
        _errors = new List<string>();
    }

    public IReadOnlyCollection<string> Errors => _errors.AsReadOnly();
    public bool HasErrors => _errors.Any();
    public bool IsValid => !HasErrors;

    public void AddError(string error)
    {
        if (!string.IsNullOrWhiteSpace(error))
            _errors.Add(error);
    }

    public void AddErrors(IEnumerable<string> errors)
    {
        if (errors != null)
            _errors.AddRange(errors.Where(e => !string.IsNullOrWhiteSpace(e)));
    }

    public string GetErrorsAsString(string separator = "; ")
    {
        return string.Join(separator, _errors);
    }

    public void Clear()
    {
        _errors.Clear();
    }
}
