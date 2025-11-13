using DAL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class SiembraService : ICrudEscritura<Siembra>, ICrudLectura<Siembra>
    {
        private readonly SiembraRepository siembraRepository;
        private readonly CultivoRepository cultivoRepository;

        public SiembraService()
        {
            siembraRepository = new SiembraRepository();
            cultivoRepository = new CultivoRepository();
        }

        public string Guardar(Siembra entidad)
        {
            var response = siembraRepository.Insertar(entidad);
            return response.Mensaje;
        }

        public bool Actualizar(Siembra entidad)
        {
            var response = siembraRepository.Actualizar(entidad);
            return response.Estado;
        }

        public bool Eliminar(Siembra entidad)
        {
            var response = siembraRepository.Eliminar(entidad.Id);
            return response.Estado;
        }

        public ReadOnlyCollection<Siembra> Consultar()
        {
            var response = siembraRepository.ObtenerTodos();
            var lista = response.Lista ?? new List<Siembra>();
            return new ReadOnlyCollection<Siembra>(lista);
        }

        public Siembra ObtenerPorId(string id)
        {
            var response = siembraRepository.ObtenerPorId(Convert.ToInt32(id));
            return response.Entidad;
        }

        public DateTime CalcularFechaGerminacionAutomatica(int id)
        {
            var siembra = siembraRepository.ObtenerPorId(id).Entidad;
            if (siembra == null)
                throw new Exception("No se encontró la siembra.");

            var cultivo = cultivoRepository.ObtenerPorId(siembra.IdCultivo).Entidad;
            if (cultivo == null)
                throw new Exception("No se encontró el cultivo asociado.");

            DateTime fechaBase = siembra.FechaSiembra;
            double promedioDias = (double)((cultivo.DiasGerminacion_Fecha1 + cultivo.DiasGerminacion_Fecha2) / 2.0);

            return fechaBase.AddDays(promedioDias);
        }

        public DateTime CalcularFechaFloracionAutomatica(int id)
        {
            var siembra = siembraRepository.ObtenerPorId(id).Entidad;
            if (siembra == null)
                throw new Exception("No se encontró la siembra.");

            var cultivo = cultivoRepository.ObtenerPorId(siembra.IdCultivo).Entidad;
            if (cultivo == null)
                throw new Exception("No se encontró el cultivo asociado.");

            DateTime fechaBase = siembra.FechaGerminacion ?? CalcularFechaGerminacionAutomatica(id);

            double promedioDias = (double)((cultivo.DiasFloracion_Fecha1 + cultivo.DiasFloracion_Fecha2) / 2.0);

            return fechaBase.AddDays(promedioDias);
        }

        public DateTime CalcularFechaCosechaAutomatica(int id)
        {
            var siembra = siembraRepository.ObtenerPorId(id).Entidad;
            if (siembra == null)
                throw new Exception("No se encontró la siembra.");

            var cultivo = cultivoRepository.ObtenerPorId(siembra.IdCultivo).Entidad;
            if (cultivo == null)
                throw new Exception("No se encontró el cultivo asociado.");

            DateTime fechaBase = siembra.FechaFloracion ?? CalcularFechaFloracionAutomatica(id);

            double promedioDias = (double)((cultivo.DiasCosecha_Fecha1 + cultivo.DiasCosecha_Fecha2) / 2.0);

            return fechaBase.AddDays(promedioDias);
        }

        public double CalcularPorcentajeDesarrollo(DateTime fechaSiembra, int duracionCiclo1, int duracionCiclo2, DateTime? fechaCosecha)
        {
            DateTime fechaActual = DateTime.Now;

            if (fechaCosecha.HasValue && fechaCosecha.Value <= fechaActual)
            {
                return 100;
            }

            double promedioCiclo = (duracionCiclo1 + duracionCiclo2) / 2.0;
            double diasPasados = (fechaActual - fechaSiembra).TotalDays;
            double porcentaje = (diasPasados / promedioCiclo) * 100;

            if (porcentaje < 0)
            {
                porcentaje = 0;
            }
            else if (porcentaje > 100)
            {
                porcentaje = 100;
            }

            return porcentaje;
        }

        public bool ConfirmarFechaCosecha(int id)
        {
            var siembra = siembraRepository.ObtenerPorId(id).Entidad;
            if (siembra == null)
                return false;

            if (siembra.FechaCosecha != null)
                return true;

            siembra.FechaCosecha = DateTime.Now;

            var response = siembraRepository.Actualizar(siembra);
            return response.Estado;
        }


        public bool ConfirmarFechaFloracion(int id)
        {
            var siembra = siembraRepository.ObtenerPorId(id).Entidad;
            if (siembra == null)
                return false;

            if (siembra.FechaFloracion != null)
                return true;

            siembra.FechaFloracion = DateTime.Now;

            siembra.FechaCosecha = CalcularFechaCosechaAutomatica(id);

            var response = siembraRepository.Actualizar(siembra);
            return response.Estado;
        }

        public bool ConfirmarFechaGerminacion(int id)
        {
            var siembra = siembraRepository.ObtenerPorId(id).Entidad;
            if (siembra == null)
                return false;

            if (siembra.FechaGerminacion != null)
                return true;

            siembra.FechaGerminacion = DateTime.Now;

            siembra.FechaFloracion = CalcularFechaFloracionAutomatica(id);
            siembra.FechaCosecha = CalcularFechaCosechaAutomatica(id);

            var response = siembraRepository.Actualizar(siembra);
            return response.Estado;
        }










    }
}
