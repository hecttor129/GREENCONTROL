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
                IdHistorial = Convert.ToInt32(reader["IDHISTORIAL"]),
                IdParcela = reader["IDPARCELA"] != DBNull.Value ? Convert.ToInt32(reader["IDPARCELA"]) : (int?)null,
                FechaCosecha = reader["FECHACOSECHA"] != DBNull.Value ? Convert.ToDateTime(reader["FECHACOSECHA"]) : (DateTime?)null,
                FechaSiembra = reader["FECHASIEMBRA"] != DBNull.Value ? Convert.ToDateTime(reader["FECHASIEMBRA"]) : (DateTime?)null,
                EtapaActual = reader["ETAPAACTUAL"] != DBNull.Value ? reader["ETAPAACTUAL"].ToString() : null,
                CalidadCosechada = reader["CALIDADCOSECHADA"] != DBNull.Value ? Convert.ToInt32(reader["CALIDADCOSECHADA"]) : (int?)null,
                CantidadCosechada = reader["CANTIDADCOSECHADA"] != DBNull.Value ? Convert.ToDecimal(reader["CANTIDADCOSECHADA"]) : (decimal?)null,
                DuracionCiclo = reader["DURACIONCICLO"] != DBNull.Value ? Convert.ToInt32(reader["DURACIONCICLO"]) : (int?)null,
                NombreCultivo = reader["NOMBRECULTIVO"] != DBNull.Value ? reader["NOMBRECULTIVO"].ToString() : null,
                NombreParcela = reader["NOMBREPARCELA"] != DBNull.Value ? reader["NOMBREPARCELA"].ToString() : null,
                TipoSuelo = reader["TIPOSUELO"] != DBNull.Value ? reader["TIPOSUELO"].ToString() : null,
                PhSuelo = reader["PHSUELO"] != DBNull.Value ? Convert.ToDecimal(reader["PHSUELO"]) : (decimal?)null,
                EstadoChar = reader["ESTADO"] != DBNull.Value ? reader["ESTADO"].ToString() : null,
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
                IDHISTORIAL, IDPARCELA, FECHACOSECHA, FECHASIEMBRA, ETAPAACTUAL, 
                CALIDADCOSECHADA, CANTIDADCOSECHADA, DURACIONCICLO, NOMBRECULTIVO, 
                NOMBREPARCELA, TIPOSUELO, PHSUELO, ESTADO, COSTOTOTALPRODUCCION, 
                INGRESOTOTAL, RENTABILIDADFINAL, FECHASNAPSHOT) 
                VALUES (
                :Id, :IdParcela, :FechaCosecha, :FechaSiembra, :EtapaActual,
                :CalidadCosechada, :CantidadCosechada, :DuracionCiclo, :NombreCultivo,
                :NombreParcela, :TipoSuelo, :PhSuelo, :Estado, :CostoTotalProduccion,
                :IngresoTotal, :RentabilidadFinal, :FechaSnapshot)";

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
                    command.Parameters.Add(new OracleParameter("IdParcela", entidad.IdParcela ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaCosecha", entidad.FechaCosecha ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaSiembra", entidad.FechaSiembra ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("EtapaActual", entidad.EtapaActual ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("CalidadCosechada", entidad.CalidadCosechada ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("CantidadCosechada", entidad.CantidadCosechada ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("DuracionCiclo", entidad.DuracionCiclo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("NombreCultivo", entidad.NombreCultivo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("NombreParcela", entidad.NombreParcela ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("TipoSuelo", entidad.TipoSuelo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("PhSuelo", entidad.PhSuelo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Estado", entidad.EstadoChar ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("CostoTotalProduccion", entidad.CostoTotalProduccion ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("IngresoTotal", entidad.IngresoTotal ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("RentabilidadFinal", entidad.RentabilidadFinal ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaSnapshot", entidad.FechaSnapshot ?? (object)DBNull.Value));

                    command.ExecuteNonQuery();
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
                FECHACOSECHA = :FechaCosecha,
                FECHASIEMBRA = :FechaSiembra,
                ETAPAACTUAL = :EtapaActual,
                CALIDADCOSECHADA = :CalidadCosechada,
                CANTIDADCOSECHADA = :CantidadCosechada,
                DURACIONCICLO = :DuracionCiclo,
                NOMBRECULTIVO = :NombreCultivo,
                NOMBREPARCELA = :NombreParcela,
                TIPOSUELO = :TipoSuelo,
                PHSUELO = :PhSuelo,
                ESTADO = :Estado,
                COSTOTOTALPRODUCCION = :CostoTotalProduccion,
                INGRESOTOTAL = :IngresoTotal,
                RENTABILIDADFINAL = :RentabilidadFinal,
                FECHASNAPSHOT = :FechaSnapshot
                WHERE IDHISTORIAL = :Id";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    command.Transaction = transaction;
                    command.Parameters.Add(new OracleParameter("IdParcela", entidad.IdParcela ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaCosecha", entidad.FechaCosecha ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaSiembra", entidad.FechaSiembra ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("EtapaActual", entidad.EtapaActual ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("CalidadCosechada", entidad.CalidadCosechada ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("CantidadCosechada", entidad.CantidadCosechada ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("DuracionCiclo", entidad.DuracionCiclo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("NombreCultivo", entidad.NombreCultivo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("NombreParcela", entidad.NombreParcela ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("TipoSuelo", entidad.TipoSuelo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("PhSuelo", entidad.PhSuelo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Estado", entidad.EstadoChar ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("CostoTotalProduccion", entidad.CostoTotalProduccion ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("IngresoTotal", entidad.IngresoTotal ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("RentabilidadFinal", entidad.RentabilidadFinal ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaSnapshot", entidad.FechaSnapshot ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Id", entidad.IdHistorial));

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
            string query = "DELETE FROM HISTORIAL WHERE IDHISTORIAL = :Id";

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
            string query = "SELECT * FROM HISTORIAL WHERE IDHISTORIAL = :Id";

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
            string query = "SELECT * FROM HISTORIAL ORDER BY FECHASIEMBRA DESC";

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
    }
}
