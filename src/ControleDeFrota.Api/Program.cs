using ControleDeFrota.Api.Repositories;
using ControleDeFrota.Api.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddSingleton<IVeiculoRepository, InMemoryVeiculoRepository>();
builder.Services.AddSingleton<IUsuarioRepository, InMemoryUsuarioRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDefaultFiles();
    app.UseStaticFiles();
    app.MapGet("/swagger", () => Results.Redirect("/swagger/index.html"));
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
