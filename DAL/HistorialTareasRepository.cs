using ENTITY;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class HistorialTareasRepository : ConexionOracle, IRepository<HistorialTareas>
    {
        private HistorialTareas Mapear(OracleDataReader reader)
        {
            return new HistorialTareas
            {
                IdHistorialTareas = Convert.ToInt32(reader["IDHISTORIALTAREAS"]),
                IdParcela = Convert.ToInt32(reader["IDPARCELA"]),
                Tipo = reader["TIPO"] != DBNull.Value ? reader["TIPO"].ToString() : null,
                FechaTarea = reader["FECHATAREA"] != DBNull.Value ? Convert.ToDateTime(reader["FECHATAREA"]) : (DateTime?)null,
                EstadoChar = reader["ESTADO"] != DBNull.Value ? reader["ESTADO"].ToString() : "0",
                Urgencia = reader["URGENCIA"] != DBNull.Value ? Convert.ToInt32(reader["URGENCIA"]) : (int?)null,
                Descripcion = reader["DESCRIPCION"] != DBNull.Value ? reader["DESCRIPCION"].ToString() : null
            };
        }

        public Response<HistorialTareas> Insertar(HistorialTareas entidad)
        {
            Response<HistorialTareas> response = new Response<HistorialTareas>();

            string queryId = "SELECT SEQ_HISTORIALTAREAS.NEXTVAL FROM DUAL";
            string queryInsert = @"INSERT INTO HISTORIALTAREAS 
                (IDHISTORIALTAREAS, IDPARCELA, TIPO, FECHATAREA, ESTADO, URGENCIA, DESCRIPCION)
                VALUES (:Id, :IdParcela, :Tipo, :FechaTarea, :Estado, :Urgencia, :Descripcion)";

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
                    command.Parameters.Add(new OracleParameter("Tipo", entidad.Tipo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaTarea", entidad.FechaTarea ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Estado", entidad.EstadoChar ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Urgencia", entidad.Urgencia ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Descripcion", entidad.Descripcion ?? (object)DBNull.Value));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                entidad.IdHistorialTareas = nuevoId;
                response.Estado = true;
                response.Mensaje = "Historial de tarea registrado exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al insertar historial de tarea: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<HistorialTareas> Actualizar(HistorialTareas entidad)
        {
            Response<HistorialTareas> response = new Response<HistorialTareas>();

            string query = @"UPDATE HISTORIALTAREAS SET 
                             IDPARCELA = :IdParcela,
                             TIPO = :Tipo,
                             FECHATAREA = :FechaTarea,
                             ESTADO = :Estado,
                             URGENCIA = :Urgencia,
                             DESCRIPCION = :Descripcion
                             WHERE IDHISTORIALTAREAS = :Id";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    command.Transaction = transaction;
                    command.Parameters.Add(new OracleParameter("IdParcela", entidad.IdParcela));
                    command.Parameters.Add(new OracleParameter("Tipo", entidad.Tipo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaTarea", entidad.FechaTarea ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Estado", entidad.EstadoChar ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Urgencia", entidad.Urgencia ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Descripcion", entidad.Descripcion ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Id", entidad.IdHistorialTareas));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                response.Estado = true;
                response.Mensaje = "Historial de tarea actualizado exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al actualizar historial de tarea: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<HistorialTareas> Eliminar(int id)
        {
            Response<HistorialTareas> response = new Response<HistorialTareas>();

            string query = "DELETE FROM HISTORIALTAREAS WHERE IDHISTORIALTAREAS = :Id";

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
                response.Mensaje = "Historial de tarea eliminado exitosamente";
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al eliminar historial de tarea: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<HistorialTareas> ObtenerPorId(int id)
        {
            Response<HistorialTareas> response = new Response<HistorialTareas>();
            string query = "SELECT * FROM HISTORIALTAREAS WHERE IDHISTORIALTAREAS = :Id";

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
                            response.Mensaje = "Historial de tarea encontrado";
                            response.Entidad = Mapear(reader);
                        }
                        else
                        {
                            response.Estado = false;
                            response.Mensaje = "Historial de tarea no encontrado";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener historial de tarea: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<HistorialTareas> ObtenerTodos()
        {
            Response<HistorialTareas> response = new Response<HistorialTareas>();
            List<HistorialTareas> lista = new List<HistorialTareas>();
            string query = "SELECT * FROM HISTORIALTAREAS ORDER BY FECHATAREA DESC";

            try
            {
                AbrirConexion();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(Mapear(reader));
                        }
                    }
                }

                response.Estado = true;
                response.Mensaje = lista.Count > 0
                    ? $"Se encontraron {lista.Count} registros de historial de tareas"
                    : "No hay registros de historial de tareas";
                response.Lista = lista;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener historial de tareas: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }


    }
}
