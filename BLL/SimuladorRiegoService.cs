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
    public class SimuladorRiegoService : ICrudEscritura<SimuladorRiego>, ICrudLectura<SimuladorRiego>
    {
        private readonly SimuladorRiegoRepository simuladorRiegoRepository;
        private readonly SensorService sensorService;

        public SimuladorRiegoService()
        {
            simuladorRiegoRepository = new SimuladorRiegoRepository();
            sensorService = new SensorService();
        }

        public string Guardar(SimuladorRiego entidad)
        {
            var response = simuladorRiegoRepository.Insertar(entidad);
            return response.Mensaje;
        }

        public bool Actualizar(SimuladorRiego entidad)
        {
            var response = simuladorRiegoRepository.Actualizar(entidad);
            return response.Estado;
        }

        public bool Eliminar(SimuladorRiego entidad)
        {
            var response = simuladorRiegoRepository.Eliminar(entidad.Id);
            return response.Estado;
        }

        public ReadOnlyCollection<SimuladorRiego> Consultar()
        {
            var response = simuladorRiegoRepository.ObtenerTodos();
            var lista = response.Lista ?? new List<SimuladorRiego>();
            return new ReadOnlyCollection<SimuladorRiego>(lista);
        }

        public SimuladorRiego ObtenerPorId(string id)
        {
            var response = simuladorRiegoRepository.ObtenerPorId(Convert.ToInt32(id));
            return response.Entidad;
        }

        //public void SupervisarSensor(Sensor sensor, SimuladorRiego simulador)
        //{
        //    // Si el riego está activo, aumentar humedad gradualmente
        //    if (simulador.EstadoRiego == "ACTIVO")
        //    {
        //        if (sensor.HumedadActual >= 100)
        //        {
        //            sensor.HumedadActual = 100;
        //            simulador.EstadoRiego = "INACTIVO"; // Deja de regar cuando llega a 100
        //        }
        //    }
        //    else
        //    {
        //        // Si el riego está inactivo y la humedad baja del umbral, activar riego
        //        if (sensor.HumedadActual <= simulador.UmbralActivacion)
        //        {
        //            simulador.EstadoRiego = "ACTIVO";
        //        }
        //    }
        //}







    }
}
