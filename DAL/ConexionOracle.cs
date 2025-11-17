using ENTITY;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class ConexionOracle
    {
        private string cadenaConexion =
            "User Id=Consultor_GreenControl;Password=consultor123;Data Source=localhost:1521/xepdb1;";

        protected OracleConnection conexion;

        public ConexionOracle()
        {
            conexion = new OracleConnection(cadenaConexion);
        }

        public bool AbrirConexion()
        {
            try
            {
                if (conexion.State != ConnectionState.Open)
                {
                    conexion.Open();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al abrir la conexión: {ex.Message}");
                return false;
            }
        }

        public void CerrarConexion()
        {
            try
            {
                if (conexion.State == ConnectionState.Open)
                {
                    conexion.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cerrar conexión: {ex.Message}");
            }
        }

        public OracleConnection ObtenerConexion()
        {
            return conexion;
        }

        public bool ProbarConexion()
        {
            using (var conn = new OracleConnection(cadenaConexion))
            {
                try
                {
                    conn.Open();
                    Console.WriteLine("Conexión a Oracle exitosa.");
                    return true;
                }
                catch (OracleException ex)
                {
                    Console.WriteLine($"Error de conexión Oracle: {ex.Message}");
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error general: {ex.Message}");
                    return false;
                }
            }
        }



    }
}
