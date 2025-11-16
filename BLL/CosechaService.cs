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
        private readonly ParcelaRepository parcelaRepository;

        public CosechaService()
        {
            cosechaRepository = new CosechaRepository();
            parcelaRepository = new ParcelaRepository();
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
            if (entidad.IdParcela <= 0)
                return "Debe seleccionar una parcela válida.";

            if (parcelaRepository.ObtenerPorId(entidad.IdParcela).Entidad == null)
                return "La parcela seleccionada no existe.";

            if (entidad.CalidadCosechada.HasValue &&
                (entidad.CalidadCosechada < 1 || entidad.CalidadCosechada > 5))
                return "La calidad debe estar entre 1 y 5.";

            if (entidad.CantidadCosechada.HasValue &&
                entidad.CantidadCosechada <= 0)
                return "La cantidad debe ser mayor que cero.";

            if (entidad.PrecioVentaUnitario.HasValue &&
                entidad.PrecioVentaUnitario < 0)
                return "El precio no puede ser negativo.";

            return null;
        }
    }
}
