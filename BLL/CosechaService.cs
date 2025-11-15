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
    public class CosechaService : ICrudEscritura<Cosecha>, ICrudLectura<Cosecha>
    {

        private readonly CosechaRepository cosechaRepository;
        private readonly SiembraRepository siembraRepository;

        public CosechaService()
        {
            cosechaRepository = new CosechaRepository();
            siembraRepository = new SiembraRepository();
        }

        public string Guardar(Cosecha entidad)
        {
            string validacion = Validar(entidad);

            if (validacion != null)
                return validacion;

            var response = cosechaRepository.Insertar(entidad);
            return response.Mensaje;
        }

        public bool Actualizar(Cosecha entidad)
        {
            string validacion = Validar(entidad);

            if (validacion != null)
                return false;

            var response = cosechaRepository.Actualizar(entidad);
            return response.Estado;
        }

        public bool Eliminar(Cosecha entidad)
        {
            var response = cosechaRepository.Eliminar(entidad.Id);
            return response.Estado;
        }

        public ReadOnlyCollection<Cosecha> Consultar()
        {
            var response = cosechaRepository.ObtenerTodos();
            var lista = response.Lista ?? new List<Cosecha>();
            return new ReadOnlyCollection<Cosecha>(lista);
        }

        public Cosecha ObtenerPorId(string id)
        {
            var response = cosechaRepository.ObtenerPorId(Convert.ToInt32(id));
            return response.Entidad;
        }

        private string Validar(Cosecha entidad)
        {
            if (entidad.IdSiembra <= 0)
                return "Debe seleccionar una siembra valida.";

            var siembra = siembraRepository.ObtenerPorId(entidad.IdSiembra);
            if (siembra.Entidad == null)
                return "La siembra seleccionada no existe.";
         
            if (entidad.Id > 0)
            {
                var cosechaExistente = cosechaRepository.ObtenerPorId(entidad.Id).Entidad;

                if (cosechaExistente != null)
                {
                    if (cosechaExistente.Estado == "1")
                    {
                        return "La cosecha ya esta completada/inactiva y no puede modificarse.";
                    }

                    entidad.Estado = "1";
                
                }

                if (cosechaExistente == null)
                    return "La cosecha que intenta actualizar no existe.";
            }

            if (entidad.Calidad.HasValue)
            {
                if (entidad.Calidad < 1 || entidad.Calidad > 5)
                    return "La calidad debe estar entre 1 y 5 estrellas.";
            }

            if (entidad.Cantidad.HasValue && entidad.Cantidad <= 0)
                return "La cantidad debe ser un número mayor que cero.";

            return null;
        }





    }
}
