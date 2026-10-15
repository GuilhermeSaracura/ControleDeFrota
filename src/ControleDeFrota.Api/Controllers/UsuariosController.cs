using ControleDeFrota.Api.Dtos;
using ControleDeFrota.Api.Models;
using ControleDeFrota.Api.Repositories.Interfaces;
using ControleDeFrota.Api.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeFrota.Api.Controllers;

/// <summary>
/// Expõe os endpoints de gerenciamento de usuários da frota.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioRepository _repository;
    private readonly IVeiculoRepository _veiculoRepository;

    public UsuariosController(IUsuarioRepository repository, IVeiculoRepository veiculoRepository)
    {
        _repository = repository;
        _veiculoRepository = veiculoRepository;
    }

    /// <summary>
    /// Sincroniza o veículo vinculado ao usuário quando o relacionamento é informado.
    /// </summary>
    private async Task SincronizarVeiculoAsync(Usuario usuario)
    {
        if (usuario.VeiculoId.HasValue)
        {
            var veiculo = await _veiculoRepository.ObterPorIdAsync(usuario.VeiculoId.Value);
            if (veiculo is null)
            {
                return;
            }

            veiculo.UsuarioId = usuario.Id;
            await _veiculoRepository.AtualizarAsync(veiculo.Id, veiculo);
        }
    }

    /// <summary>
    /// Lista todos os usuários cadastrados.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UsuarioResponseDto>>> Get()
    {
        var usuarios = await _repository.ObterTodosAsync();
        return Ok(usuarios.Select(usuario => usuario.ToResponse()));
    }

    /// <summary>
    /// Busca um usuário pelo identificador informado.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UsuarioResponseDto>> GetById(Guid id)
    {
        var usuario = await _repository.ObterPorIdAsync(id);

        if (usuario is null)
        {
            return NotFound();
        }

        return Ok(usuario.ToResponse());
    }

    /// <summary>
    /// Cria um novo usuário e aplica a regra de vínculo com veículo quando necessário.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<UsuarioResponseDto>> Post(UsuarioRequestDto request)
    {
        var usuario = request.ToEntity();

        if (usuario.VeiculoId.HasValue)
        {
            if (usuario.TipoContratacao != TipoContratacao.Funcionario || string.IsNullOrWhiteSpace(usuario.NumeroHabilitacao))
            {
                return BadRequest("Funcionário deve ser do tipo Funcionário e possuir habilitação para ter veículo vinculado.");
            }

            var veiculo = await _veiculoRepository.ObterPorIdAsync(usuario.VeiculoId.Value);

            var erroVeiculo = RegraFrotaUtils.ValidarVinculoVeiculo(usuario, veiculo);
            if (!string.IsNullOrWhiteSpace(erroVeiculo))
            {
                return BadRequest(erroVeiculo);
            }
        }

        var resultado = await _repository.AdicionarAsync(usuario);

        await SincronizarVeiculoAsync(resultado);

        return CreatedAtAction(nameof(GetById), new { id = resultado.Id }, resultado.ToResponse());
    }

    /// <summary>
    /// Atualiza os dados de um usuário e mantém o vínculo com o veículo consistente.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put(Guid id, UsuarioRequestDto request)
    {
        var usuario = request.ToEntity();

        if (usuario.VeiculoId.HasValue)
        {
            if (usuario.TipoContratacao != TipoContratacao.Funcionario || string.IsNullOrWhiteSpace(usuario.NumeroHabilitacao))
            {
                return BadRequest("Funcionário deve ser do tipo Funcionário e possuir habilitação para ter veículo vinculado.");
            }

            var veiculo = await _veiculoRepository.ObterPorIdAsync(usuario.VeiculoId.Value);

            var erroVeiculo = RegraFrotaUtils.ValidarVinculoVeiculo(usuario, veiculo);
            if (!string.IsNullOrWhiteSpace(erroVeiculo))
            {
                return BadRequest(erroVeiculo);
            }
        }

        var resultado = await _repository.AtualizarAsync(id, usuario);

        if (resultado is null)
        {
            return NotFound();
        }

        await SincronizarVeiculoAsync(resultado);

        return NoContent();
    }

    /// <summary>
    /// Remove um usuário cadastrado.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var removido = await _repository.RemoverAsync(id);

        if (!removido)
        {
            return NotFound();
        }

        return NoContent();
    }
}
