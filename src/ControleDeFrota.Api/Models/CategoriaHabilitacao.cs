namespace ControleDeFrota.Api.Models;

/// <summary>
/// Descreve as categorias de habilitação do usuário para condução de veículos.
/// </summary>
[Flags]
public enum CategoriaHabilitacao
{
    Nenhuma = 0,
    B = 1,
    C = 2,
    BEC = B | C
}
