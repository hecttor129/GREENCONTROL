using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENTITY;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class NotificacionesRepository : ConexionOracle, IRepository<Notificaciones>
    {

        private Notificaciones Mapear(OracleDataReader reader)
        {
            return new Notificaciones
            {
                Id = Convert.ToInt32(reader["ID_NOTIFICACIONES"]),
                IdUsuario = Convert.ToInt32(reader["ID_USUARIO"]),
                Titulo = reader["TITULO"] != DBNull.Value ? reader["TITULO"].ToString() : null,
                Mensaje = reader["MENSAJE"] != DBNull.Value ? reader["MENSAJE"].ToString() : null,
                FechaEnvio = reader["FECHAENVIO"] != DBNull.Value ? Convert.ToDateTime(reader["FECHAENVIO"]) : DateTime.Now,
                Tipo = reader["TIPO"] != DBNull.Value ? reader["TIPO"].ToString() : null,
                Leido = reader["LEIDO"] != DBNull.Value && reader["LEIDO"].ToString() == "1"
            };
        }

        public Response<Notificaciones> Insertar(Notificaciones entidad)
        {
            Response<Notificaciones> response = new Response<Notificaciones>();

            string queryId = "SELECT SEQ_NOTIFICACIONES.NEXTVAL FROM DUAL";
            string queryInsert = @"INSERT INTO NOTIFICACIONES 
                (ID_NOTIFICACIONES, ID_USUARIO, TITULO, MENSAJE, FECHAENVIO, TIPO, LEIDO)
                VALUES (:Id, :IdUsuario, :Titulo, :Mensaje, :FechaEnvio, :Tipo, :Leido)";

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
                    command.Parameters.Add(new OracleParameter("IdUsuario", entidad.IdUsuario));
                    command.Parameters.Add(new OracleParameter("Titulo", entidad.Titulo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Mensaje", entidad.Mensaje ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaEnvio", entidad.FechaEnvio));
                    command.Parameters.Add(new OracleParameter("Tipo", entidad.Tipo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Leido", entidad.Leido ? "1" : "0"));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                entidad.Id = nuevoId;
                response.Estado = true;
                response.Mensaje = "Notificación registrada exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al insertar notificación: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Notificaciones> Actualizar(Notificaciones entidad)
        {
            Response<Notificaciones> response = new Response<Notificaciones>();

            string query = @"UPDATE NOTIFICACIONES SET 
                ID_USUARIO = :IdUsuario,
                TITULO = :Titulo,
                MENSAJE = :Mensaje,
                FECHAENVIO = :FechaEnvio,
                TIPO = :Tipo,
                LEIDO = :Leido
                WHERE ID_NOTIFICACIONES = :Id";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    command.Transaction = transaction;
                    command.Parameters.Add(new OracleParameter("IdUsuario", entidad.IdUsuario));
                    command.Parameters.Add(new OracleParameter("Titulo", entidad.Titulo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Mensaje", entidad.Mensaje ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaEnvio", entidad.FechaEnvio));
                    command.Parameters.Add(new OracleParameter("Tipo", entidad.Tipo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Leido", entidad.Leido ? "1" : "0"));
                    command.Parameters.Add(new OracleParameter("Id", entidad.Id));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                response.Estado = true;
                response.Mensaje = "Notificación actualizada exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al actualizar notificación: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Notificaciones> Eliminar(int id)
        {
            Response<Notificaciones> response = new Response<Notificaciones>();
            string query = "DELETE FROM NOTIFICACIONES WHERE ID_NOTIFICACIONES = :Id";

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
                response.Mensaje = "Notificación eliminada exitosamente";
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al eliminar notificación: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Notificaciones> ObtenerPorId(int id)
        {
            Response<Notificaciones> response = new Response<Notificaciones>();
            string query = "SELECT * FROM NOTIFICACIONES WHERE ID_NOTIFICACIONES = :Id";

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
                            response.Mensaje = "Notificación encontrada";
                            response.Entidad = Mapear(reader);
                        }
                        else
                        {
                            response.Estado = false;
                            response.Mensaje = "Notificación no encontrada";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener notificación: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Notificaciones> ObtenerTodos()
        {
            Response<Notificaciones> response = new Response<Notificaciones>();
            List<Notificaciones> lista = new List<Notificaciones>();
            string query = "SELECT * FROM NOTIFICACIONES ORDER BY FECHAENVIO DESC";

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
                response.Mensaje = lista.Count > 0 ? $"Se encontraron {lista.Count} notificaciones" : "No hay notificaciones registradas";
                response.Lista = lista;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener notificaciones: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        //public Response<Notificaciones> ObtenerPorUsuario(int idUsuario)
        //{
        //    Response<Notificaciones> response = new Response<Notificaciones>();
        //    List<Notificaciones> lista = new List<Notificaciones>();
        //    string query = "SELECT * FROM NOTIFICACIONES WHERE ID_USUARIO = :IdUsuario ORDER BY FECHAENVIO DESC";

        //    try
        //    {
        //        AbrirConexion();

        //        using (OracleCommand command = new OracleCommand(query, conexion))
        //        {
        //            command.Parameters.Add(new OracleParameter("IdUsuario", idUsuario));

        //            using (OracleDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    lista.Add(Mapear(reader));
        //                }
        //            }
        //        }

        //        response.Estado = true;
        //        response.Mensaje = lista.Count > 0
        //            ? $"Se encontraron {lista.Count} notificaciones para el usuario {idUsuario}"
        //            : "El usuario no tiene notificaciones";
        //        response.Lista = lista;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Estado = false;
        //        response.Mensaje = "Error al obtener notificaciones del usuario: " + ex.Message;
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return response;
        //}

        //public Response<Notificaciones> MarcarComoLeida(int id)
        //{
        //    Response<Notificaciones> response = new Response<Notificaciones>();
        //    string query = "UPDATE NOTIFICACIONES SET LEIDO = '1' WHERE ID_NOTIFICACIONES = :Id";

        //    try
        //    {
        //        AbrirConexion();

        //        using (OracleCommand command = new OracleCommand(query, conexion))
        //        {
        //            command.Parameters.Add(new OracleParameter("Id", id));
        //            int filas = command.ExecuteNonQuery();

        //            response.Estado = filas > 0;
        //            response.Mensaje = filas > 0
        //                ? "Notificación marcada como leída"
        //                : "No se encontró la notificación especificada";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Estado = false;
        //        response.Mensaje = "Error al marcar notificación como leída: " + ex.Message;
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return response;
        //}

        //public Response<Notificaciones> ObtenerNoLeidasPorUsuario(int idUsuario)
        //{
        //    Response<Notificaciones> response = new Response<Notificaciones>();
        //    List<Notificaciones> lista = new List<Notificaciones>();
        //    string query = @"SELECT * FROM NOTIFICACIONES 
        //             WHERE ID_USUARIO = :IdUsuario AND LEIDO = '0'
        //             ORDER BY FECHAENVIO DESC";

        //    try
        //    {
        //        AbrirConexion();

        //        using (OracleCommand command = new OracleCommand(query, conexion))
        //        {
        //            command.Parameters.Add(new OracleParameter("IdUsuario", idUsuario));

        //            using (OracleDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    lista.Add(Mapear(reader));
        //                }
        //            }
        //        }

        //        response.Estado = true;
        //        response.Mensaje = lista.Count > 0
        //            ? $"Se encontraron {lista.Count} notificaciones no leídas para el usuario {idUsuario}"
        //            : "No hay notificaciones no leídas para este usuario";
        //        response.Lista = lista;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Estado = false;
        //        response.Mensaje = "Error al obtener notificaciones no leídas: " + ex.Message;
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return response;
        //}



    }
}
