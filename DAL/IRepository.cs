using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface IRepository<T>
    {
        Response<T> Insertar(T entidad);
        Response<T> Actualizar(T entidad);
        Response<T> Eliminar(int id);
        Response<T> ObtenerPorId(int id);
        Response<T> ObtenerTodos();

    }
}
