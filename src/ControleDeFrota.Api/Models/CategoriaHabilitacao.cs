namespace ControleDeFrota.Api.Models;

[Flags]
public enum CategoriaHabilitacao
{
    Nenhuma = 0,
    B = 1,
    C = 2,
    BEC = B | C
}
