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
    public class RentabilidadService : ICrudEscritura<Rentabilidad>, ICrudLectura<Rentabilidad>
    {
        private readonly GastosRepository gastosRepository = new GastosRepository();
        private readonly CosechaRepository cosechaRepository = new CosechaRepository();
        private readonly RentabilidadRepository rentabilidadRepository;

        public RentabilidadService()
        {
            rentabilidadRepository = new RentabilidadRepository();
            cosechaRepository = new CosechaRepository();
            gastosRepository = new GastosRepository();
        }

        public string Guardar(Rentabilidad entidad)
        {
            RecalcularRentabilidad(entidad.IdParcela);
            var response = rentabilidadRepository.Insertar(entidad);
            return response.Mensaje;
        }
        public bool Actualizar(Rentabilidad entidad)
        {
            RecalcularRentabilidad(entidad.IdParcela);
            var response = rentabilidadRepository.Actualizar(entidad);
            return response.Estado;
        }
        public bool Eliminar(Rentabilidad entidad)
        {
            var response = rentabilidadRepository.Eliminar(entidad.IdRentabilidad);
            return response.Estado;
        }
        public ReadOnlyCollection<Rentabilidad> Consultar()
        {
            var response = rentabilidadRepository.ObtenerTodos();
            var lista = response.Lista ?? new List<Rentabilidad>();
            return new ReadOnlyCollection<Rentabilidad>(lista);
        }
        public Rentabilidad ObtenerPorId(string id)
        {
            var response = rentabilidadRepository.ObtenerPorId(Convert.ToInt32(id));
            return response.Entidad;
        }

        private decimal CalcularIngresoTotalCosecha(int idParcela)
        {
            var cosechas = cosechaRepository.ObtenerPorId(idParcela);
            decimal total = 0;
            if (cosechas.Estado && cosechas.Lista != null)
            {
                foreach (var cosecha in cosechas.Lista)
                {
                    if (cosecha.CantidadCosechada.HasValue && cosecha.PrecioVentaUnitario.HasValue)
                    {
                        total += cosecha.CantidadCosechada.Value * cosecha.PrecioVentaUnitario.Value;
                    }
                }
            }
            return total;
        }
        private decimal CalcularIngresoTotalGastos(int idParcela)
        {
            var gastos = gastosRepository.ObtenerPorId(idParcela);
            decimal total = 0;
            if (gastos.Entidad.Tipo == "ingreso" && gastos.Lista != null)
            {
                foreach (var gasto in gastos.Lista)
                {
                    
                        total += gasto.Monto;
                    
                }
            }
            return total;

        }

        private decimal CalcularIngresoTotal(int idParcela)
        {
            decimal ingresoCosecha = CalcularIngresoTotalCosecha(idParcela);
            decimal ingresoGastos = CalcularIngresoTotalGastos(idParcela);
            return ingresoCosecha + ingresoGastos;
        }
        private decimal CalcularCostoTotalProduccion(int idParcela)
        {
            var gastos = gastosRepository.ObtenerPorId(idParcela);
            decimal total = 0;
            if (gastos.Estado && gastos.Lista != null)
            {
                foreach (var gasto in gastos.Lista)
                {
                    if (gasto.Tipo != "ingreso")
                    {
                        total += gasto.Monto;
                    }
                }
            }
            return total;
        }

        private decimal CalcularBalance(int idParcela)
        {
            decimal ingresoTotal = CalcularIngresoTotal(idParcela);
            decimal costoTotal = CalcularCostoTotalProduccion(idParcela);
            return ingresoTotal - costoTotal;
        }
        private decimal CalcularRentabilidadPorcentual(int idParcela)
        {
            decimal ingresoTotal = CalcularIngresoTotal(idParcela);
            decimal costoTotal = CalcularCostoTotalProduccion(idParcela);
            if (costoTotal == 0) return 0;
            return (ingresoTotal - costoTotal) / costoTotal * 100;
        }

        public Rentabilidad RecalcularRentabilidad(int idParcela)
        {
            return new Rentabilidad
            {
                IdParcela = idParcela,
                IngresoTotal = CalcularIngresoTotal(idParcela),
                CostoTotalProduccion = CalcularCostoTotalProduccion(idParcela),
                RentabilidadPorcentual = CalcularRentabilidadPorcentual(idParcela)
            };
        }


    }
}
