using ENTITY;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class TareasRepository : ConexionOracle, IRepository<Tareas>
    {
        private Tareas Mapear(OracleDataReader reader)
        {
            return new Tareas
            {
                IdTarea = Convert.ToInt32(reader["IDTAREA"]),
                IdSiembra = Convert.ToInt32(reader["IDSIEMBRA"]),
                Tipo = reader["TIPO"] != DBNull.Value ? reader["TIPO"].ToString() : null,
                FechaTareaProgramada = Convert.ToDateTime(reader["FECHATAREAPROGRAMADA"]),
                FechaTareaTerminada = reader["FECHATAREATERMINADA"] != DBNull.Value ? Convert.ToDateTime(reader["FECHATAREATERMINADA"]) : (DateTime?)null,
                Urgencia = reader["URGENCIA"] != DBNull.Value ? Convert.ToInt32(reader["URGENCIA"]) : (int?)null,
                Descripcion = reader["DESCRIPCIÓN"] != DBNull.Value ? reader["DESCRIPCIÓN"].ToString() : null,
                Estado = reader["ESTADO"] != DBNull.Value ? reader["ESTADO"].ToString() : "1"
            };
        }

        public Response<Tareas> Insertar(Tareas entidad)
        {
            var response = new Response<Tareas>();
            string queryId = "SELECT SEQ_TAREAS.NEXTVAL FROM DUAL";
            string queryInsert = @"INSERT INTO TAREAS (IDTAREA, IDSIEMBRA, TIPO, FECHATAREAPROGRAMADA, 
                                   FECHATAREATERMINADA, URGENCIA, DESCRIPCIÓN, ESTADO)
                                   VALUES (:Id, :IdSiembra, :Tipo, :FechaTareaProgramada, 
                                   :FechaTareaTerminada, :Urgencia, :Descripcion, :Estado)";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                int nuevoId;
                using (var cmdId = conexion.CreateCommand())
                {
                    cmdId.Transaction = transaction;
                    cmdId.CommandText = queryId;
                    nuevoId = Convert.ToInt32(cmdId.ExecuteScalar());
                }

                using (var cmd = conexion.CreateCommand())
                {
                    cmd.Transaction = transaction;
                    cmd.CommandText = queryInsert;
                    cmd.Parameters.Add(new OracleParameter("Id", nuevoId));
                    cmd.Parameters.Add(new OracleParameter("IdSiembra", entidad.IdSiembra));
                    cmd.Parameters.Add(new OracleParameter("Tipo", entidad.Tipo ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("FechaTareaProgramada", entidad.FechaTareaProgramada));
                    cmd.Parameters.Add(new OracleParameter("FechaTareaTerminada", entidad.FechaTareaTerminada ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("Urgencia", entidad.Urgencia ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("Descripcion", OracleDbType.Clob) { Value = entidad.Descripcion ?? (object)DBNull.Value });
                    cmd.Parameters.Add(new OracleParameter("Estado", entidad.Estado ?? "1"));

                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
                entidad.IdTarea = nuevoId;
                response.Estado = true;
                response.Mensaje = "Tareas registrada exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al insertar tarea: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Tareas> Actualizar(Tareas entidad)
        {
            var response = new Response<Tareas>();
            string query = @"UPDATE TAREAS SET 
                             IDSIEMBRA = :IdSiembra,
                             TIPO = :Tipo,
                             FECHATAREAPROGRAMADA = :FechaTareaProgramada,
                             FECHATAREATERMINADA = :FechaTareaTerminada,
                             URGENCIA = :Urgencia,
                             DESCRIPCIÓN = :Descripcion,
                             ESTADO = :Estado
                             WHERE IDTAREA = :Id";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                using (var cmd = conexion.CreateCommand())
                {
                    cmd.Transaction = transaction;
                    cmd.CommandText = query;
                    cmd.Parameters.Add(new OracleParameter("IdSiembra", entidad.IdSiembra));
                    cmd.Parameters.Add(new OracleParameter("Tipo", entidad.Tipo ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("FechaTareaProgramada", entidad.FechaTareaProgramada));
                    cmd.Parameters.Add(new OracleParameter("FechaTareaTerminada", entidad.FechaTareaTerminada ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("Urgencia", entidad.Urgencia ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("Descripcion", OracleDbType.Clob) { Value = entidad.Descripcion ?? (object)DBNull.Value });
                    cmd.Parameters.Add(new OracleParameter("Estado", entidad.Estado ?? "1"));
                    cmd.Parameters.Add(new OracleParameter("Id", entidad.IdTarea));

                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
                response.Estado = true;
                response.Mensaje = "Tareas actualizada exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al actualizar tarea: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Tareas> Eliminar(int id)
        {
            var response = new Response<Tareas>();
            string query = "UPDATE TAREAS SET ESTADO = '0' WHERE IDTAREA = :Id";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                using (var cmd = conexion.CreateCommand())
                {
                    cmd.Transaction = transaction;
                    cmd.CommandText = query;
                    cmd.Parameters.Add(new OracleParameter("Id", id));
                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
                response.Estado = true;
                response.Mensaje = "Tareas eliminada exitosamente";
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al eliminar tarea: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Tareas> ObtenerPorId(int id)
        {
            var response = new Response<Tareas>();
            string query = "SELECT * FROM TAREAS WHERE IDTAREA = :Id AND ESTADO = '1'";

            try
            {
                AbrirConexion();

                using (var cmd = conexion.CreateCommand())
                {
                    cmd.CommandText = query;
                    cmd.Parameters.Add(new OracleParameter("Id", id));

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            response.Estado = true;
                            response.Mensaje = "Tareas encontrada";
                            response.Entidad = Mapear(reader);
                        }
                        else
                        {
                            response.Estado = false;
                            response.Mensaje = "Tareas no encontrada";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener tarea: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Tareas> ObtenerTodos()
        {
            var response = new Response<Tareas>();
            var listaTareas = new List<Tareas>();
            string query = "SELECT * FROM TAREAS WHERE ESTADO = '1' ORDER BY FECHATAREAPROGRAMADA DESC";

            try
            {
                AbrirConexion();

                using (var cmd = conexion.CreateCommand())
                {
                    cmd.CommandText = query;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaTareas.Add(Mapear(reader));
                        }
                    }
                }

                response.Estado = true;
                response.Mensaje = listaTareas.Count > 0
                    ? $"Se encontraron {listaTareas.Count} tareas"
                    : "No hay tareas registradas";
                response.Lista = listaTareas;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener tareas: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Tareas> ObtenerPorSiembra(int idSiembra)
        {
            Response<Tareas> response = new Response<Tareas>();
            List<Tareas> listaTareas = new List<Tareas>();
            string query = "SELECT * FROM TAREAS WHERE IDSIEMBRA = :IdSiembra AND ESTADO = '1' ORDER BY FECHATAREAPROGRAMADA DESC";

            try
            {
                AbrirConexion();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    command.Parameters.Add(new OracleParameter("IdSiembra", idSiembra));

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaTareas.Add(Mapear(reader));
                        }
                    }
                }

                response.Estado = true;
                response.Lista = listaTareas;
                response.Mensaje = listaTareas.Count > 0
                    ? $"La siembra tiene {listaTareas.Count} tareas"
                    : "La siembra no tiene tareas registradas";
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener tareas de la siembra: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }
    }
}
