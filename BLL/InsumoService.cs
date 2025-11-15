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
    public class InsumoService : ICrudEscritura<Insumo>, ICrudLectura<Insumo>
    {
        private readonly InsumoRepository insumoRepository;
        private readonly ParcelaRepository parcelaRepository;

        public InsumoService()
        {
            insumoRepository = new InsumoRepository();
            parcelaRepository = new ParcelaRepository();
        }

        private string Validar(Insumo insumo)
        {
            if (insumo.IdParcela <= 0)
                return "Debe seleccionar una parcela válida.";

            var parcela = parcelaRepository.ObtenerPorId(insumo.IdParcela);
            if (!parcela.Estado)
                return "La parcela indicada no existe.";

            if (insumo.Unidad <= 0)
                return "La cantidad (unidad) debe ser mayor que 0.";

            if (string.IsNullOrWhiteSpace(insumo.Tipo))
                return "El tipo de insumo es obligatorio.";

            if (insumo.Tipo.Length > 20)
                return "El tipo no debe superar los 20 caracteres.";

            foreach (char c in insumo.Tipo)
            {
                if (!char.IsLetter(c) && !char.IsWhiteSpace(c))
                    return "El tipo solo puede contener letras.";
            }

            if (insumo.CostoUnitario <= 0)
                return "El costo unitario debe ser mayor que 0.";

            return null; // Todo OK
        }

        public string Guardar(Insumo entidad)
        {
            string validacion = Validar(entidad);

            if (validacion != null)
                return validacion;

            var response = insumoRepository.Insertar(entidad);
            return response.Mensaje;
        }

        public bool Actualizar(Insumo entidad)
        {
            string validacion = Validar(entidad);

            if (validacion != null)
                return false;

            var response = insumoRepository.Actualizar(entidad);
            return response.Estado;
        }

        public bool Eliminar(Insumo entidad)
        {
            var response = insumoRepository.Eliminar(entidad.Id);
            return response.Estado;
        }

        public ReadOnlyCollection<Insumo> Consultar()
        {
            var response = insumoRepository.ObtenerTodos();
            var lista = response.Lista ?? new List<Insumo>();
            return new ReadOnlyCollection<Insumo>(lista);
        }

        public Insumo ObtenerPorId(string id)
        {
            var response = insumoRepository.ObtenerPorId(Convert.ToInt32(id));
            return response.Entidad;
        }
    }
}
