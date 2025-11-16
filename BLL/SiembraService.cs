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

        //CRUD BASICO

        public string Guardar(Siembra entidad)
        {

            if (ValidarFechaEstipuladaSiembra(entidad.FechaSiembra))
            {
                var response = siembraRepository.Insertar(entidad);
                return response.Mensaje;
            }
            else
            {
                return "La fecha de siembra no puede ser anterior a la fecha actual.";
            }

        }
        public bool Actualizar(Siembra entidad)
        {
            var response = siembraRepository.Actualizar(entidad);
            return response.Estado;
        }
        public bool Eliminar(Siembra entidad)
        {
            var response = siembraRepository.Eliminar(entidad.IdSiembra);
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

        //VALIDACIONES

        private bool ValidarFechaEstipuladaSiembra(DateTime fechaRegistradaPorUsuario)
        {
            if (fechaRegistradaPorUsuario < DateTime.Now)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool ActivarSiembra(int id)
        {
            var siembra = siembraRepository.ObtenerPorId(id).Entidad;
            if (siembra == null)
                return false;
            siembra.EstadoChar = "A";
            var response = siembraRepository.Actualizar(siembra);
            return response.Estado;
        }
        public bool InactivarSiembra(int id)
        {
            var siembra = siembraRepository.ObtenerPorId(id).Entidad;
            if (siembra == null)
                return false;
            siembra.EstadoChar = "I";
            var response = siembraRepository.Actualizar(siembra);
            return response.Estado;
        }


        //FUNCIONALIDADES Y CALCULOS EXTRAS

        //public DateTime CalcularFechaGerminacionAutomatica(int id)
        //{
        //    var siembra = siembraRepository.ObtenerPorId(id).Entidad;
        //    if (siembra == null)
        //        throw new Exception("No se encontró la siembra.");

        //    var cultivo = cultivoRepository.ObtenerPorId(siembra.IdCultivo).Entidad;
        //    if (cultivo == null)
        //        throw new Exception("No se encontró el cultivo asociado.");

        //    DateTime fechaBase = siembra.FechaSiembra;

        //    double promedioDias = (double)((cultivo.DiasGerminacion_Fecha1 + cultivo.DiasGerminacion_Fecha2) / 2.0);

        //    return fechaBase.AddDays(promedioDias);
        //}
        //public DateTime CalcularFechaFloracionAutomatica(int id)
        //{
        //    var siembra = siembraRepository.ObtenerPorId(id).Entidad;
        //    if (siembra == null)
        //        throw new Exception("No se encontró la siembra.");

        //    var cultivo = cultivoRepository.ObtenerPorId(siembra.IdCultivo).Entidad;
        //    if (cultivo == null)
        //        throw new Exception("No se encontró el cultivo asociado.");

        //    DateTime fechaBase;

        //    if (siembra.FechaGerminacion.HasValue)
        //    {
        //        fechaBase = siembra.FechaGerminacion.Value;
        //    }
        //    else
        //    {
        //        fechaBase = CalcularFechaGerminacionAutomatica(id);
        //    }

        //    double promedioDias = (double)((cultivo.DiasFloracion_Fecha1 + cultivo.DiasFloracion_Fecha2) / 2.0);

        //    DateTime fechaFloracion = fechaBase.AddDays(promedioDias);

        //    return fechaFloracion;
        //}    
        //public DateTime CalcularFechaCosechaAutomatica(int id)
        //{
        //    var siembra = siembraRepository.ObtenerPorId(id).Entidad;
        //    if (siembra == null)
        //        throw new Exception("No se encontró la siembra.");

        //    var cultivo = cultivoRepository.ObtenerPorId(siembra.IdCultivo).Entidad;
        //    if (cultivo == null)
        //        throw new Exception("No se encontró el cultivo asociado.");

        //    DateTime fechaBase;

        //    if (siembra.FechaFloracion.HasValue)
        //    {
        //        fechaBase = siembra.FechaFloracion.Value;
        //    }
        //    else
        //    {
        //        fechaBase = CalcularFechaFloracionAutomatica(id);
        //    }

        //    double promedioDias = (double)((cultivo.DiasCosecha_Fecha1 + cultivo.DiasCosecha_Fecha2) / 2.0);

        //    DateTime fechaCosecha = fechaBase.AddDays(promedioDias);

        //    return fechaCosecha;
        //}
        //public double CalcularPorcentajeDesarrollo(int id)
        //{
        //    DateTime fechaActual = DateTime.Now;

        //    var siembra = siembraRepository.ObtenerPorId(id).Entidad;
        //    if (siembra == null)
        //        throw new Exception("No se encontró la siembra.");

        //    var cultivo = cultivoRepository.ObtenerPorId(siembra.IdCultivo).Entidad;
        //    if (cultivo == null)
        //        throw new Exception("No se encontró el cultivo asociado.");


        //    double promedioCiclo = (double)((cultivo.DuracionCiclo_Fecha1 + cultivo.DuracionCiclo_Fecha2) / 2.0);
        //    double diasPasados = (fechaActual - siembra.FechaSiembra).TotalDays;
        //    double porcentaje = (diasPasados / promedioCiclo) * 100;

        //    if (porcentaje < 0)
        //    {
        //        porcentaje = 0;
        //    }
        //    else if (porcentaje > 100)
        //    {
        //        porcentaje = 100;
        //    }

        //    return porcentaje;
        //}
        //public bool ConfirmarFechaCosecha(int id)
        //{
        //    var siembra = siembraRepository.ObtenerPorId(id).Entidad;
        //    if (siembra == null)
        //        return false;

        //    if (siembra.FechaCosecha != null)
        //        return true;

        //    siembra.FechaCosecha = DateTime.Now;

        //    var response = siembraRepository.Actualizar(siembra);
        //    return response.Estado;
        //}
        //public bool ConfirmarFechaFloracion(int id)
        //{
        //    var siembra = siembraRepository.ObtenerPorId(id).Entidad;
        //    if (siembra == null)
        //        return false;

        //    if (siembra.FechaFloracion != null)
        //        return true;

        //    siembra.FechaFloracion = DateTime.Now;

        //    siembra.FechaCosecha = CalcularFechaCosechaAutomatica(id);

        //    var response = siembraRepository.Actualizar(siembra);
        //    return response.Estado;
        //}
        //public bool ConfirmarFechaGerminacion(int id)
        //{
        //    var siembra = siembraRepository.ObtenerPorId(id).Entidad;
        //    if (siembra == null)
        //        return false;

        //    if (siembra.FechaGerminacion != null)
        //        return true;

        //    siembra.FechaGerminacion = DateTime.Now;

        //    siembra.FechaFloracion = CalcularFechaFloracionAutomatica(id);
        //    siembra.FechaCosecha = CalcularFechaCosechaAutomatica(id);

        //    var response = siembraRepository.Actualizar(siembra);
        //    return response.Estado;
        //}
    }
}
