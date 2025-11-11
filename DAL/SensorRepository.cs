using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENTITY;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class SensorRepository : ConexionOracle, IRepository<Sensor>
    {
        private Sensor Mapear(OracleDataReader reader)
        {
            return new Sensor
            {
                Id = Convert.ToInt32(reader["ID_SENSOR"]),
                IdParcela = Convert.ToInt32(reader["ID_PARCELA"]),
                EstadoChar = reader["ESTADO"] != DBNull.Value ? reader["ESTADO"].ToString() : "1",
                FrecuenciaLectura = reader["FRECUENCIALECTURA"] != DBNull.Value ? Convert.ToInt32(reader["FRECUENCIALECTURA"]) : (int?)null,
                HumedadActual = reader["HUMEDADACTUAL"] != DBNull.Value ? Convert.ToDecimal(reader["HUMEDADACTUAL"]) : (decimal?)null
            };
        }

        public Response<Sensor> Insertar(Sensor entidad)
        {
            Response<Sensor> response = new Response<Sensor>();

            string queryId = "SELECT SEQ_SENSOR.NEXTVAL FROM DUAL";
            string queryInsert = "INSERT INTO SENSOR (ID_SENSOR, ID_PARCELA, ESTADO, FRECUENCIALECTURA, HUMEDADACTUAL) " +
                                 "VALUES (:Id, :IdParcela, :Estado, :FrecuenciaLectura, :HumedadActual)";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                int nuevoId;
                using (OracleCommand commandId = new OracleCommand(queryId, conexion))
                {
                    commandId.Transaction = transaction;
                    nuevoId = Convert.ToInt32(commandId.ExecuteScalar());
                }

                using (OracleCommand command = new OracleCommand(queryInsert, conexion))
                {
                    command.Transaction = transaction;
                    command.Parameters.Add(new OracleParameter("Id", nuevoId));
                    command.Parameters.Add(new OracleParameter("IdParcela", entidad.IdParcela));
                    command.Parameters.Add(new OracleParameter("Estado", entidad.EstadoChar));
                    command.Parameters.Add(new OracleParameter("FrecuenciaLectura", entidad.FrecuenciaLectura ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("HumedadActual", entidad.HumedadActual ?? (object)DBNull.Value));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                entidad.Id = nuevoId;
                response.Estado = true;
                response.Mensaje = "Sensor registrado exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al insertar sensor: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Sensor> Actualizar(Sensor entidad)
        {
            Response<Sensor> response = new Response<Sensor>();
            string query = "UPDATE SENSOR SET ID_PARCELA = :IdParcela, ESTADO = :Estado, " +
                           "FRECUENCIALECTURA = :FrecuenciaLectura, HUMEDADACTUAL = :HumedadActual " +
                           "WHERE ID_SENSOR = :Id";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    command.Transaction = transaction;
                    command.Parameters.Add(new OracleParameter("IdParcela", entidad.IdParcela));
                    command.Parameters.Add(new OracleParameter("Estado", entidad.EstadoChar));
                    command.Parameters.Add(new OracleParameter("FrecuenciaLectura", entidad.FrecuenciaLectura ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("HumedadActual", entidad.HumedadActual ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Id", entidad.Id));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                response.Estado = true;
                response.Mensaje = "Sensor actualizado exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al actualizar sensor: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Sensor> Eliminar(int id)
        {
            Response<Sensor> response = new Response<Sensor>();
            string query = "DELETE FROM SENSOR WHERE ID_SENSOR = :Id";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    command.Transaction = transaction;
                    command.Parameters.Add(new OracleParameter("Id", id));
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                response.Estado = true;
                response.Mensaje = "Sensor eliminado exitosamente";
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al eliminar sensor: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Sensor> ObtenerPorId(int id)
        {
            Response<Sensor> response = new Response<Sensor>();
            string query = "SELECT * FROM SENSOR WHERE ID_SENSOR = :Id";

            try
            {
                AbrirConexion();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    command.Parameters.Add(new OracleParameter("Id", id));

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            response.Estado = true;
                            response.Mensaje = "Sensor encontrado";
                            response.Entidad = Mapear(reader);
                        }
                        else
                        {
                            response.Estado = false;
                            response.Mensaje = "Sensor no encontrado";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener sensor: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Sensor> ObtenerTodos()
        {
            Response<Sensor> response = new Response<Sensor>();
            List<Sensor> listaSensores = new List<Sensor>();
            string query = "SELECT * FROM SENSOR ORDER BY ID_SENSOR";

            try
            {
                AbrirConexion();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaSensores.Add(Mapear(reader));
                        }
                    }
                }

                response.Estado = true;
                response.Mensaje = listaSensores.Count > 0
                    ? $"Se encontraron {listaSensores.Count} sensores"
                    : "No hay sensores registrados";
                response.Lista = listaSensores;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener sensores: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }
    }
}
