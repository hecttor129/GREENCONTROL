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
    public class HistorialGastosService : ICrudEscritura<HistorialGastos>, ICrudLectura<HistorialGastos>
    {
        private readonly HistorialGastosRepository historialGastosRepository;

        public HistorialGastosService()
        {
            historialGastosRepository = new HistorialGastosRepository();
        }

        public string Guardar(HistorialGastos entidad)
        {
            var response = historialGastosRepository.Insertar(entidad);
            return response.Mensaje;
        }

        public bool Actualizar(HistorialGastos entidad)
        {
            var response = historialGastosRepository.Actualizar(entidad);
            return response.Estado;
        }

        public bool Eliminar(HistorialGastos entidad)
        {
            var response = historialGastosRepository.Eliminar(entidad.IdHistorialGasto);
            return response.Estado;
        }

        public ReadOnlyCollection<HistorialGastos> Consultar()
        {
            var response = historialGastosRepository.ObtenerTodos();
            var lista = response.Lista ?? new List<HistorialGastos>();
            return new ReadOnlyCollection<HistorialGastos>(lista);
        }

        public HistorialGastos ObtenerPorId(string id)
        {
            var response = historialGastosRepository.ObtenerPorId(Convert.ToInt32(id));
            return response.Entidad;
        }

        public HistorialGastos CrearHistorialGastoDesdeRegistro(Gastos gasto)
        {
            return new HistorialGastos
            {
                IdHistorialGasto = gasto.Id,
                IdParcela = gasto.IdParcela,

                Tipo = gasto.Tipo,
                Monto = gasto.Monto,
                Descripcion = gasto.Descripcion,
                FechaGasto = gasto.Fecha,

                // Momento del snapshot
                //FechaSnapshot = DateTime.Now
            };
        }

    }
}
