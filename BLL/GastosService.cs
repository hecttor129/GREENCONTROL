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
    public class GastosService : ICrudEscritura<Gastos>, ICrudLectura<Gastos>
    {
        private readonly GastosRepository gastoRepository;

        public GastosService()
        {
            gastoRepository = new GastosRepository();

        }

        public string Guardar(Gastos entidad)
        {
            var response = gastoRepository.Insertar(entidad);
            return response.Mensaje;
        }

        public bool Actualizar(Gastos entidad)
        {
            var response = gastoRepository.Actualizar(entidad);
            return response.Estado;
        }

        public bool Eliminar(Gastos entidad)
        {
            var response = gastoRepository.Eliminar(entidad.IdGasto);
            return response.Estado;
        }

        public ReadOnlyCollection<Gastos> Consultar()
        {
            var response = gastoRepository.ObtenerTodos();
            var lista = response.Lista ?? new List<Gastos>();
            return new ReadOnlyCollection<Gastos>(lista);
        }

        public Gastos ObtenerPorId(string id)
        {
            var response = gastoRepository.ObtenerPorId(Convert.ToInt32(id));
            return response.Entidad;
        }
    }
}
