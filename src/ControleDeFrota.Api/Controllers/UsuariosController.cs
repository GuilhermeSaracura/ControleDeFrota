using ControleDeFrota.Api.Business;
using ControleDeFrota.Api.Models;
using ControleDeFrota.Api.Repositories.Interfaces;
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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Usuario>>> Get()
    {
        var usuarios = await _repository.ObterTodosAsync();
        return Ok(usuarios);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Usuario>> GetById(Guid id)
    {
        var usuario = await _repository.ObterPorIdAsync(id);

        if (usuario is null)
        {
            return NotFound();
        }

        return Ok(usuario);
    }

    [HttpPost]
    public async Task<ActionResult<Usuario>> Post(Usuario usuario)
    {
        var mensagem = RegraFrota.ValidarUsuario(usuario);
        if (!string.IsNullOrWhiteSpace(mensagem))
        {
            return BadRequest(mensagem);
        }

        if (usuario.VeiculoId.HasValue)
        {
            var veiculo = await _veiculoRepository.ObterPorIdAsync(usuario.VeiculoId.Value);
            var erroVeiculo = RegraFrota.ValidarVinculoVeiculo(usuario, veiculo!);

            if (veiculo is null)
            {
                return BadRequest("Veículo informado não foi encontrado.");
            }

            if (!string.IsNullOrWhiteSpace(erroVeiculo))
            {
                return BadRequest(erroVeiculo);
            }
        }

        var resultado = await _repository.AdicionarAsync(usuario);
        return CreatedAtAction(nameof(GetById), new { id = resultado.Id }, resultado);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put(Guid id, Usuario usuario)
    {
        var mensagem = RegraFrota.ValidarUsuario(usuario);
        if (!string.IsNullOrWhiteSpace(mensagem))
        {
            return BadRequest(mensagem);
        }

        if (usuario.VeiculoId.HasValue)
        {
            var veiculo = await _veiculoRepository.ObterPorIdAsync(usuario.VeiculoId.Value);
            if (veiculo is null)
            {
                return BadRequest("Veículo informado não foi encontrado.");
            }

            var erroVeiculo = RegraFrota.ValidarVinculoVeiculo(usuario, veiculo);
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
