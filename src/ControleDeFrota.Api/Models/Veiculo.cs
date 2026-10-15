namespace ControleDeFrota.Api.Models;

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
