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
        private Historial Mapear(OracleDataReader reader)
        {
            return new Historial
            {
                IdHistorial = Convert.ToInt32(reader["IDHISTORIAL"]),
                IdParcela = Convert.ToInt32(reader["IDPARCELA"]),
                FechaSiembra = reader["FECHASIEMBRA"] != DBNull.Value ? Convert.ToDateTime(reader["FECHASIEMBRA"]) : (DateTime?)null,
                FechaCosecha = reader["FECHACOSECHA"] != DBNull.Value ? Convert.ToDateTime(reader["FECHACOSECHA"]) : (DateTime?)null,
                DiasGerminacion = reader["DIASGERMINACION"] != DBNull.Value ? Convert.ToInt32(reader["DIASGERMINACION"]) : (int?)null,
                DiasFloracion = reader["DIASFLORACION"] != DBNull.Value ? Convert.ToInt32(reader["DIASFLORACION"]) : (int?)null,
                DuracionCiclo = Convert.ToInt32(reader["DURACIONCICLO"]),
                CalidadCosechada = reader["CALIDADCOSECHADA"] != DBNull.Value ? Convert.ToInt32(reader["CALIDADCOSECHADA"]) : (int?)null,
                CantidadCosechada = reader["CANTIDADCOSECHADA"] != DBNull.Value ? Convert.ToDecimal(reader["CANTIDADCOSECHADA"]) : (decimal?)null,
                NombreCultivo = reader["NOMBRECULTIVO"].ToString(),
                NombreParcela = reader["NOMBREPARCELA"].ToString(),
                TipoSuelo = reader["TIPOSUELO"] != DBNull.Value ? reader["TIPOSUELO"].ToString() : null,
                PhSuelo = reader["PHSUELO"] != DBNull.Value ? Convert.ToDecimal(reader["PHSUELO"]) : (decimal?)null,
                Estado = reader["ESTADO"] != DBNull.Value ? Convert.ToChar(reader["ESTADO"]) : '1',
                CostoTotalProduccion = reader["COSTOTOTALPRODUCCION"] != DBNull.Value ? Convert.ToDecimal(reader["COSTOTOTALPRODUCCION"]) : (decimal?)null,
                IngresoTotal = reader["INGRESOTOTAL"] != DBNull.Value ? Convert.ToDecimal(reader["INGRESOTOTAL"]) : (decimal?)null,
                RentabilidadFinal = reader["RENTABILIDADFINAL"] != DBNull.Value ? Convert.ToDecimal(reader["RENTABILIDADFINAL"]) : (decimal?)null,
                FechaSnapshot = reader["FECHASNAPSHOT"] != DBNull.Value ? Convert.ToDateTime(reader["FECHASNAPSHOT"]) : (DateTime?)null
            };
        }

        public Response<Historial> Insertar(Historial entidad)
        {
            Response<Historial> response = new Response<Historial>();
            string queryId = "SELECT SEQ_HISTORIAL.NEXTVAL FROM DUAL";
            string queryInsert = @"INSERT INTO HISTORIAL (
                IDHISTORIAL, IDPARCELA, FECHASIEMBRA, FECHACOSECHA, DIASGERMINACION, DIASFLORACION, 
                DURACIONCICLO, CALIDADCOSECHADA, CANTIDADCOSECHADA, NOMBRECULTIVO, NOMBREPARCELA, 
                TIPOSUELO, PHSUELO, ESTADO, COSTOTOTALPRODUCCION, INGRESOTOTAL, RENTABILIDADFINAL, FECHASNAPSHOT
            ) VALUES (
                :Id, :IdParcela, :FechaSiembra, :FechaCosecha, :DiasGerminacion, :DiasFloracion,
                :DuracionCiclo, :CalidadCosechada, :CantidadCosechada, :NombreCultivo, :NombreParcela,
                :TipoSuelo, :PhSuelo, :Estado, :CostoTotal, :IngresoTotal, :Rentabilidad, :FechaSnapshot
            )";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                int nuevoId;
                using (OracleCommand cmdId = new OracleCommand(queryId, conexion))
                {
                    cmdId.Transaction = transaction;
                    nuevoId = Convert.ToInt32(cmdId.ExecuteScalar());
                }

                using (OracleCommand cmd = new OracleCommand(queryInsert, conexion))
                {
                    cmd.Transaction = transaction;
                    cmd.Parameters.Add(new OracleParameter("Id", nuevoId));
                    cmd.Parameters.Add(new OracleParameter("IdParcela", entidad.IdParcela));
                    cmd.Parameters.Add(new OracleParameter("FechaSiembra", entidad.FechaSiembra ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("FechaCosecha", entidad.FechaCosecha ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("DiasGerminacion", entidad.DiasGerminacion ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("DiasFloracion", entidad.DiasFloracion ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("DuracionCiclo", entidad.DuracionCiclo));
                    cmd.Parameters.Add(new OracleParameter("CalidadCosechada", entidad.CalidadCosechada ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("CantidadCosechada", entidad.CantidadCosechada ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("NombreCultivo", entidad.NombreCultivo));
                    cmd.Parameters.Add(new OracleParameter("NombreParcela", entidad.NombreParcela));
                    cmd.Parameters.Add(new OracleParameter("TipoSuelo", entidad.TipoSuelo ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("PhSuelo", entidad.PhSuelo ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("Estado", entidad.Estado));
                    cmd.Parameters.Add(new OracleParameter("CostoTotal", entidad.CostoTotalProduccion ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("IngresoTotal", entidad.IngresoTotal ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("Rentabilidad", entidad.RentabilidadFinal ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("FechaSnapshot", entidad.FechaSnapshot ?? (object)DBNull.Value));

                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
                entidad.IdHistorial = nuevoId;
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
            string query = @"UPDATE HISTORIAL SET 
                IDPARCELA = :IdParcela,
                FECHASIEMBRA = :FechaSiembra,
                FECHACOSECHA = :FechaCosecha,
                DIASGERMINACION = :DiasGerminacion,
                DIASFLORACION = :DiasFloracion,
                DURACIONCICLO = :DuracionCiclo,
                CALIDADCOSECHADA = :CalidadCosechada,
                CANTIDADCOSECHADA = :CantidadCosechada,
                NOMBRECULTIVO = :NombreCultivo,
                NOMBREPARCELA = :NombreParcela,
                TIPOSUELO = :TipoSuelo,
                PHSUELO = :PhSuelo,
                ESTADO = :Estado,
                COSTOTOTALPRODUCCION = :CostoTotal,
                INGRESOTOTAL = :IngresoTotal,
                RENTABILIDADFINAL = :Rentabilidad,
                FECHASNAPSHOT = :FechaSnapshot
            WHERE IDHISTORIAL = :Id";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                using (OracleCommand cmd = new OracleCommand(query, conexion))
                {
                    cmd.Transaction = transaction;
                    cmd.Parameters.Add(new OracleParameter("IdParcela", entidad.IdParcela));
                    cmd.Parameters.Add(new OracleParameter("FechaSiembra", entidad.FechaSiembra ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("FechaCosecha", entidad.FechaCosecha ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("DiasGerminacion", entidad.DiasGerminacion ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("DiasFloracion", entidad.DiasFloracion ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("DuracionCiclo", entidad.DuracionCiclo));
                    cmd.Parameters.Add(new OracleParameter("CalidadCosechada", entidad.CalidadCosechada ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("CantidadCosechada", entidad.CantidadCosechada ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("NombreCultivo", entidad.NombreCultivo));
                    cmd.Parameters.Add(new OracleParameter("NombreParcela", entidad.NombreParcela));
                    cmd.Parameters.Add(new OracleParameter("TipoSuelo", entidad.TipoSuelo ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("PhSuelo", entidad.PhSuelo ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("Estado", entidad.Estado));
                    cmd.Parameters.Add(new OracleParameter("CostoTotal", entidad.CostoTotalProduccion ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("IngresoTotal", entidad.IngresoTotal ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("Rentabilidad", entidad.RentabilidadFinal ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("FechaSnapshot", entidad.FechaSnapshot ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("Id", entidad.IdHistorial));

                    cmd.ExecuteNonQuery();
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
            string query = "UPDATE HISTORIAL SET ESTADO = '0' WHERE IDHISTORIAL = :Id";

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
            string query = "SELECT * FROM HISTORIAL WHERE IDHISTORIAL = :Id AND ESTADO = '1'";

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
                            response.Entidad = Mapear(reader);
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
            List<Historial> lista = new List<Historial>();
            string query = "SELECT * FROM HISTORIAL WHERE ESTADO = '1' ORDER BY FECHASNAPSHOT DESC";

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
                    ? $"Se encontraron {lista.Count} registros de historial"
                    : "No hay registros de historial";
                response.Lista = lista;
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

        public Response<Historial> ObtenerPorParcela(int idParcela)
        {
            Response<Historial> response = new Response<Historial>();
            List<Historial> lista = new List<Historial>();
            string query = "SELECT * FROM HISTORIAL WHERE IDPARCELA = :IdParcela AND ESTADO = '1' ORDER BY FECHASNAPSHOT DESC";

            try
            {
                AbrirConexion();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    command.Parameters.Add(new OracleParameter("IdParcela", idParcela));

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(Mapear(reader));
                        }
                    }
                }

                response.Estado = true;
                response.Lista = lista;
                response.Mensaje = lista.Count > 0
                    ? $"Se encontraron {lista.Count} registros de historial para esta parcela"
                    : "Esta parcela no tiene registros de historial";
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener historial de la parcela: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }
    }
}
