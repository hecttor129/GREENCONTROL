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
    public class HistorialService : ICrudEscritura<Historial>, ICrudLectura<Historial>
    {
        private readonly HistorialRepository historialRepository;
        private readonly ParcelaRepository parcelaRepository;
        private readonly CultivoRepository cultivoRepository;
        private readonly CosechaRepository cosechaRepository;
        private readonly GastosRepository gastosRepository;
        private readonly SiembraRepository siembraRepository;
        private readonly RentabilidadRepository rentabilidadRepository;



        public HistorialService()
        {
            historialRepository = new HistorialRepository();
            parcelaRepository = new ParcelaRepository();
            cultivoRepository = new CultivoRepository();
            cosechaRepository = new CosechaRepository();
            gastosRepository = new GastosRepository();
            siembraRepository = new SiembraRepository();
            rentabilidadRepository = new RentabilidadRepository();
        }

        public string Guardar(Historial entidad)
        {
            entidad.IdParcelaCopy = entidad.IdParcela;

            var response = historialRepository.Insertar(entidad);
            return response.Mensaje;
        }

        public bool Actualizar(Historial entidad)
        {
            entidad.IdParcelaCopy = entidad.IdParcela;

            var response = historialRepository.Actualizar(entidad);
            return response.Estado;
        }

        public bool Eliminar(Historial entidad)
        {
            var response = historialRepository.Eliminar(entidad.IdHistorial);
            return response.Estado;
        }

        public ReadOnlyCollection<Historial> Consultar()
        {
            var response = historialRepository.ObtenerTodos();
            var lista = response.Lista ?? new List<Historial>();
            return new ReadOnlyCollection<Historial>(lista);
        }

        public Historial ObtenerPorId(string id)
        {
            var response = historialRepository.ObtenerPorId(Convert.ToInt32(id));
            return response.Entidad;
        }


        public int CalcularDuracionCiclo(int idSiembra)
        {
            var siembra = siembraRepository.ObtenerPorId(idSiembra);
            int diasCiclo = siembra.Entidad.FechaCosecha.HasValue && siembra.Entidad.FechaSiembra != null
                ? (siembra.Entidad.FechaCosecha.Value - siembra.Entidad.FechaSiembra).Days
                : 0;

            return diasCiclo;
        }


        public Historial CrearHistorialDesdeRegistros(int idParcela)
        {
            var parcela = parcelaRepository.ObtenerPorId(idParcela).Entidad;
            var siembra = siembraRepository.ObtenerPorId(parcela.IdCultivo).Entidad;
            var cultivo = cultivoRepository.ObtenerPorId(parcela.IdCultivo).Entidad;
            var cosecha = cosechaRepository.ObtenerPorId(idParcela).Entidad;
            var rentabilidad = rentabilidadRepository.ObtenerPorId(idParcela).Entidad;

            return new Historial
            {
                IdParcela = parcela.IdParcela,
                IdParcelaCopy = parcela.IdParcela,

                // Fechas principales
                FechaSiembra = siembra.FechaSiembra,
                FechaGerminacion = siembra.FechaGerminacion,
                FechaFloracion = siembra.FechaFloracion,
                FechaCosecha = siembra?.FechaCosecha,


                // Producción
                CalidadCosechada = cosecha?.CalidadCosechada,
                CantidadCosechada = cosecha?.CantidadCosechada,
                DuracionCiclo = CalcularDuracionCiclo(siembra.IdSiembra),

                // Datos de la parcela
                NombreParcela = parcela.Nombre,
                TipoSuelo = parcela.TipoSuelo,
                PhSuelo = parcela.PhSuelo,

                // Datos del cultivo
                NombreCultivo = cultivo.Nombre,


                // Datos económicos (YA RECIBIDOS, SIN CALCULAR)
                CostoTotalProduccion = rentabilidad.CostoTotalProduccion,
                IngresoTotal = rentabilidad.IngresoTotal,
                RentabilidadFinal = rentabilidad.RentabilidadPorcentual,

                // Momento en que se toma el snapshot
                FechaSnapshot = DateTime.Now
            };
        }


    }
}