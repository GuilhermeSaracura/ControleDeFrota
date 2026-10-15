using ControleDeFrota.Api.Dtos;
using ControleDeFrota.Api.Models;
using ControleDeFrota.Api.Repositories.Interfaces;
using ControleDeFrota.Api.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeFrota.Api.Controllers;

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

    private async Task SincronizarVeiculoAsync(Usuario usuario)
    {
        var veiculo = await _veiculoRepository.ObterPorIdAsync(usuario.VeiculoId.Value);
        if (veiculo is null)
        {
            return;
        }

        veiculo.UsuarioId = usuario.Id;
        await _veiculoRepository.AtualizarAsync(veiculo.Id, veiculo);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UsuarioResponseDto>>> Get()
    {
        var usuarios = await _repository.ObterTodosAsync();
        return Ok(usuarios.Select(usuario => usuario.ToResponse()));
    }

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

        if (resultado.VeiculoId.HasValue)
        {
            await SincronizarVeiculoAsync(resultado);
        }

        return CreatedAtAction(nameof(GetById), new { id = resultado.Id }, resultado.ToResponse());
    }

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

        if (resultado.VeiculoId.HasValue)
        {
            await SincronizarVeiculoAsync(resultado);
        }

        return NoContent();
    }

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
