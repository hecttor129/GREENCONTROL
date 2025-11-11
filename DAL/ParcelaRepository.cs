using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENTITY;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class ParcelaRepository : ConexionOracle, IRepository<Parcela>
    {
        private Parcela MapearDesdeReader(OracleDataReader reader)
        {
            return new Parcela
            {
                Id = Convert.ToInt32(reader["ID_PARCELA"]),
                IdUsuario = Convert.ToInt32(reader["ID_USUARIO"]),
                Nombre = reader["NOMBRE"] != DBNull.Value ? reader["NOMBRE"].ToString() : null,
                Ubicacion = reader["UBICACION"] != DBNull.Value ? reader["UBICACION"].ToString() : null,
                Area = reader["AREA"] != DBNull.Value ? Convert.ToDouble(reader["AREA"]) : 0
            };
        }

        public Response<Parcela> Insertar(Parcela entidad)
        {
            Response<Parcela> response = new Response<Parcela>();

            string queryId = "SELECT SEQ_PARCELA.NEXTVAL FROM DUAL";
            string queryInsert = "INSERT INTO PARCELA (ID_PARCELA, ID_USUARIO, NOMBRE, UBICACION, AREA) " +
                           "VALUES (:Id, :IdUsuario, :Nombre, :Ubicacion, :Area)";

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
                    command.Parameters.Add(new OracleParameter("Nombre", entidad.Nombre ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Ubicacion", entidad.Ubicacion ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Area", entidad.Area));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                entidad.Id = nuevoId;
                response.Estado = true;
                response.Mensaje = "Parcela registrada exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al insertar parcela: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Parcela> Actualizar(Parcela entidad)
        {
            Response<Parcela> response = new Response<Parcela>();
            string query = "UPDATE PARCELA SET ID_USUARIO = :IdUsuario, NOMBRE = :Nombre, " +
                           "UBICACION = :Ubicacion, AREA = :Area " +
                           "WHERE ID_PARCELA = :Id";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    command.Transaction = transaction;
                    command.Parameters.Add(new OracleParameter("IdUsuario", entidad.IdUsuario));
                    command.Parameters.Add(new OracleParameter("Nombre", entidad.Nombre ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Ubicacion", entidad.Ubicacion ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Area", entidad.Area));
                    command.Parameters.Add(new OracleParameter("Id", entidad.Id));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                response.Estado = true;
                response.Mensaje = "Parcela actualizada exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al actualizar parcela: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Parcela> Eliminar(int id)
        {
            Response<Parcela> response = new Response<Parcela>();
            string query = "DELETE FROM PARCELA WHERE ID_PARCELA = :Id";

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
                response.Mensaje = "Parcela eliminada exitosamente";
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al eliminar parcela: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Parcela> ObtenerPorId(int id)
        {
            Response<Parcela> response = new Response<Parcela>();
            string query = "SELECT * FROM PARCELA WHERE ID_PARCELA = :Id";

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
                            response.Mensaje = "Parcela encontrada";
                            response.Entidad = MapearDesdeReader(reader);
                        }
                        else
                        {
                            response.Estado = false;
                            response.Mensaje = "Parcela no encontrada";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener parcela: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Parcela> ObtenerTodos()
        {
            Response<Parcela> response = new Response<Parcela>();
            List<Parcela> listaParcelas = new List<Parcela>();
            string query = "SELECT * FROM PARCELA ORDER BY ID_PARCELA";

            try
            {
                AbrirConexion();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaParcelas.Add(MapearDesdeReader(reader));
                        }
                    }
                }

                response.Estado = true;
                response.Mensaje = listaParcelas.Count > 0
                    ? $"Se encontraron {listaParcelas.Count} parcelas"
                    : "No hay parcelas registradas";
                response.Lista = listaParcelas;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener parcelas: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        //public Response<Parcela> ObtenerPorUsuario(int idUsuario)
        //{
        //    Response<Parcela> response = new Response<Parcela>();
        //    List<Parcela> listaParcelas = new List<Parcela>();
        //    string query = "SELECT * FROM PARCELA WHERE ID_USUARIO = :IdUsuario ORDER BY NOMBRE";

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
        //                    listaParcelas.Add(Mapear(reader));
        //                }
        //            }
        //        }

        //        response.Estado = true;
        //        response.Mensaje = listaParcelas.Count > 0
        //            ? $"El usuario tiene {listaParcelas.Count} parcelas"
        //            : "El usuario no tiene parcelas registradas";
        //        response.Lista = listaParcelas;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Estado = false;
        //        response.Mensaje = "Error al obtener parcelas del usuario: " + ex.Message;
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return response;
        //}

        //public Response<Parcela> ObtenerPorNombre(string nombre)
        //{
        //    Response<Parcela> response = new Response<Parcela>();
        //    string query = "SELECT * FROM PARCELA WHERE UPPER(NOMBRE) = UPPER(:Nombre)";

        //    try
        //    {
        //        AbrirConexion();

        //        using (OracleCommand command = new OracleCommand(query, conexion))
        //        {
        //            command.Parameters.Add(new OracleParameter("Nombre", nombre));

        //            using (OracleDataReader reader = command.ExecuteReader())
        //            {
        //                if (reader.Read())
        //                {
        //                    response.Estado = true;
        //                    response.Mensaje = "Parcela encontrada";
        //                    response.Entidad = Mapear(reader);
        //                }
        //                else
        //                {
        //                    response.Estado = false;
        //                    response.Mensaje = "Parcela no encontrada";
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Estado = false;
        //        response.Mensaje = "Error al obtener parcela por nombre: " + ex.Message;
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return response;
        //}

        //public Response<Parcela> CalcularAreaTotalUsuario(int idUsuario)
        //{
        //    Response<Parcela> response = new Response<Parcela>();
        //    string query = "SELECT SUM(AREA) AS AREA_TOTAL FROM PARCELA WHERE ID_USUARIO = :IdUsuario";

        //    try
        //    {
        //        AbrirConexion();

        //        using (OracleCommand command = new OracleCommand(query, conexion))
        //        {
        //            command.Parameters.Add(new OracleParameter("IdUsuario", idUsuario));

        //            object resultado = command.ExecuteScalar();

        //            if (resultado != null && resultado != DBNull.Value)
        //            {
        //                double areaTotal = Convert.ToDouble(resultado);
        //                response.Estado = true;
        //                response.Mensaje = $"Área total de parcelas: {areaTotal:N2} m²";
        //            }
        //            else
        //            {
        //                response.Estado = true;
        //                response.Mensaje = "El usuario no tiene parcelas registradas. Área total: 0.00 m²";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Estado = false;
        //        response.Mensaje = "Error al calcular área total: " + ex.Message;
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return response;
        //}

        //public Response<Parcela> ObtenerPorUbicacion(string ubicacion)
        //{
        //    Response<Parcela> response = new Response<Parcela>();
        //    List<Parcela> listaParcelas = new List<Parcela>();
        //    string query = "SELECT * FROM PARCELA WHERE UPPER(UBICACION) LIKE UPPER(:Ubicacion) ORDER BY NOMBRE";

        //    try
        //    {
        //        AbrirConexion();

        //        using (OracleCommand command = new OracleCommand(query, conexion))
        //        {
        //            command.Parameters.Add(new OracleParameter("Ubicacion", "%" + ubicacion + "%"));

        //            using (OracleDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    listaParcelas.Add(Mapear(reader));
        //                }
        //            }
        //        }

        //        response.Estado = true;
        //        response.Mensaje = listaParcelas.Count > 0
        //            ? $"Se encontraron {listaParcelas.Count} parcelas en '{ubicacion}'"
        //            : $"No hay parcelas en '{ubicacion}'";
        //        response.Lista = listaParcelas;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Estado = false;
        //        response.Mensaje = "Error al obtener parcelas por ubicación: " + ex.Message;
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return response;
        //}
    }
}
