namespace ControleDeFrota.Api.Models;

public class Veiculo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Placa { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int Ano { get; set; }
    public string Cor { get; set; } = string.Empty;
    public bool Disponivel { get; set; } = true;
    public Guid? UsuarioId { get; set; }
}
