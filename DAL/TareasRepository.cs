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
                IdParcela = Convert.ToInt32(reader["IDPARCELA"]),
                Tipo = reader["TIPO"] != DBNull.Value ? reader["TIPO"].ToString() : string.Empty,
                FechaTarea = reader["FECHATAREA"] != DBNull.Value ? Convert.ToDateTime(reader["FECHATAREA"]) : (DateTime?)null,
                EstadoChar = reader["ESTADO"] != DBNull.Value ? reader["ESTADO"].ToString() : "0",
                Urgencia = reader["URGENCIA"] != DBNull.Value ? Convert.ToInt32(reader["URGENCIA"]) : (int?)null,
                Costo = reader["COSTO"] != DBNull.Value ? Convert.ToDecimal(reader["COSTO"]) : (decimal?)null
            };
        }

        public Response<Tareas> Insertar(Tareas entidad)
        {
            var response = new Response<Tareas>();
            string queryId = "SELECT SEQ_TAREAS.NEXTVAL FROM DUAL";
            string queryInsert = @"INSERT INTO TAREAS (IDTAREA, IDPARCELA, TIPO, FECHATAREA, URGENCIA, ESTADO, COSTO)
                                   VALUES (:Id, :IdParcela, :Tipo, :FechaTarea, :Urgencia, :Estado, :Costo)";

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

                    cmd.Parameters.Add(new OracleParameter("Id", OracleDbType.Int32) { Value = nuevoId });
                    cmd.Parameters.Add(new OracleParameter("IdParcela", OracleDbType.Int32) { Value = entidad.IdParcela });
                    cmd.Parameters.Add(new OracleParameter("Tipo", OracleDbType.Varchar2) { Value = (object)entidad.Tipo ?? DBNull.Value });
                    cmd.Parameters.Add(new OracleParameter("FechaTarea", OracleDbType.Date) { Value = entidad.FechaTarea ?? (object)DBNull.Value });
                    cmd.Parameters.Add(new OracleParameter("Urgencia", OracleDbType.Int32) { Value = (object)entidad.Urgencia ?? DBNull.Value });
                    cmd.Parameters.Add(new OracleParameter("Estado", OracleDbType.Varchar2) { Value = entidad.EstadoChar ?? "0" });
                    cmd.Parameters.Add(new OracleParameter("Costo", OracleDbType.Decimal) { Value = entidad.Costo ?? (object)DBNull.Value });

                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();

                entidad.IdTarea = nuevoId;
                response.Estado = true;
                response.Mensaje = "Tarea registrada exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                try { transaction?.Rollback(); } catch { /* ignorar */ }
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
            string query = @"UPDATE TAREAS
                             SET IDPARCELA = :IdParcela,
                                 TIPO = :Tipo,
                                 FECHATAREA = :FechaTarea,
                                 URGENCIA = :Urgencia,
                                 ESTADO = :Estado,
                                 COSTO = :Costo
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

                    cmd.Parameters.Add(new OracleParameter("IdParcela", OracleDbType.Int32) { Value = entidad.IdParcela });
                    cmd.Parameters.Add(new OracleParameter("Tipo", OracleDbType.Varchar2) { Value = (object)entidad.Tipo ?? DBNull.Value });
                    cmd.Parameters.Add(new OracleParameter("FechaTarea", OracleDbType.Date) { Value = entidad.FechaTarea ?? (object)DBNull.Value });
                    cmd.Parameters.Add(new OracleParameter("Urgencia", OracleDbType.Int32) { Value = (object)entidad.Urgencia ?? DBNull.Value });
                    cmd.Parameters.Add(new OracleParameter("Estado", OracleDbType.Varchar2) { Value = entidad.EstadoChar ?? "0" });
                    cmd.Parameters.Add(new OracleParameter("Costo", OracleDbType.Decimal) { Value = entidad.Costo ?? (object)DBNull.Value });
                    cmd.Parameters.Add(new OracleParameter("Id", OracleDbType.Int32) { Value = entidad.IdTarea });

                    int filas = cmd.ExecuteNonQuery();
                    if (filas == 0)
                    {
                        transaction.Rollback();
                        response.Estado = false;
                        response.Mensaje = "No se encontró la tarea para actualizar.";
                        return response;
                    }
                }

                transaction.Commit();
                response.Estado = true;
                response.Mensaje = "Tarea actualizada exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                try { transaction?.Rollback(); } catch { }
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
            string query = "DELETE FROM TAREAS WHERE IDTAREA = :Id";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                using (var cmd = conexion.CreateCommand())
                {
                    cmd.Transaction = transaction;
                    cmd.CommandText = query;
                    cmd.Parameters.Add(new OracleParameter("Id", OracleDbType.Int32) { Value = id });

                    int filas = cmd.ExecuteNonQuery();
                    if (filas == 0)
                    {
                        transaction.Rollback();
                        response.Estado = false;
                        response.Mensaje = "No se encontró la tarea para eliminar.";
                        return response;
                    }
                }

                transaction.Commit();
                response.Estado = true;
                response.Mensaje = "Tarea eliminada exitosamente";
            }
            catch (Exception ex)
            {
                try { transaction?.Rollback(); } catch { }
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
            string query = "SELECT * FROM TAREAS WHERE IDTAREA = :Id";

            try
            {
                AbrirConexion();

                using (var cmd = conexion.CreateCommand())
                {
                    cmd.CommandText = query;
                    cmd.Parameters.Add(new OracleParameter("Id", OracleDbType.Int32) { Value = id });

                    using (var reader = cmd.ExecuteReader())
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
                            response.Entidad = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener tarea: " + ex.Message;
                response.Entidad = null;
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
            string query = "SELECT * FROM TAREAS ORDER BY FECHATAREA DESC";

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
                response.Mensaje = listaTareas.Count > 0 ? $"Se encontraron {listaTareas.Count} tareas" : "No hay tareas registradas";
                response.Lista = listaTareas;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener tareas: " + ex.Message;
                response.Lista = null;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        //public decimal CalcularCostoTotalTareas(int idParcela)
        //{
        //    const string query = "SELECT NVL(SUM(COSTO), 0) FROM TAREAS WHERE IDPARCELA = :IdParcela";
        //    decimal costoTotal = 0;

        //    try
        //    {
        //        AbrirConexion();

        //        using (OracleCommand command = new OracleCommand(query, conexion))
        //        {
        //            command.Parameters.Add(new OracleParameter("IdParcela", idParcela));
        //            object resultado = command.ExecuteScalar();

        //            if (resultado != null && resultado != DBNull.Value)
        //                costoTotal = Convert.ToDecimal(resultado);
        //        }
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return costoTotal;
        //}

        //public Response<Tareas> ObtenerPorParcela(int idParcela)
        //{
        //    Response<Tareas> response = new Response<Tareas>();
        //    List<Tareas> listaTareas = new List<Tareas>();
        //    string query = "SELECT * FROM TAREAS WHERE IDPARCELA = :IdParcela ORDER BY FECHATAREA DESC";

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
        //    string query = "SELECT * FROM TAREAS WHERE UPPER(TIPO) = UPPER(:TipoTarea) ORDER BY FECHATAREA DESC";

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
        //    string query = "UPDATE TAREAS SET ESTADO = '1' WHERE IDTAREA = :Id";

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
        //    string query = "UPDATE TAREAS SET ESTADO = '0' WHERE IDTAREA = :Id";

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
