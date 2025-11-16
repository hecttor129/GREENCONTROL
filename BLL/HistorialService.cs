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
    public class HistorialService : ICrudEscritura<Historial>, ICrudLectura<Historial>
    {
        private readonly HistorialRepository historialRepository;

        public HistorialService()
        {
            historialRepository = new HistorialRepository();
        }

        public string Guardar(Historial entidad)
        {
            var response = historialRepository.Insertar(entidad);
            return response.Mensaje;
        }

        public bool Actualizar(Historial entidad)
        {
            var response = historialRepository.Actualizar(entidad);
            return response.Estado;
        }

        public bool Eliminar(Historial entidad)
        {
            var response = historialRepository.Eliminar(entidad.IdHistorial);
            return response.Estado;
        }

        public ReadOnlyCollection<Historial> Consultar()
        {
            var response = historialRepository.ObtenerTodos();
            var lista = response.Lista ?? new List<Historial>();
            return new ReadOnlyCollection<Historial>(lista);
        }

        public Historial ObtenerPorId(string id)
        {
            var response = historialRepository.ObtenerPorId(Convert.ToInt32(id));
            return response.Entidad;
        }
    }
}
