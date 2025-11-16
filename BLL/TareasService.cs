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
            var error = ValidarTarea(entidad);
            if (error != null)
                return error;

            entidad.Urgencia = ObtenerUrgenciaTarea(entidad.FechaTarea);
            entidad.Estado = false;
            entidad.EstadoChar = "0";

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


        //FUNCIONALIDADES EXTRA
        public bool CompletarTarea(int idTarea)
        {
            var tarea = tareasRepository.ObtenerPorId(idTarea).Entidad;
            if (tarea == null)
            {
                return false;
            }

            tarea.EstadoChar = "1";
            tarea.Urgencia = 0;
            tarea.Estado = true;

            var response = tareasRepository.Actualizar(tarea);
            return response.Estado;
        }
        public bool ReabrirTarea(int idTarea)
        {
            var tarea = tareasRepository.ObtenerPorId(idTarea).Entidad;
            if (tarea == null)
            {
                return false;
            }
            tarea.EstadoChar = "0";
            tarea.Urgencia = ObtenerUrgenciaTarea(tarea.FechaTarea);
            tarea.Estado = false;

            var response = tareasRepository.Actualizar(tarea);
            return response.Estado;
        }
        private int ObtenerUrgenciaTarea(DateTime? fechaTarea)
        {
            if (!fechaTarea.HasValue)
                return 1;

            DateTime fechaActual = DateTime.Now;
            int diferencia = (int)(fechaTarea.Value - fechaActual).TotalDays;
            if (diferencia <= 3 && diferencia > 0)
            {
                return 3;
            }
            else if (diferencia <= 7 && diferencia > 0)
            {
                return 2;
            }
            else if (diferencia < 0)
            {
                return 4;
            }
            else
            {
                return 1;
            }
        }
        private string ValidarTarea(Tareas entidad)
        {
            // Validar parcela
            if (!validarIdParcela(entidad.IdParcela))
                return "La parcela asignada no existe.";

            // Fecha mínima
            if (!ValidarFechaEstipuladaTarea(entidad.FechaTarea))
                return "La fecha de la tarea no puede ser menor que la fecha actual.";

            // Fecha máxima
            if (!ValidarMaximaFechaTarea(entidad.FechaTarea))
                return "La fecha de la tarea no puede superar los dos años permitidos.";

            // Tipo de tarea
            if (!ValidarTipoTarea(entidad.Tipo))
                return "El tipo de tarea es inválido. Solo puede contener letras, espacios y máximo 20 caracteres.";

            // Costo

            return null;
        }


        //VALIDACIONES
        private bool validarIdParcela(int idParcela)
        {
            var parcela = parcelaRepository.ObtenerPorId(idParcela).Entidad;

            if (parcela != null)
            {
                return true;
            }
            return false;
        }
        private bool ValidarFechaEstipuladaTarea(DateTime? fechaTarea)
        {
            if (!fechaTarea.HasValue || fechaTarea.Value < DateTime.Now)
            {
                return false;
            }
            return true;
        }
        private bool ValidarTipoTarea(string tipo)
        {
            if (string.IsNullOrWhiteSpace(tipo))
                return false;

            if (tipo.Length > 20)
                return false;

            if (!tipo.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                return false;

            return true;
        }
        private bool ValidarMaximaFechaTarea(DateTime? fecha)
        {
            if (!fecha.HasValue || fecha.Value < DateTime.Today)
                return false;

            if (fecha.Value > DateTime.Today.AddYears(2))
                return false;

            return true;
        }
    }
}
