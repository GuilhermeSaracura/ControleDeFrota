namespace ControleDeFrota.Api.Models;

/// <summary>
/// Representa um veículo da frota e seu vínculo com o responsável pelo uso.
/// </summary>
public class Veiculo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Placa { get; set; }
    public required string Modelo { get; set; }
    public int Ano { get; set; }
    public required string Cor { get; set; }
    public bool Disponivel { get; set; } = true;
    public CategoriaHabilitacao Categoria { get; set; }
    public Guid? UsuarioId { get; set; }
}
