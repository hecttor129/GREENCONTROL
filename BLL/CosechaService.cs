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
    public class CosechaService : ICrudEscritura<Cosecha>, ICrudLectura<Cosecha>
    {
        private readonly CosechaRepository cosechaRepository;

        public CosechaService()
        {
            cosechaRepository = new CosechaRepository();
        }

        public string Guardar(Cosecha entidad)
        {
            var response = cosechaRepository.Insertar(entidad);
            return response.Mensaje;
        }

        public bool Actualizar(Cosecha entidad)
        {
            var response = cosechaRepository.Actualizar(entidad);
            return response.Estado;
        }

        public bool Eliminar(Cosecha entidad)
        {
            var response = cosechaRepository.Eliminar(entidad.Id);
            return response.Estado;
        }

        public ReadOnlyCollection<Cosecha> Consultar()
        {
            var response = cosechaRepository.ObtenerTodos();
            var lista = response.Lista ?? new List<Cosecha>();
            return new ReadOnlyCollection<Cosecha>(lista);
        }

        public Cosecha ObtenerPorId(string id)
        {
            var response = cosechaRepository.ObtenerPorId(Convert.ToInt32(id));
            return response.Entidad;
        }

    }
}
