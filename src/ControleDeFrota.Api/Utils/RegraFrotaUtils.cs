using ControleDeFrota.Api.Models;

namespace ControleDeFrota.Api.Utils;

public static class RegraFrotaUtils
{
    public static bool PossuiCategoriaParaVeiculo(Usuario usuario, Veiculo veiculo)
    {
        return usuario.CategoriasHabilitacao.HasFlag(veiculo.Categoria);
    }

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
