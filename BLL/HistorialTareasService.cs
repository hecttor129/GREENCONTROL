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
    public class HistorialTareasService : ICrudEscritura<HistorialTareas>, ICrudLectura<HistorialTareas>
    {
        private readonly HistorialTareasRepository historialTareasRepository;

        public HistorialTareasService()
        {
            historialTareasRepository = new HistorialTareasRepository();
        }

        public string Guardar(HistorialTareas entidad)
        {
            var response = historialTareasRepository.Insertar(entidad);
            return response.Mensaje;
        }

        public bool Actualizar(HistorialTareas entidad)
        {
            var response = historialTareasRepository.Actualizar(entidad);
            return response.Estado;
        }

        public bool Eliminar(HistorialTareas entidad)
        {
            var response = historialTareasRepository.Eliminar(entidad.IdHistorialTareas);
            return response.Estado;
        }

        public ReadOnlyCollection<HistorialTareas> Consultar()
        {
            var response = historialTareasRepository.ObtenerTodos();
            var lista = response.Lista ?? new List<HistorialTareas>();
            return new ReadOnlyCollection<HistorialTareas>(lista);
        }

        public HistorialTareas ObtenerPorId(string id)
        {
            var response = historialTareasRepository.ObtenerPorId(Convert.ToInt32(id));
            return response.Entidad;
        }

        public HistorialTareas CrearHistorialDesdeTarea(
               Tareas tarea,
               Parcela parcela
               )
        {
            return new HistorialTareas
            {
                IdHistorialTareas = tarea.IdTarea,
                IdParcela = parcela.IdParcela,

                // Datos de la tarea
                Tipo = tarea.Tipo,
                Estado = tarea.Estado,
                FechaTarea = tarea.FechaTarea,   
                Descripcion = tarea.Descripcion,

            };
        }

    }
}
