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
    public class InsumoService : ICrudEscritura<Insumo>, ICrudLectura<Insumo>
    {
        private readonly InsumoRepository insumoRepository;

        public InsumoService()
        {
            insumoRepository = new InsumoRepository();
        }

        public string Guardar(Insumo entidad)
        {
            var response = insumoRepository.Insertar(entidad);
            return response.Mensaje;
        }

        public bool Actualizar(Insumo entidad)
        {
            var response = insumoRepository.Actualizar(entidad);
            return response.Estado;
        }

        public bool Eliminar(Insumo entidad)
        {
            var response = insumoRepository.Eliminar(entidad.Id);
            return response.Estado;
        }

        public ReadOnlyCollection<Insumo> Consultar()
        {
            var response = insumoRepository.ObtenerTodos();
            var lista = response.Lista ?? new List<Insumo>();
            return new ReadOnlyCollection<Insumo>(lista);
        }

        public Insumo ObtenerPorId(string id)
        {
            var response = insumoRepository.ObtenerPorId(Convert.ToInt32(id));
            return response.Entidad;
        }
    }
}
