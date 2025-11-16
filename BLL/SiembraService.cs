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
        public string ObtenerNombreCultivo(int idSiembra)
        {
            var siembra = ObtenerPorId(idSiembra.ToString());
            if (siembra.IdCultivo != null)
            {
                var cultivo = cultivoRepository.ObtenerPorId(siembra.IdCultivo);
                return cultivo.Entidad.Nombre;
            }
            return string.Empty;
        }

    }
}
