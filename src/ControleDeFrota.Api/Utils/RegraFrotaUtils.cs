using ControleDeFrota.Api.Models;

namespace ControleDeFrota.Api.Utils;

/// <summary>
/// Centraliza as regras de validação do vínculo entre usuários e veículos.
/// </summary>
public static class RegraFrotaUtils
{
    /// <summary>
    /// Verifica se a categoria de habilitação do usuário é compatível com a categoria do veículo.
    /// </summary>
    public static bool PossuiCategoriaParaVeiculo(Usuario usuario, Veiculo veiculo)
    {
        return usuario.CategoriasHabilitacao.HasFlag(veiculo.Categoria);
    }

    /// <summary>
    /// Valida se o vínculo entre usuário e veículo é permitido pelo conjunto de regras da frota.
    /// </summary>
    public static string? ValidarVinculoVeiculo(Usuario usuario, Veiculo? veiculo)
    {
        if (veiculo is null)
        {
            return "Veículo informado não foi encontrado.";
        }

        if (!PossuiCategoriaParaVeiculo(usuario, veiculo))
        {
            return "Funcionário não está habilitado para a categoria do veículo informado.";
        }

        if (usuario.VeiculoId != veiculo.Id)
        {
            return "Funcionário já está vinculado a outro veículo.";
        }

        return null;
    }
}
