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
        GastosRepository insumoRepository = new GastosRepository();
        TareasRepository tareasRepository = new TareasRepository();

        private readonly RentabilidadRepository rentabilidadRepository;

        public RentabilidadService()
        {
            rentabilidadRepository = new RentabilidadRepository();
        }

        public string Guardar(Rentabilidad entidad)
        {
            var response = rentabilidadRepository.Insertar(entidad);
            return response.Mensaje;
        }

        public bool Actualizar(Rentabilidad entidad)
        {
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















    }
}
