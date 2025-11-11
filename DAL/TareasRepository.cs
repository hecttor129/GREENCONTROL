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
                Id = Convert.ToInt32(reader["ID_TAREA"]),
                IdParcela = Convert.ToInt32(reader["ID_PARCELA"]),
                TipoTarea = reader["TIPOTAREA"] != DBNull.Value
                    ? reader["TIPOTAREA"].ToString()
                    : null,
                FechaTarea = reader["FECHATAREA"] != DBNull.Value
                    ? Convert.ToDateTime(reader["FECHATAREA"])
                    : (DateTime?)null,
                Urgencia = reader["URGENCIA"] != DBNull.Value
                    ? Convert.ToInt32(reader["URGENCIA"])
                    : (int?)null,
                EstadoChar = reader["ESTADO"] != DBNull.Value
                    ? reader["ESTADO"].ToString()
                    : "0",
                Costo = reader["COSTO"] != DBNull.Value
                    ? Convert.ToDecimal(reader["COSTO"])
                    : (decimal?)null
            };
        }

        public Response<Tareas> Insertar(Tareas entidad)
        {
            Response<Tareas> response = new Response<Tareas>();

            string queryId = "SELECT SEQ_TAREAS.NEXTVAL FROM DUAL";
            string queryInsert = "INSERT INTO TAREAS (ID_TAREA, ID_PARCELA, TIPOTAREA, FECHATAREA, " +
                           "URGENCIA, ESTADO, COSTO) " +
                           "VALUES (:Id, :IdParcela, :TipoTarea, :FechaTarea, :Urgencia, :Estado, :Costo)";

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
                    command.Parameters.Add(new OracleParameter("TipoTarea",
                        entidad.TipoTarea ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaTarea",
                        entidad.FechaTarea.HasValue ? (object)entidad.FechaTarea.Value : DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Urgencia",
                        entidad.Urgencia.HasValue ? (object)entidad.Urgencia.Value : DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Estado",
                        entidad.EstadoChar ?? "0"));
                    command.Parameters.Add(new OracleParameter("Costo",
                        entidad.Costo.HasValue ? (object)entidad.Costo.Value : DBNull.Value));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                entidad.Id = nuevoId;
                response.Estado = true;
                response.Mensaje = "Tarea registrada exitosamente";
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
            Response<Tareas> response = new Response<Tareas>();
            string query = "UPDATE TAREAS SET ID_PARCELA = :IdParcela, TIPOTAREA = :TipoTarea, " +
                           "FECHATAREA = :FechaTarea, URGENCIA = :Urgencia, ESTADO = :Estado, " +
                           "COSTO = :Costo " +
                           "WHERE ID_TAREA = :Id";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    command.Transaction = transaction;
                    command.Parameters.Add(new OracleParameter("IdParcela", entidad.IdParcela));
                    command.Parameters.Add(new OracleParameter("TipoTarea",
                        entidad.TipoTarea ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaTarea",
                        entidad.FechaTarea.HasValue ? (object)entidad.FechaTarea.Value : DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Urgencia",
                        entidad.Urgencia.HasValue ? (object)entidad.Urgencia.Value : DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Estado",
                        entidad.EstadoChar ?? "0"));
                    command.Parameters.Add(new OracleParameter("Costo",
                        entidad.Costo.HasValue ? (object)entidad.Costo.Value : DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Id", entidad.Id));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                response.Estado = true;
                response.Mensaje = "Tarea actualizada exitosamente";
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
            Response<Tareas> response = new Response<Tareas>();
            string query = "DELETE FROM TAREAS WHERE ID_TAREA = :Id";

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
                response.Mensaje = "Tarea eliminada exitosamente";
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
            Response<Tareas> response = new Response<Tareas>();
            string query = "SELECT * FROM TAREAS WHERE ID_TAREA = :Id";

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
                            response.Mensaje = "Tarea encontrada";
                            response.Entidad = Mapear(reader);
                        }
                        else
                        {
                            response.Estado = false;
                            response.Mensaje = "Tarea no encontrada";
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
            Response<Tareas> response = new Response<Tareas>();
            List<Tareas> listaTareas = new List<Tareas>();
            string query = "SELECT * FROM TAREAS ORDER BY FECHATAREA DESC";

            try
            {
                AbrirConexion();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    using (OracleDataReader reader = command.ExecuteReader())
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


        //public Response<Tareas> ObtenerPorParcela(int idParcela)
        //{
        //    Response<Tareas> response = new Response<Tareas>();
        //    List<Tareas> listaTareas = new List<Tareas>();
        //    string query = "SELECT * FROM TAREAS WHERE ID_PARCELA = :IdParcela ORDER BY FECHATAREA DESC";

        //    try
        //    {
        //        AbrirConexion();

        //        using (OracleCommand command = new OracleCommand(query, conexion))
        //        {
        //            command.Parameters.Add(new OracleParameter("IdParcela", idParcela));

        //            using (OracleDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    listaTareas.Add(Mapear(reader));
        //                }
        //            }
        //        }

        //        response.Estado = true;
        //        response.Mensaje = listaTareas.Count > 0
        //            ? $"La parcela tiene {listaTareas.Count} tareas"
        //            : "La parcela no tiene tareas registradas";
        //        response.Lista = listaTareas;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Estado = false;
        //        response.Mensaje = "Error al obtener tareas de la parcela: " + ex.Message;
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return response;
        //}

        //public Response<Tareas> ObtenerPorTipo(string tipoTarea)
        //{
        //    Response<Tareas> response = new Response<Tareas>();
        //    List<Tareas> listaTareas = new List<Tareas>();
        //    string query = "SELECT * FROM TAREAS WHERE UPPER(TIPOTAREA) = UPPER(:TipoTarea) ORDER BY FECHATAREA DESC";

        //    try
        //    {
        //        AbrirConexion();

        //        using (OracleCommand command = new OracleCommand(query, conexion))
        //        {
        //            command.Parameters.Add(new OracleParameter("TipoTarea", tipoTarea));

        //            using (OracleDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    listaTareas.Add(Mapear(reader));
        //                }
        //            }
        //        }

        //        response.Estado = true;
        //        response.Mensaje = listaTareas.Count > 0
        //            ? $"Se encontraron {listaTareas.Count} tareas de tipo '{tipoTarea}'"
        //            : $"No hay tareas de tipo '{tipoTarea}'";
        //        response.Lista = listaTareas;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Estado = false;
        //        response.Mensaje = "Error al obtener tareas por tipo: " + ex.Message;
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return response;
        //}

        //public Response<Tareas> ObtenerTareasPendientes()
        //{
        //    Response<Tareas> response = new Response<Tareas>();
        //    List<Tareas> listaTareas = new List<Tareas>();
        //    string query = "SELECT * FROM TAREAS WHERE ESTADO = '0' ORDER BY URGENCIA DESC, FECHATAREA";

        //    try
        //    {
        //        AbrirConexion();

        //        using (OracleCommand command = new OracleCommand(query, conexion))
        //        {
        //            using (OracleDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    listaTareas.Add(Mapear(reader));
        //                }
        //            }
        //        }

        //        response.Estado = true;
        //        response.Mensaje = listaTareas.Count > 0
        //            ? $"Hay {listaTareas.Count} tareas pendientes"
        //            : "No hay tareas pendientes";
        //        response.Lista = listaTareas;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Estado = false;
        //        response.Mensaje = "Error al obtener tareas pendientes: " + ex.Message;
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return response;
        //}

        //public Response<Tareas> ObtenerTareasCompletadas()
        //{
        //    Response<Tareas> response = new Response<Tareas>();
        //    List<Tareas> listaTareas = new List<Tareas>();
        //    string query = "SELECT * FROM TAREAS WHERE ESTADO = '1' ORDER BY FECHATAREA DESC";

        //    try
        //    {
        //        AbrirConexion();

        //        using (OracleCommand command = new OracleCommand(query, conexion))
        //        {
        //            using (OracleDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    listaTareas.Add(Mapear(reader));
        //                }
        //            }
        //        }

        //        response.Estado = true;
        //        response.Mensaje = listaTareas.Count > 0
        //            ? $"Hay {listaTareas.Count} tareas completadas"
        //            : "No hay tareas completadas";
        //        response.Lista = listaTareas;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Estado = false;
        //        response.Mensaje = "Error al obtener tareas completadas: " + ex.Message;
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return response;
        //}

        //public Response<Tareas> ObtenerPorUrgencia(int urgencia)
        //{
        //    Response<Tareas> response = new Response<Tareas>();
        //    List<Tareas> listaTareas = new List<Tareas>();
        //    string query = "SELECT * FROM TAREAS WHERE URGENCIA = :Urgencia ORDER BY FECHATAREA";

        //    try
        //    {
        //        AbrirConexion();

        //        using (OracleCommand command = new OracleCommand(query, conexion))
        //        {
        //            command.Parameters.Add(new OracleParameter("Urgencia", urgencia));

        //            using (OracleDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    listaTareas.Add(Mapear(reader));
        //                }
        //            }
        //        }

        //        response.Estado = true;
        //        response.Mensaje = listaTareas.Count > 0
        //            ? $"Se encontraron {listaTareas.Count} tareas con urgencia {urgencia}"
        //            : $"No hay tareas con urgencia {urgencia}";
        //        response.Lista = listaTareas;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Estado = false;
        //        response.Mensaje = "Error al obtener tareas por urgencia: " + ex.Message;
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return response;
        //}

        //public Response<Tareas> MarcarComoCompletada(int id)
        //{
        //    Response<Tareas> response = new Response<Tareas>();
        //    string query = "UPDATE TAREAS SET ESTADO = '1' WHERE ID_TAREA = :Id";

        //    OracleTransaction transaction = null;

        //    try
        //    {
        //        AbrirConexion();
        //        transaction = conexion.BeginTransaction();

        //        using (OracleCommand command = new OracleCommand(query, conexion))
        //        {
        //            command.Transaction = transaction;
        //            command.Parameters.Add(new OracleParameter("Id", id));

        //            int filasAfectadas = command.ExecuteNonQuery();

        //            if (filasAfectadas > 0)
        //            {
        //                transaction.Commit();
        //                response.Estado = true;
        //                response.Mensaje = "Tarea marcada como completada";
        //            }
        //            else
        //            {
        //                transaction.Rollback();
        //                response.Estado = false;
        //                response.Mensaje = "No se encontró la tarea";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        transaction?.Rollback();
        //        response.Estado = false;
        //        response.Mensaje = "Error al marcar tarea como completada: " + ex.Message;
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return response;
        //}

        //public Response<Tareas> MarcarComoPendiente(int id)
        //{
        //    Response<Tareas> response = new Response<Tareas>();
        //    string query = "UPDATE TAREAS SET ESTADO = '0' WHERE ID_TAREA = :Id";

        //    OracleTransaction transaction = null;

        //    try
        //    {
        //        AbrirConexion();
        //        transaction = conexion.BeginTransaction();

        //        using (OracleCommand command = new OracleCommand(query, conexion))
        //        {
        //            command.Transaction = transaction;
        //            command.Parameters.Add(new OracleParameter("Id", id));

        //            int filasAfectadas = command.ExecuteNonQuery();

        //            if (filasAfectadas > 0)
        //            {
        //                transaction.Commit();
        //                response.Estado = true;
        //                response.Mensaje = "Tarea marcada como pendiente";
        //            }
        //            else
        //            {
        //                transaction.Rollback();
        //                response.Estado = false;
        //                response.Mensaje = "No se encontró la tarea";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        transaction?.Rollback();
        //        response.Estado = false;
        //        response.Mensaje = "Error al marcar tarea como pendiente: " + ex.Message;
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return response;
        //}

        //public Response<Tareas> CalcularCostoTotalParcela(int idParcela)
        //{
        //    Response<Tareas> response = new Response<Tareas>();
        //    string query = "SELECT SUM(COSTO) AS COSTO_TOTAL FROM TAREAS WHERE ID_PARCELA = :IdParcela AND COSTO IS NOT NULL";

        //    try
        //    {
        //        AbrirConexion();

        //        using (OracleCommand command = new OracleCommand(query, conexion))
        //        {
        //            command.Parameters.Add(new OracleParameter("IdParcela", idParcela));

        //            object resultado = command.ExecuteScalar();

        //            if (resultado != null && resultado != DBNull.Value)
        //            {
        //                decimal costoTotal = Convert.ToDecimal(resultado);
        //                response.Estado = true;
        //                response.Mensaje = $"Costo total de tareas: ${costoTotal:N2}";
        //            }
        //            else
        //            {
        //                response.Estado = true;
        //                response.Mensaje = "La parcela no tiene tareas con costo registrado. Costo total: $0.00";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Estado = false;
        //        response.Mensaje = "Error al calcular costo total: " + ex.Message;
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return response;
        //}

        //public Response<Tareas> ObtenerPorRangoFechas(DateTime fechaInicio, DateTime fechaFin)
        //{
        //    Response<Tareas> response = new Response<Tareas>();
        //    List<Tareas> listaTareas = new List<Tareas>();
        //    string query = "SELECT * FROM TAREAS WHERE FECHATAREA BETWEEN :FechaInicio AND :FechaFin ORDER BY FECHATAREA";

        //    try
        //    {
        //        AbrirConexion();

        //        using (OracleCommand command = new OracleCommand(query, conexion))
        //        {
        //            command.Parameters.Add(new OracleParameter("FechaInicio", fechaInicio));
        //            command.Parameters.Add(new OracleParameter("FechaFin", fechaFin));

        //            using (OracleDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    listaTareas.Add(Mapear(reader));
        //                }
        //            }
        //        }

        //        response.Estado = true;
        //        response.Mensaje = listaTareas.Count > 0
        //            ? $"Se encontraron {listaTareas.Count} tareas entre {fechaInicio:dd/MM/yyyy} y {fechaFin:dd/MM/yyyy}"
        //            : $"No hay tareas entre {fechaInicio:dd/MM/yyyy} y {fechaFin:dd/MM/yyyy}";
        //        response.Lista = listaTareas;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Estado = false;
        //        response.Mensaje = "Error al obtener tareas por rango de fechas: " + ex.Message;
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return response;
        //}
    }
}
