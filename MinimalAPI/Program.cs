using Microsoft.AspNetCore.Cors;
using MinimalAPI.Entidades;

var builder = WebApplication.CreateBuilder(args);
var origenesPermitidos = builder.Configuration.GetValue<string>("origenespermitidos")!;

//Servicios
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
app.MapGet("/generos", () => 
{
    var generos = new List<Genero>
    {
        new Genero { Id = 1, Nombre = "Drama"},
        new Genero { Id = 2, Nombre = "Accion"},
        new Genero { Id = 3, Nombre = "Comedia"}
    };

    return generos;
}).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(15)));

//Fin

app.Run();
