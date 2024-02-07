using MinimalAPI.Entidades;

namespace MinimalAPI.Repositorios
{
    public interface IRepositorioGeneros
    {
        Task<bool> Existe(int id);
        Task<Genero?> ObtenerPorId(int id);
        Task<List<Genero>> ObtenerTodos();
        Task<int> Crear(Genero genero);
        Task Actualizar(Genero genero);

        Task Borrar(int id);
    }
}
