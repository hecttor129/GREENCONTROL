using DAL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BLL
{
    public class TareasService : ICrudEscritura<Tareas>, ICrudLectura<Tareas>
    {
        private readonly TareasRepository tareasRepository;
        private readonly ParcelaRepository parcelaRepository;

        public TareasService()
        {
            tareasRepository = new TareasRepository();
            parcelaRepository = new ParcelaRepository();
        }

        public string Guardar(Tareas entidad)
        {
            var response = tareasRepository.Insertar(entidad);
            return response.Mensaje;
        }
        public bool Actualizar(Tareas entidad)
        {
            var response = tareasRepository.Actualizar(entidad);
            return response.Estado;
        }
        public bool Eliminar(Tareas entidad)
        {
            var response = tareasRepository.Eliminar(entidad.IdTarea);
            return response.Estado;
        }
        public ReadOnlyCollection<Tareas> Consultar()
        {
            var response = tareasRepository.ObtenerTodos();
            var lista = response.Lista ?? new List<Tareas>();
            return new ReadOnlyCollection<Tareas>(lista);
        }
        public Tareas ObtenerPorId(string id)
        {
            var response = tareasRepository.ObtenerPorId(Convert.ToInt32(id));
            return response.Entidad;
        }

    }
}
