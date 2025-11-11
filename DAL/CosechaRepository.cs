using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENTITY;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class CosechaRepository : ConexionOracle, IRepository<Cosecha>
    {
        public Response<Cosecha> Insertar(Cosecha entidad)
        {
            Response<Cosecha> response = new Response<Cosecha>();

            // Primero obtenemos el siguiente ID de la secuencia
            string queryId = "SELECT SEQ_COSECHA.NEXTVAL FROM DUAL";
            string queryInsert = @"INSERT INTO COSECHA 
                (ID_COSECHA, ID_SIEMBRA, FECHACOSECHAREAL, FECHACOSECHAESTIMADA, 
                 FECHAGERMINACION, FECHAFLORACION, FECHASIEMBRA, PORCENTAJEDESARROLLO, ESTADO) 
                VALUES 
                (:Id, :IdSiembra, :FechaCosechaReal, :FechaCosechaEstimada, 
                 :FechaGerminacion, :FechaFloracion, :FechaSiembra, :PorcentajeDesarrollo, :Estado)";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                // Obtener el siguiente ID
                int nuevoId;
                using (OracleCommand commandId = new OracleCommand(queryId, conexion))
                {
                    commandId.Transaction = transaction;
                    nuevoId = Convert.ToInt32(commandId.ExecuteScalar());
                }

                // Insertar con el ID obtenido
                using (OracleCommand command = new OracleCommand(queryInsert, conexion))
                {
                    command.Transaction = transaction;
                    command.Parameters.Add(new OracleParameter("Id", nuevoId));
                    command.Parameters.Add(new OracleParameter("IdSiembra", entidad.IdSiembra));
                    command.Parameters.Add(new OracleParameter("FechaCosechaReal", entidad.FechaCosechaReal));
                    command.Parameters.Add(new OracleParameter("FechaCosechaEstimada",
                        entidad.FechaCosechaEstimada.HasValue ? (object)entidad.FechaCosechaEstimada.Value : DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaGerminacion",
                        entidad.FechaGerminacion.HasValue ? (object)entidad.FechaGerminacion.Value : DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaFloracion",
                        entidad.FechaFloracion.HasValue ? (object)entidad.FechaFloracion.Value : DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaSiembra", entidad.FechaSiembra));
                    command.Parameters.Add(new OracleParameter("PorcentajeDesarrollo",
                        entidad.PorcentajeDesarrollo.HasValue ? (object)entidad.PorcentajeDesarrollo.Value : DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Estado", entidad.Estado ?? "1"));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                entidad.Id = nuevoId;
                response.Estado = true;
                response.Mensaje = "Cosecha registrada exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al insertar cosecha: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Cosecha> Actualizar(Cosecha entidad)
        {
            Response<Cosecha> response = new Response<Cosecha>();
            string query = @"UPDATE COSECHA SET 
                ID_SIEMBRA = :IdSiembra,
                FECHACOSECHAREAL = :FechaCosechaReal,
                FECHACOSECHAESTIMADA = :FechaCosechaEstimada,
                FECHAGERMINACION = :FechaGerminacion,
                FECHAFLORACION = :FechaFloracion,
                FECHASIEMBRA = :FechaSiembra,
                PORCENTAJEDESARROLLO = :PorcentajeDesarrollo,
                ESTADO = :Estado
                WHERE ID_COSECHA = :Id";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    command.Transaction = transaction;
                    command.Parameters.Add(new OracleParameter("IdSiembra", entidad.IdSiembra));
                    command.Parameters.Add(new OracleParameter("FechaCosechaReal", entidad.FechaCosechaReal));
                    command.Parameters.Add(new OracleParameter("FechaCosechaEstimada",
                        entidad.FechaCosechaEstimada.HasValue ? (object)entidad.FechaCosechaEstimada.Value : DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaGerminacion",
                        entidad.FechaGerminacion.HasValue ? (object)entidad.FechaGerminacion.Value : DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaFloracion",
                        entidad.FechaFloracion.HasValue ? (object)entidad.FechaFloracion.Value : DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaSiembra", entidad.FechaSiembra));
                    command.Parameters.Add(new OracleParameter("PorcentajeDesarrollo",
                        entidad.PorcentajeDesarrollo.HasValue ? (object)entidad.PorcentajeDesarrollo.Value : DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Estado", entidad.Estado ?? "1"));
                    command.Parameters.Add(new OracleParameter("Id", entidad.Id));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                response.Estado = true;
                response.Mensaje = "Cosecha actualizada exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al actualizar cosecha: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Cosecha> Eliminar(int id)
        {
            Response<Cosecha> response = new Response<Cosecha>();
            string query = "DELETE FROM COSECHA WHERE ID_COSECHA = :Id";

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
                response.Mensaje = "Cosecha eliminada exitosamente";
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al eliminar cosecha: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Cosecha> ObtenerPorId(int id)
        {
            Response<Cosecha> response = new Response<Cosecha>();
            string query = "SELECT * FROM COSECHA WHERE ID_COSECHA = :Id";

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
                            Cosecha cosecha = Mapear(reader);

                            response.Estado = true;
                            response.Mensaje = "Cosecha encontrada";
                            response.Entidad = cosecha;
                        }
                        else
                        {
                            response.Estado = false;
                            response.Mensaje = "Cosecha no encontrada";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener cosecha: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Cosecha> ObtenerTodos()
        {
            Response<Cosecha> response = new Response<Cosecha>();
            List<Cosecha> listaCosechas = new List<Cosecha>();
            string query = "SELECT * FROM COSECHA ORDER BY FECHACOSECHAREAL DESC";

            try
            {
                AbrirConexion();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaCosechas.Add(Mapear(reader));
                        }
                    }
                }

                response.Estado = true;
                response.Mensaje = listaCosechas.Count > 0
                    ? $"Se encontraron {listaCosechas.Count} cosechas"
                    : "No hay cosechas registradas";
                response.Lista = listaCosechas;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener cosechas: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        //public Response<Cosecha> ObtenerPorSiembra(int idSiembra)
        //{
        //    Response<Cosecha> response = new Response<Cosecha>();
        //    List<Cosecha> listaCosechas = new List<Cosecha>();
        //    string query = "SELECT * FROM COSECHA WHERE ID_SIEMBRA = :IdSiembra ORDER BY FECHACOSECHAREAL DESC";

        //    try
        //    {
        //        AbrirConexion();

        //        using (OracleCommand command = new OracleCommand(query, conexion))
        //        {
        //            command.Parameters.Add(new OracleParameter("IdSiembra", idSiembra));

        //            using (OracleDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    listaCosechas.Add(Mapear(reader));
        //                }
        //            }
        //        }

        //        response.Estado = true;
        //        response.Mensaje = listaCosechas.Count > 0
        //            ? $"Se encontraron {listaCosechas.Count} cosechas para esta siembra"
        //            : "Esta siembra no tiene cosechas registradas";
        //        response.Lista = listaCosechas;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Estado = false;
        //        response.Mensaje = "Error al obtener cosechas de la siembra: " + ex.Message;
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return response;
        //}

        //public Response<Cosecha> ObtenerActivas()
        //{
        //    Response<Cosecha> response = new Response<Cosecha>();
        //    List<Cosecha> listaCosechas = new List<Cosecha>();
        //    string query = "SELECT * FROM COSECHA WHERE ESTADO = '1' ORDER BY FECHACOSECHAREAL DESC";

        //    try
        //    {
        //        AbrirConexion();

        //        using (OracleCommand command = new OracleCommand(query, conexion))
        //        {
        //            using (OracleDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    listaCosechas.Add(Mapear(reader));
        //                }
        //            }
        //        }

        //        response.Estado = true;
        //        response.Mensaje = listaCosechas.Count > 0
        //            ? $"Se encontraron {listaCosechas.Count} cosechas activas"
        //            : "No hay cosechas activas";
        //        response.Lista = listaCosechas;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Estado = false;
        //        response.Mensaje = "Error al obtener cosechas activas: " + ex.Message;
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return response;
        //}

        //public Response<Cosecha> CambiarEstado(int id, string nuevoEstado)
        //{
        //    Response<Cosecha> response = new Response<Cosecha>();
        //    string query = "UPDATE COSECHA SET ESTADO = :Estado WHERE ID_COSECHA = :Id";

        //    OracleTransaction transaction = null;

        //    try
        //    {
        //        // Validar estado
        //        if (nuevoEstado != "0" && nuevoEstado != "1")
        //        {
        //            response.Estado = false;
        //            response.Mensaje = "Estado inválido. Debe ser '0' (inactivo) o '1' (activo)";
        //            return response;
        //        }

        //        AbrirConexion();
        //        transaction = conexion.BeginTransaction();

        //        using (OracleCommand command = new OracleCommand(query, conexion))
        //        {
        //            command.Transaction = transaction;
        //            command.Parameters.Add(new OracleParameter("Estado", nuevoEstado));
        //            command.Parameters.Add(new OracleParameter("Id", id));

        //            int filasAfectadas = command.ExecuteNonQuery();

        //            if (filasAfectadas > 0)
        //            {
        //                transaction.Commit();
        //                response.Estado = true;
        //                response.Mensaje = nuevoEstado == "1"
        //                    ? "Cosecha activada exitosamente"
        //                    : "Cosecha desactivada exitosamente";
        //            }
        //            else
        //            {
        //                transaction.Rollback();
        //                response.Estado = false;
        //                response.Mensaje = "No se encontró la cosecha con el ID especificado";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        transaction?.Rollback();
        //        response.Estado = false;
        //        response.Mensaje = "Error al cambiar estado de cosecha: " + ex.Message;
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return response;
        //}

        private Cosecha Mapear(OracleDataReader reader)
        {
            return new Cosecha
            {
                Id = Convert.ToInt32(reader["ID_COSECHA"]),
                IdSiembra = Convert.ToInt32(reader["ID_SIEMBRA"]),
                FechaCosechaReal = Convert.ToDateTime(reader["FECHACOSECHAREAL"]),
                FechaCosechaEstimada = reader["FECHACOSECHAESTIMADA"] != DBNull.Value
                    ? Convert.ToDateTime(reader["FECHACOSECHAESTIMADA"])
                    : (DateTime?)null,
                FechaGerminacion = reader["FECHAGERMINACION"] != DBNull.Value
                    ? Convert.ToDateTime(reader["FECHAGERMINACION"])
                    : (DateTime?)null,
                FechaFloracion = reader["FECHAFLORACION"] != DBNull.Value
                    ? Convert.ToDateTime(reader["FECHAFLORACION"])
                    : (DateTime?)null,
                FechaSiembra = Convert.ToDateTime(reader["FECHASIEMBRA"]),
                PorcentajeDesarrollo = reader["PORCENTAJEDESARROLLO"] != DBNull.Value
                    ? Convert.ToDecimal(reader["PORCENTAJEDESARROLLO"])
                    : (decimal?)null,
                Estado = reader["ESTADO"].ToString()
            };
        }

        //public Response<Cosecha> ObtenerPorRangoFechas(DateTime fechaInicio, DateTime fechaFin)
        //{
        //    Response<Cosecha> response = new Response<Cosecha>();
        //    List<Cosecha> listaCosechas = new List<Cosecha>();
        //    string query = @"SELECT * FROM COSECHA 
        //        WHERE FECHACOSECHAREAL BETWEEN :FechaInicio AND :FechaFin 
        //        ORDER BY FECHACOSECHAREAL DESC";

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
        //                    listaCosechas.Add(Mapear(reader));
        //                }
        //            }
        //        }

        //        response.Estado = true;
        //        response.Mensaje = listaCosechas.Count > 0
        //            ? $"Se encontraron {listaCosechas.Count} cosechas en el rango de fechas"
        //            : "No hay cosechas en el rango de fechas especificado";
        //        response.Lista = listaCosechas;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Estado = false;
        //        response.Mensaje = "Error al obtener cosechas por rango de fechas: " + ex.Message;
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return response;
        //}
    }
}
