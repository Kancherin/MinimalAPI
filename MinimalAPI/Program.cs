using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using MinimalAPI;
using MinimalAPI.Entidades;
using MinimalAPI.Repositorios;

var builder = WebApplication.CreateBuilder(args);
var origenesPermitidos = builder.Configuration.GetValue<string>("origenespermitidos")!;

//Servicios
builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
    opciones.UseSqlServer("name=DefaultConnection"));

builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(configuracion =>
    {
        configuracion.WithOrigins(origenesPermitidos).AllowAnyHeader().AllowAnyMethod();
    });

    opciones.AddPolicy("libre", configuracion =>
    {
        configuracion.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});
builder.Services.AddOutputCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IRepositorioGeneros, RepositorioGeneros>();

//Fin

var app = builder.Build();

//Middleware
//if (builder.Environment.IsDevelopment())
//{

//}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();
app.UseOutputCache();
app.MapGet("/", [EnableCors(policyName: "libre")]() => "Hello World!");

var endpointsGeneros = app.MapGroup("/generos");

endpointsGeneros.MapGet("/", async (IRepositorioGeneros repositorio) => 
{
    return await repositorio.ObtenerTodos();
}).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("generos-get"));

endpointsGeneros.MapGet("/{id:int}", async (IRepositorioGeneros repositorio, int id) => 
{
    var genero = await repositorio.ObtenerPorId(id);

    if (genero is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(genero);

});

endpointsGeneros.MapPost("/", async (Genero genero, IRepositorioGeneros repositorio, 
    IOutputCacheStore outputCacheStore) =>
{
    var id = await repositorio.Crear(genero);
    await outputCacheStore.EvictByTagAsync("generos-get", default);
    return Results.Created($"/generos/{id}", genero);
});

endpointsGeneros.MapPut("/{id:int}", async (int id, Genero genero, IRepositorioGeneros repositorio,
        IOutputCacheStore outputCacheStore) =>
{
    var existe = await repositorio.Existe(id);

    if (!existe)
    {
        return TypedResults.NotFound();
    }

    await repositorio.Actualizar(genero);
    await outputCacheStore.EvictByTagAsync("generos-get", default);
    return Results.NoContent();
});

endpointsGeneros.MapDelete("/{id:int}", async (int id, IRepositorioGeneros repositorio,
    IOutputCacheStore outputCacheStore) =>
{
    var existe = await repositorio.Existe(id);

    if (!existe)
    {
        return TypedResults.NotFound();
    }

    await repositorio.Borrar(id);
    await outputCacheStore.EvictByTagAsync("generos-get", default);
    return Results.NoContent();
});

//Fin

app.Run();
