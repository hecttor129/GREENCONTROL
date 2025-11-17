using DAL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class IngresosService : ICrudEscritura<Ingresos>, ICrudLectura<Ingresos>
    {
        private readonly IngresosRepository ingresosRepository;

        public IngresosService()
        {
            ingresosRepository = new IngresosRepository();
        }

        public string Guardar(Ingresos entidad)
        {
            var response = ingresosRepository.Insertar(entidad);
            return response.Mensaje;
        }

        public bool Actualizar(Ingresos entidad)
        {
            var response = ingresosRepository.Actualizar(entidad);
            return response.Estado;
        }

        public bool Eliminar(Ingresos entidad)
        {
            var response = ingresosRepository.Eliminar(entidad.IdIngresos);
            return response.Estado;
        }

        public ReadOnlyCollection<Ingresos> Consultar()
        {
            var response = ingresosRepository.ObtenerTodos();
            var lista = response.Lista ?? new List<Ingresos>();
            return new ReadOnlyCollection<Ingresos>(lista);
        }

        public Ingresos ObtenerPorId(string id)
        {
            var response = ingresosRepository.ObtenerPorId(Convert.ToInt32(id));
            return response.Entidad;
        }


    }

}
