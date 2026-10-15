using ControleDeFrota.Api.Dtos;
using ControleDeFrota.Api.Models;
using ControleDeFrota.Api.Repositories.Interfaces;
using ControleDeFrota.Api.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeFrota.Api.Controllers;

/// <summary>
/// Expõe os endpoints de gerenciamento de veículos da frota.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class VeiculosController : ControllerBase
{
    private readonly IVeiculoRepository _repository;
    private readonly IUsuarioRepository _usuarioRepository;

    public VeiculosController(IVeiculoRepository repository, IUsuarioRepository usuarioRepository)
    {
        _repository = repository;
        _usuarioRepository = usuarioRepository;
    }

    /// <summary>
    /// Sincroniza o usuário vinculado ao veículo quando o relacionamento é informado.
    /// </summary>
    private async Task SincronizarUsuarioAsync(Veiculo veiculo)
    {
        if (veiculo.UsuarioId.HasValue)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(veiculo.UsuarioId.Value);
            if (usuario is null)
            {
                return;
            }

            usuario.VeiculoId = veiculo.Id;
            await _usuarioRepository.AtualizarAsync(usuario.Id, usuario);
        }
    }

    /// <summary>
    /// Lista todos os veículos cadastrados.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VeiculoResponseDto>>> Get()
    {
        var veiculos = await _repository.ObterTodosAsync();
        return Ok(veiculos.Select(veiculo => veiculo.ToResponse()));
    }

    /// <summary>
    /// Busca um veículo pelo identificador informado.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<VeiculoResponseDto>> GetById(Guid id)
    {
        var veiculo = await _repository.ObterPorIdAsync(id);

        if (veiculo is null)
        {
            return NotFound();
        }

        return Ok(veiculo.ToResponse());
    }

    /// <summary>
    /// Cria um novo veículo e aplica a sincronização do vínculo com o usuário, quando houver.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<VeiculoResponseDto>> Post(VeiculoRequestDto request)
    {
        var veiculo = request.ToEntity();

        await SincronizarUsuarioAsync(veiculo);

        return CreatedAtAction(nameof(GetById), new { id = veiculo.Id }, veiculo.ToResponse());
    }

    /// <summary>
    /// Atualiza um veículo existente e mantém o vínculo com o usuário consistente.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put(Guid id, VeiculoUpdateRequestDto request)
    {
        var veiculo = request.ToEntity();

        if (veiculo.UsuarioId.HasValue)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(veiculo.UsuarioId.Value);
            if (usuario is null)
            {
                return BadRequest("Usuário informado não foi encontrado.");
            }

            if (usuario.TipoContratacao != TipoContratacao.Funcionario || string.IsNullOrWhiteSpace(usuario.NumeroHabilitacao))
            {
                return BadRequest("Funcionário deve ser do tipo Funcionário e possuir habilitação para ter veículo vinculado.");
            }

            var erro = RegraFrotaUtils.ValidarVinculoVeiculo(usuario, veiculo);
            if (!string.IsNullOrWhiteSpace(erro))
            {
                return BadRequest(erro);
            }
        }

        var resultado = await _repository.AtualizarAsync(id, veiculo);

        if (resultado is null)
        {
            return NotFound();
        }

        if (resultado.UsuarioId.HasValue)
        {
            await SincronizarUsuarioAsync(resultado);
        }

        return NoContent();
    }

    /// <summary>
    /// Remove um veículo cadastrado.
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
