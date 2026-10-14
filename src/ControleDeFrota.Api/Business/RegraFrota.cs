using ControleDeFrota.Api.Models;

namespace ControleDeFrota.Api.Business;

public static class RegraFrota
{
    public static bool EhEstagiarioOuAprendiz(Usuario usuario)
    {
        return usuario.TipoContratacao == TipoContratacao.Estagiario ||
               usuario.TipoContratacao == TipoContratacao.Aprendiz;
    }

    public static bool PossuiCategoriaParaVeiculo(Usuario usuario, Veiculo veiculo)
    {
        return usuario.CategoriasHabilitacao.HasFlag(veiculo.Categoria);
    }

    public static string? ValidarUsuario(Usuario usuario)
    {
        if (EhEstagiarioOuAprendiz(usuario) && usuario.VeiculoId.HasValue)
        {
            return "Estagiários e aprendizes não podem ter nenhum veículo vinculado.";
        }

        if (!usuario.PossuiHabilitacao && usuario.VeiculoId.HasValue)
        {
            return "Funcionário sem habilitação não pode ter veículo vinculado.";
        }

        if (!usuario.PossuiHabilitacao && usuario.CategoriasHabilitacao != CategoriaHabilitacao.Nenhuma)
        {
            return "Funcionário sem habilitação não pode possuir categorias de direção cadastradas.";
        }

        if (usuario.PossuiHabilitacao && string.IsNullOrWhiteSpace(usuario.NumeroHabilitacao))
        {
            return "O número da habilitação é obrigatório quando o funcionário possui habilitação.";
        }

        if (usuario.PossuiHabilitacao && usuario.CategoriasHabilitacao == CategoriaHabilitacao.Nenhuma)
        {
            return "Funcionário com habilitação deve informar ao menos uma categoria de veículo.";
        }

        if (EhEstagiarioOuAprendiz(usuario) && usuario.PossuiHabilitacao)
        {
            return "Estagiários e aprendizes não podem estar aptos a conduzir veículos.";
        }

        return null;
    }

    public static string? ValidarVinculoVeiculo(Usuario? usuario, Veiculo veiculo)
    {
        if (usuario is null)
        {
            return "Usuário informado não foi encontrado.";
        }

        if (EhEstagiarioOuAprendiz(usuario))
        {
            return "Estagiários e aprendizes não podem ter veículos vinculados.";
        }

        if (!usuario.PossuiHabilitacao)
        {
            return "Funcionário sem habilitação não pode ter veículo vinculado.";
        }

        if (string.IsNullOrWhiteSpace(usuario.NumeroHabilitacao))
        {
            return "O número da habilitação deve ser informado.";
        }

        if (usuario.CategoriasHabilitacao == CategoriaHabilitacao.Nenhuma)
        {
            return "Funcionário com habilitação deve possuir ao menos uma categoria cadastrada.";
        }

        if (!PossuiCategoriaParaVeiculo(usuario, veiculo))
        {
            return "Funcionário não está habilitado para a categoria do veículo informado.";
        }

        if (usuario.VeiculoId.HasValue && usuario.VeiculoId != veiculo.Id)
        {
            return "Funcionário já está vinculado a outro veículo.";
        }

        return null;
    }
}
