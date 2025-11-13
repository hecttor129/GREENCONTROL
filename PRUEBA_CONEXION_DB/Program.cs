using BLL;
using DAL;
using ENTITY;
using System;
using System.Timers;

class Program
{
    static void Main(string[] args)
    {
        ProbarConexionProgram();
    }







//    void sistemariego()
//    {
//        Sensor sensor = new Sensor
//        {
//            Id = 1,
//            IdParcela = 1,
//            HumedadActual = 100,
//            FrecuenciaLectura = 1 // cada 1 segundo
//        };

//        SimuladorRiego simulador = new SimuladorRiego
//        {
//            Id = 1,
//            IdParcela = 1,
//            EstadoRiego = "INACTIVO",
//            UmbralActivacion = 90
//        };

//        SensorService sensorService = new SensorService();
//        SimuladorRiegoService riegoService = new SimuladorRiegoService();

//        System.Timers.Timer timer = new System.Timers.Timer(sensor.FrecuenciaLectura.Value * 1000);
//        timer.Elapsed += (sender, e) =>
//        {
//            // Si el riego está inactivo, la humedad baja
//            if (simulador.EstadoRiego == "INACTIVO")
//            {
//                sensorService.SimularDisminucion(sensor, 1);
//            }
//            else
//            {
//                // Si el riego está activo, sube gradualmente
//                sensor.HumedadActual += 1;

//                if (sensor.HumedadActual >= 100)
//                {
//                    sensor.HumedadActual = 100;
//                    simulador.EstadoRiego = "INACTIVO";
//                }
//            }

//            // Supervisar el estado del riego según la humedad
//            riegoService.SupervisarSensor(sensor, simulador);

//            Console.WriteLine($"[{DateTime.Now:T}] Parcela {sensor.IdParcela} → Humedad: {sensor.HumedadActual}% | Riego: {simulador.EstadoRiego}");
//        };

//        timer.Start();
//        Console.WriteLine("Simulación de sensor iniciada. Presiona Enter para detener...");
//        Console.ReadLine();
//        timer.Stop();

//}
    static void ProbarConexionProgram()
    {
        Console.WriteLine(" Probando conexión a Oracle...");

        ConexionOracle conexion = new ConexionOracle();

        if (conexion.ProbarConexion())
        {
            Console.WriteLine(" Conexión establecida correctamente.");
        }
        else
        {
            Console.WriteLine(" No se pudo conectar a la base de datos.");
        }

        Console.WriteLine("Presiona una tecla para salir...");
        Console.ReadKey();

    }
}