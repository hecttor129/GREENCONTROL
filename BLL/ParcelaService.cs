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
    public class ParcelaService : ICrudEscritura<Parcela>, ICrudLectura<Parcela>
    {
        private readonly ParcelaRepository parcelaRepository;

        public ParcelaService()
        {
            parcelaRepository = new ParcelaRepository();
        }

        public string Guardar(Parcela entidad)
        {
            var response = parcelaRepository.Insertar(entidad);
            return response.Mensaje;
        }

        public bool Actualizar(Parcela entidad)
        {
            var response = parcelaRepository.Actualizar(entidad);
            return response.Estado;
        }

        public bool Eliminar(Parcela entidad)
        {
            var response = parcelaRepository.Eliminar(entidad.IdParcela);
            return response.Estado;
        }

        public ReadOnlyCollection<Parcela> Consultar()
        {
            var response = parcelaRepository.ObtenerTodos();
            var lista = response.Lista ?? new List<Parcela>();
            return new ReadOnlyCollection<Parcela>(lista);
        }

        public Parcela ObtenerPorId(string id)
        {
            var response = parcelaRepository.ObtenerPorId(Convert.ToInt32(id));
            return response.Entidad;
        }


        public Historial ObtenerDatos(int idParcela)
        {



            throw new NotImplementedException();

        }





    }
}
