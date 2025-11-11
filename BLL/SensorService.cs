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
    public class SensorService : ICrudEscritura<Sensor>, ICrudLectura<Sensor>
    {
        private readonly SensorRepository sensorRepository;

        public SensorService()
        {
            sensorRepository = new SensorRepository();
        }

        public string Guardar(Sensor entidad)
        {
            var response = sensorRepository.Insertar(entidad);
            return response.Mensaje;
        }

        public bool Actualizar(Sensor entidad)
        {
            var response = sensorRepository.Actualizar(entidad);
            return response.Estado;
        }

        public bool Eliminar(Sensor entidad)
        {
            var response = sensorRepository.Eliminar(entidad.Id);
            return response.Estado;
        }

        public ReadOnlyCollection<Sensor> Consultar()
        {
            var response = sensorRepository.ObtenerTodos();
            var lista = response.Lista ?? new List<Sensor>();
            return new ReadOnlyCollection<Sensor>(lista);
        }

        public Sensor ObtenerPorId(string id)
        {
            var response = sensorRepository.ObtenerPorId(Convert.ToInt32(id));
            return response.Entidad;
        }

        //public void SimularDisminucion(Sensor sensor, decimal decremento)
        //{
        //    // Baja el nivel de humedad
        //    sensor.HumedadActual = Math.Max(0, (sensor.HumedadActual ?? 100) - decremento);
        //    sensorRepository.Actualizar(sensor);
        //}

        //public void RecargarHumedad(Sensor sensor)
        //{
        //    // Riego completo: recargar humedad a 100%
        //    sensor.HumedadActual = 100;
        //    sensorRepository.Actualizar(sensor);
        //}



    }
}
