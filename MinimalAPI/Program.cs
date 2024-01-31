using MinimalAPI.Entidades;

var builder = WebApplication.CreateBuilder(args);


//Servicios

//Fin
var app = builder.Build();

//Middleware
app.MapGet("/", () => "Hello World!");
app.MapGet("/generos", () => 
{
    var generos = new List<Genero>
    {
        new Genero { Id = 1, Nombre = "Drama"},
        new Genero { Id = 2, Nombre = "Accion"},
        new Genero { Id = 3, Nombre = "Comedia"}
    };

    return generos;
});


//Fin
app.Run();
