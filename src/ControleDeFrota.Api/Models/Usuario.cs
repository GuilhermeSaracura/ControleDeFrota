namespace ControleDeFrota.Api.Models;

public class Usuario
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
}
