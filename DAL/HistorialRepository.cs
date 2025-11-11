using ENTITY;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class HistorialRepository : ConexionOracle, IRepository<Historial>
    {

        private Historial MapearDesdeReader(OracleDataReader reader)
        {
            return new Historial
            {
                Id = Convert.ToInt32(reader["ID_HISTORIAL"]),
                IdParcela = Convert.ToInt32(reader["ID_PARCELA"]),
                HFechaSiembra = Convert.ToDateTime(reader["HFECHASIEMBRA"]),
                HFechaCosecha = reader["HFECHACOSECHA"] != DBNull.Value
                    ? Convert.ToDateTime(reader["HFECHACOSECHA"])
                    : (DateTime?)null,
                DuracionCiclo = reader["DURACIONCICLO"] != DBNull.Value
                    ? Convert.ToInt32(reader["DURACIONCICLO"])
                    : (int?)null,
                EtapaActual = reader["ETAPAACTUAL"] != DBNull.Value
                    ? reader["ETAPAACTUAL"].ToString()
                    : null,
                RegistroInsumos = reader["REGISTROINSUMOS"] != DBNull.Value
                    ? reader["REGISTROINSUMOS"].ToString()
                    : null,
                RegistroTareas = reader["REGISTROTAREAS"] != DBNull.Value
                    ? reader["REGISTROTAREAS"].ToString()
                    : null
            };
        }

        public Response<Historial> Insertar(Historial entidad)
        {
            Response<Historial> response = new Response<Historial>();

            string queryId = "SELECT SEQ_HISTORIAL.NEXTVAL FROM DUAL";
            string queryInsert = "INSERT INTO HISTORIAL (ID_HISTORIAL, ID_PARCELA, HFECHASIEMBRA, " +
                           "HFECHACOSECHA, DURACIONCICLO, ETAPAACTUAL, REGISTROINSUMOS, REGISTROTAREAS) " +
                           "VALUES (:Id, :IdParcela, :HFechaSiembra, :HFechaCosecha, :DuracionCiclo, " +
                           ":EtapaActual, :RegistroInsumos, :RegistroTareas)";

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
                    command.Parameters.Add(new OracleParameter("HFechaSiembra", entidad.HFechaSiembra));
                    command.Parameters.Add(new OracleParameter("HFechaCosecha",
                        entidad.HFechaCosecha.HasValue ? (object)entidad.HFechaCosecha.Value : DBNull.Value));
                    command.Parameters.Add(new OracleParameter("DuracionCiclo",
                        entidad.DuracionCiclo.HasValue ? (object)entidad.DuracionCiclo.Value : DBNull.Value));
                    command.Parameters.Add(new OracleParameter("EtapaActual",
                        entidad.EtapaActual ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("RegistroInsumos", OracleDbType.Clob)
                    {
                        Value = entidad.RegistroInsumos ?? (object)DBNull.Value
                    });
                    command.Parameters.Add(new OracleParameter("RegistroTareas", OracleDbType.Clob)
                    {
                        Value = entidad.RegistroTareas ?? (object)DBNull.Value
                    });

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                entidad.Id = nuevoId;
                response.Estado = true;
                response.Mensaje = "Historial registrado exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al insertar historial: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Historial> Actualizar(Historial entidad)
        {
            Response<Historial> response = new Response<Historial>();
            string query = "UPDATE HISTORIAL SET ID_PARCELA = :IdParcela, HFECHASIEMBRA = :HFechaSiembra, " +
                           "HFECHACOSECHA = :HFechaCosecha, DURACIONCICLO = :DuracionCiclo, " +
                           "ETAPAACTUAL = :EtapaActual, REGISTROINSUMOS = :RegistroInsumos, " +
                           "REGISTROTAREAS = :RegistroTareas " +
                           "WHERE ID_HISTORIAL = :Id";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    command.Transaction = transaction;
                    command.Parameters.Add(new OracleParameter("IdParcela", entidad.IdParcela));
                    command.Parameters.Add(new OracleParameter("HFechaSiembra", entidad.HFechaSiembra));
                    command.Parameters.Add(new OracleParameter("HFechaCosecha",
                        entidad.HFechaCosecha.HasValue ? (object)entidad.HFechaCosecha.Value : DBNull.Value));
                    command.Parameters.Add(new OracleParameter("DuracionCiclo",
                        entidad.DuracionCiclo.HasValue ? (object)entidad.DuracionCiclo.Value : DBNull.Value));
                    command.Parameters.Add(new OracleParameter("EtapaActual",
                        entidad.EtapaActual ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("RegistroInsumos", OracleDbType.Clob)
                    {
                        Value = entidad.RegistroInsumos ?? (object)DBNull.Value
                    });
                    command.Parameters.Add(new OracleParameter("RegistroTareas", OracleDbType.Clob)
                    {
                        Value = entidad.RegistroTareas ?? (object)DBNull.Value
                    });
                    command.Parameters.Add(new OracleParameter("Id", entidad.Id));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                response.Estado = true;
                response.Mensaje = "Historial actualizado exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al actualizar historial: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Historial> Eliminar(int id)
        {
            Response<Historial> response = new Response<Historial>();
            string query = "DELETE FROM HISTORIAL WHERE ID_HISTORIAL = :Id";

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
                response.Mensaje = "Historial eliminado exitosamente";
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al eliminar historial: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Historial> ObtenerPorId(int id)
        {
            Response<Historial> response = new Response<Historial>();
            string query = "SELECT * FROM HISTORIAL WHERE ID_HISTORIAL = :Id";

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
                            response.Mensaje = "Historial encontrado";
                            response.Entidad = MapearDesdeReader(reader);
                        }
                        else
                        {
                            response.Estado = false;
                            response.Mensaje = "Historial no encontrado";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener historial: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Historial> ObtenerTodos()
        {
            Response<Historial> response = new Response<Historial>();
            List<Historial> listaHistoriales = new List<Historial>();
            string query = "SELECT * FROM HISTORIAL ORDER BY HFECHASIEMBRA DESC";

            try
            {
                AbrirConexion();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaHistoriales.Add(MapearDesdeReader(reader));
                        }
                    }
                }

                response.Estado = true;
                response.Mensaje = listaHistoriales.Count > 0
                    ? $"Se encontraron {listaHistoriales.Count} registros de historial"
                    : "No hay registros de historial";
                response.Lista = listaHistoriales;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener historiales: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

 
        //public Response<Historial> ObtenerPorParcela(int idParcela)
        //{
        //    Response<Historial> response = new Response<Historial>();
        //    List<Historial> listaHistoriales = new List<Historial>();
        //    string query = "SELECT * FROM HISTORIAL WHERE ID_PARCELA = :IdParcela ORDER BY HFECHASIEMBRA DESC";

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
        //                    listaHistoriales.Add(Mapear(reader));
        //                }
        //            }
        //        }

        //        response.Estado = true;
        //        response.Mensaje = listaHistoriales.Count > 0
        //            ? $"La parcela tiene {listaHistoriales.Count} registros de historial"
        //            : "La parcela no tiene registros de historial";
        //        response.Lista = listaHistoriales;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Estado = false;
        //        response.Mensaje = "Error al obtener historiales de la parcela: " + ex.Message;
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return response;
        //}

        //public Response<Historial> ObtenerPorEtapa(string etapa)
        //{
        //    Response<Historial> response = new Response<Historial>();
        //    List<Historial> listaHistoriales = new List<Historial>();
        //    string query = "SELECT * FROM HISTORIAL WHERE UPPER(ETAPAACTUAL) = UPPER(:Etapa) ORDER BY HFECHASIEMBRA DESC";

        //    try
        //    {
        //        AbrirConexion();

        //        using (OracleCommand command = new OracleCommand(query, conexion))
        //        {
        //            command.Parameters.Add(new OracleParameter("Etapa", etapa));

        //            using (OracleDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    listaHistoriales.Add(Mapear(reader));
        //                }
        //            }
        //        }

        //        response.Estado = true;
        //        response.Mensaje = listaHistoriales.Count > 0
        //            ? $"Se encontraron {listaHistoriales.Count} registros en etapa '{etapa}'"
        //            : $"No hay registros en etapa '{etapa}'";
        //        response.Lista = listaHistoriales;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Estado = false;
        //        response.Mensaje = "Error al obtener historiales por etapa: " + ex.Message;
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return response;
        //}

        //public Response<Historial> ObtenerPorRangoFechas(DateTime fechaInicio, DateTime fechaFin)
        //{
        //    Response<Historial> response = new Response<Historial>();
        //    List<Historial> listaHistoriales = new List<Historial>();
        //    string query = "SELECT * FROM HISTORIAL WHERE HFECHASIEMBRA BETWEEN :FechaInicio AND :FechaFin " +
        //                  "ORDER BY HFECHASIEMBRA DESC";

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
        //                    listaHistoriales.Add(Mapear(reader));
        //                }
        //            }
        //        }

        //        response.Estado = true;
        //        response.Mensaje = listaHistoriales.Count > 0
        //            ? $"Se encontraron {listaHistoriales.Count} registros entre {fechaInicio:dd/MM/yyyy} y {fechaFin:dd/MM/yyyy}"
        //            : $"No hay registros entre {fechaInicio:dd/MM/yyyy} y {fechaFin:dd/MM/yyyy}";
        //        response.Lista = listaHistoriales;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Estado = false;
        //        response.Mensaje = "Error al obtener historiales por rango de fechas: " + ex.Message;
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return response;
        //}

        //public Response<Historial> ObtenerCiclosCompletados()
        //{
        //    Response<Historial> response = new Response<Historial>();
        //    List<Historial> listaHistoriales = new List<Historial>();
        //    string query = "SELECT * FROM HISTORIAL WHERE HFECHACOSECHA IS NOT NULL ORDER BY HFECHACOSECHA DESC";

        //    try
        //    {
        //        AbrirConexion();

        //        using (OracleCommand command = new OracleCommand(query, conexion))
        //        {
        //            using (OracleDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    listaHistoriales.Add(Mapear(reader));
        //                }
        //            }
        //        }

        //        response.Estado = true;
        //        response.Mensaje = listaHistoriales.Count > 0
        //            ? $"Se encontraron {listaHistoriales.Count} ciclos completados"
        //            : "No hay ciclos completados";
        //        response.Lista = listaHistoriales;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Estado = false;
        //        response.Mensaje = "Error al obtener ciclos completados: " + ex.Message;
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return response;
        //}

        //public Response<Historial> ObtenerCiclosActivos()
        //{
        //    Response<Historial> response = new Response<Historial>();
        //    List<Historial> listaHistoriales = new List<Historial>();
        //    string query = "SELECT * FROM HISTORIAL WHERE HFECHACOSECHA IS NULL ORDER BY HFECHASIEMBRA DESC";

        //    try
        //    {
        //        AbrirConexion();

        //        using (OracleCommand command = new OracleCommand(query, conexion))
        //        {
        //            using (OracleDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    listaHistoriales.Add(Mapear(reader));
        //                }
        //            }
        //        }

        //        response.Estado = true;
        //        response.Mensaje = listaHistoriales.Count > 0
        //            ? $"Se encontraron {listaHistoriales.Count} ciclos activos"
        //            : "No hay ciclos activos";
        //        response.Lista = listaHistoriales;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Estado = false;
        //        response.Mensaje = "Error al obtener ciclos activos: " + ex.Message;
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return response;
        //}

        //public Response<Historial> CalcularPromedioDuracionCiclos()
        //{
        //    Response<Historial> response = new Response<Historial>();
        //    string query = "SELECT AVG(DURACIONCICLO) AS PROMEDIO FROM HISTORIAL WHERE DURACIONCICLO IS NOT NULL";

        //    try
        //    {
        //        AbrirConexion();

        //        using (OracleCommand command = new OracleCommand(query, conexion))
        //        {
        //            object resultado = command.ExecuteScalar();

        //            if (resultado != null && resultado != DBNull.Value)
        //            {
        //                double promedio = Convert.ToDouble(resultado);
        //                response.Estado = true;
        //                response.Mensaje = $"Promedio de duración de ciclos: {promedio:N2} días";
        //            }
        //            else
        //            {
        //                response.Estado = true;
        //                response.Mensaje = "No hay ciclos con duración registrada";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Estado = false;
        //        response.Mensaje = "Error al calcular promedio de duración: " + ex.Message;
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return response;
        //}
    }
}
