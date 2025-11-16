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
    public class NotificacionesService : ICrudEscritura<Notificaciones>, ICrudLectura<Notificaciones>
    {
        private readonly NotificacionesRepository notificacionesRepository;

        public NotificacionesService()
        {
            notificacionesRepository = new NotificacionesRepository();
        }

        public string Guardar(Notificaciones entidad)
        {
            var response = notificacionesRepository.Insertar(entidad);
            return response.Mensaje;
        }

        public bool Actualizar(Notificaciones entidad)
        {
            var response = notificacionesRepository.Actualizar(entidad);
            return response.Estado;
        }

        public bool Eliminar(Notificaciones entidad)
        {
            var response = notificacionesRepository.Eliminar(entidad.IdNotificacion);
            return response.Estado;
        }

        public ReadOnlyCollection<Notificaciones> Consultar()
        {
            var response = notificacionesRepository.ObtenerTodos();
            var lista = response.Lista ?? new List<Notificaciones>();
            return new ReadOnlyCollection<Notificaciones>(lista);
        }

        public Notificaciones ObtenerPorId(string id)
        {
            var response = notificacionesRepository.ObtenerPorId(Convert.ToInt32(id));
            return response.Entidad;
        }
    }
}
