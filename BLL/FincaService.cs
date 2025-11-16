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
    public class FincaService : ICrudEscritura<Finca>, ICrudLectura<Finca>
    {
        private readonly FincaRepository fincaRepository;

        public FincaService()
        {
            fincaRepository = new FincaRepository();
        }

        public string Guardar(Finca entidad)
        {
            var response = fincaRepository.Insertar(entidad);
            return response.Mensaje;
        }

        public bool Actualizar(Finca entidad)
        {
            var response = fincaRepository.Actualizar(entidad);
            return response.Estado;
        }

        public bool Eliminar(Finca entidad)
        {
            var response = fincaRepository.Eliminar(entidad.Id);
            return response.Estado;
        }

        public ReadOnlyCollection<Finca> Consultar()
        {
            var response = fincaRepository.ObtenerTodos();
            var lista = response.Lista ?? new List<Finca>();
            return new ReadOnlyCollection<Finca>(lista);
        }

        public Finca ObtenerPorId(string id)
        {
            var response = fincaRepository.ObtenerPorId(Convert.ToInt32(id));
            return response.Entidad;
        }
    }
}
