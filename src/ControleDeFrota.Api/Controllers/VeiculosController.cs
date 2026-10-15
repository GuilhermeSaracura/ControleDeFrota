using ControleDeFrota.Api.Dtos;
using ControleDeFrota.Api.Models;
using ControleDeFrota.Api.Repositories.Interfaces;
using ControleDeFrota.Api.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeFrota.Api.Controllers;

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

    private async Task SincronizarUsuarioAsync(Veiculo veiculo)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(veiculo.UsuarioId.Value);
        if (usuario is null)
        {
            return;
        }

        usuario.VeiculoId = veiculo.Id;
        await _usuarioRepository.AtualizarAsync(usuario.Id, usuario);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VeiculoResponseDto>>> Get()
    {
        var veiculos = await _repository.ObterTodosAsync();
        return Ok(veiculos.Select(veiculo => veiculo.ToResponse()));
    }

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

    [HttpPost]
    public async Task<ActionResult<VeiculoResponseDto>> Post(VeiculoRequestDto request)
    {
        var veiculo = request.ToEntity();

        var resultado = await _repository.AdicionarAsync(veiculo);

        if (resultado.UsuarioId.HasValue)
        {
            await SincronizarUsuarioAsync(resultado);
        }

        return CreatedAtAction(nameof(GetById), new { id = resultado.Id }, resultado.ToResponse());
    }

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
