using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using ENTITY;
using System.Collections.ObjectModel;

namespace BLL
{
    public class CultivoService : ICrudEscritura<Cultivo>, ICrudLectura<Cultivo>
    {
        private readonly CultivoRepository cultivoRepository;

        public CultivoService()
        {
            cultivoRepository = new CultivoRepository();
        }

        public string Guardar(Cultivo entidad)
        {
            var response = cultivoRepository.Insertar(entidad);
            return response.Mensaje;
        }

        public bool Actualizar(Cultivo entidad)
        {
            var response = cultivoRepository.Actualizar(entidad);
            return response.Estado;
        }

        public bool Eliminar(Cultivo entidad)
        {
            var response = cultivoRepository.Eliminar(entidad.IdCultivo);
            return response.Estado;
        }

        public ReadOnlyCollection<Cultivo> Consultar()
        {
            var response = cultivoRepository.ObtenerTodos();
            var lista = response.Lista ?? new List<Cultivo>();
            return new ReadOnlyCollection<Cultivo>(lista);
        }

        public Cultivo ObtenerPorId(string id)
        {
            var response = cultivoRepository.ObtenerPorId(Convert.ToInt32(id));
            return response.Entidad;
        }

    }
}
