namespace ControleDeFrota.Api.Models;

/// <summary>
/// Representa um usuário da empresa e seus dados de vínculo com a frota.
/// </summary>
public class Usuario
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Nome { get; set; }
    public required string Email { get; set; }
    public string Departamento { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public TipoContratacao TipoContratacao { get; set; }
    public string NumeroHabilitacao { get; set; } = string.Empty;
    public CategoriaHabilitacao CategoriasHabilitacao { get; set; } = CategoriaHabilitacao.Nenhuma;
    public Guid? VeiculoId { get; set; }
}
