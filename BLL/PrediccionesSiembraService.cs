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
    public class PrediccionesSiembraService
    {
        private readonly PrediccionesSiembraRepository repository;
        private readonly CultivoRepository cultivoRepository;
        private readonly SiembraRepository siembraRepository;
        private readonly ParcelaRepository parcelaRepository;
        private readonly HistorialRepository historialRepository;

        public PrediccionesSiembraService()
        {
            repository = new PrediccionesSiembraRepository();
            cultivoRepository = new CultivoRepository();
            siembraRepository = new SiembraRepository();
            parcelaRepository = new ParcelaRepository();
            historialRepository = new HistorialRepository();
        }

        public string Guardar(PrediccionesSiembra entidad)
        {
            var response = repository.Insertar(entidad);
            return response.Mensaje;
        }

        public bool Actualizar(PrediccionesSiembra entidad)
        {
            var response = repository.Actualizar(entidad);
            return response.Estado;
        }

        public bool Eliminar(PrediccionesSiembra entidad)
        {
            var response = repository.Eliminar(entidad.IdPrediccion);
            return response.Estado;
        }

        public ReadOnlyCollection<PrediccionesSiembra> Consultar()
        {
            var response = repository.ObtenerTodos();
            var lista = response.Lista ?? new List<PrediccionesSiembra>();
            return new ReadOnlyCollection<PrediccionesSiembra>(lista);
        }

        public PrediccionesSiembra ObtenerPorId(string id)
        {
            var response = repository.ObtenerPorId(Convert.ToInt32(id));
            return response.Entidad;
        }

        //crear el json con los datos necesarios para la prediccion
        public string CrearJsonPrediccion()
        {

            return "";
        }






    }
}
