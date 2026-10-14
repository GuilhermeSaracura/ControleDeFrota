using ControleDeFrota.Api.Models;
using ControleDeFrota.Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeFrota.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioRepository _repository;

    public UsuariosController(IUsuarioRepository repository)
    {
        _repository = repository;
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
        var resultado = await _repository.AdicionarAsync(usuario);
        return CreatedAtAction(nameof(GetById), new { id = resultado.Id }, resultado);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put(Guid id, Usuario usuario)
    {
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
