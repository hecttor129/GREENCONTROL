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
        private readonly GastosRepository insumoRepository;
        private readonly ParcelaRepository parcelaRepository;

        public GastosService()
        {
            insumoRepository = new GastosRepository();
            parcelaRepository = new ParcelaRepository();
        }

        private string Validar(Gastos gasto)
        {
            if (gasto.IdParcela <= 0)
                return "Debe seleccionar una parcela válida.";

            var parcela = parcelaRepository.ObtenerPorId(gasto.IdParcela);
            if (!parcela.Estado)
                return "La parcela indicada no existe.";

            if (string.IsNullOrWhiteSpace(gasto.Tipo))
                return "El tipo es obligatorio.";

            if (gasto.Tipo.Length > 20)
                return "El tipo no debe superar 20 caracteres.";

            if (gasto.Monto <= 0)
                return "El monto debe ser mayor que 0.";

            return null;
        }

        public string Guardar(Gastos entidad)
        {
            string validacion = Validar(entidad);

            if (validacion != null)
                return validacion;

            var response = insumoRepository.Insertar(entidad);
            return response.Mensaje;
        }

        public bool Actualizar(Gastos entidad)
        {
            string validacion = Validar(entidad);

            if (validacion != null)
                return false;

            var response = insumoRepository.Actualizar(entidad);
            return response.Estado;
        }

        public bool Eliminar(Gastos entidad)
        {
            var response = insumoRepository.Eliminar(entidad.Id);
            return response.Estado;
        }

        public ReadOnlyCollection<Gastos> Consultar()
        {
            var response = insumoRepository.ObtenerTodos();
            var lista = response.Lista ?? new List<Gastos>();
            return new ReadOnlyCollection<Gastos>(lista);
        }

        public Gastos ObtenerPorId(string id)
        {
            var response = insumoRepository.ObtenerPorId(Convert.ToInt32(id));
            return response.Entidad;
        }
    }
}
