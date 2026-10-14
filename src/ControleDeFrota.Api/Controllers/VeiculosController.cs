using ControleDeFrota.Api.Models;
using ControleDeFrota.Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeFrota.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VeiculosController : ControllerBase
{
    private readonly IVeiculoRepository _repository;

    public VeiculosController(IVeiculoRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Veiculo>>> Get()
    {
        var veiculos = await _repository.ObterTodosAsync();
        return Ok(veiculos);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Veiculo>> GetById(Guid id)
    {
        var veiculo = await _repository.ObterPorIdAsync(id);

        if (veiculo is null)
        {
            return NotFound();
        }

        return Ok(veiculo);
    }

    [HttpPost]
    public async Task<ActionResult<Veiculo>> Post(Veiculo veiculo)
    {
        var resultado = await _repository.AdicionarAsync(veiculo);
        return CreatedAtAction(nameof(GetById), new { id = resultado.Id }, resultado);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put(Guid id, Veiculo veiculo)
    {
        var resultado = await _repository.AtualizarAsync(id, veiculo);

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
