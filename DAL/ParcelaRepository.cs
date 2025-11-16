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
                IdParcela = Convert.ToInt32(reader["IDPARCELA"]),
                IdFinca = Convert.ToInt32(reader["IDFINCA"]),
                IdCultivo = Convert.ToInt32(reader["IDCULTIVO"]),
                Nombre = reader["NOMBRE"] != DBNull.Value ? reader["NOMBRE"].ToString() : null,
                AreaCalculada = reader["AREACALCULADA"] != DBNull.Value ? Convert.ToDecimal(reader["AREACALCULADA"]) : (decimal?)null,
                Poligonos = reader["POLIGONOS"] != DBNull.Value ? reader["POLIGONOS"].ToString() : null,
                TipoSuelo = reader["TIPOSUELO"] != DBNull.Value ? reader["TIPOSUELO"].ToString() : null,
                PhSuelo = reader["PHSUELO"] != DBNull.Value ? Convert.ToDecimal(reader["PHSUELO"]) : (decimal?)null
            };
        }

        public Response<Parcela> Insertar(Parcela entidad)
        {
            Response<Parcela> response = new Response<Parcela>();

            string queryId = "SELECT SEQ_PARCELA.NEXTVAL FROM DUAL";
            string queryInsert = @"INSERT INTO PARCELA (IDPARCELA, IDFINCA, IDCULTIVO, NOMBRE, AREACALCULADA, POLIGONOS, TIPOSUELO, PHSUELO) 
                           VALUES (:Id, :IdFinca, :IdCultivo, :Nombre, :AreaCalculada, :Poligonos, :TipoSuelo, :PhSuelo)";

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

                    var parametros = new OracleParameter[]
                    {
                        new OracleParameter("Id", nuevoId),
                        new OracleParameter("IdFinca", entidad.IdFinca),
                        new OracleParameter("IdCultivo", entidad.IdCultivo),
                        new OracleParameter("Nombre", entidad.Nombre ?? (object)DBNull.Value),
                        new OracleParameter("AreaCalculada", entidad.AreaCalculada ?? (object)DBNull.Value),
                        new OracleParameter("Poligonos", OracleDbType.Clob) { Value = entidad.Poligonos ?? (object)DBNull.Value },
                        new OracleParameter("TipoSuelo", OracleDbType.Varchar2) { Value = entidad.TipoSuelo ?? (object)DBNull.Value },
                        new OracleParameter("PhSuelo", OracleDbType.Decimal) { Value = entidad.PhSuelo ?? (object)DBNull.Value }
                    };

                    command.Parameters.AddRange(parametros);
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                entidad.IdParcela = nuevoId;
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
            string query = @"UPDATE PARCELA SET 
                   IDFINCA = :IdFinca,
                   IDCULTIVO = :IdCultivo,
                   NOMBRE = :Nombre,
                   AREACALCULADA = :AreaCalculada,
                   POLIGONOS = :Poligonos,
                   TIPOSUELO = :TipoSuelo,
                   PHSUELO = :PhSuelo
                   WHERE IDPARCELA = :Id";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    command.Transaction = transaction;

                    var parametros = new OracleParameter[]
                    {
                new OracleParameter("IdFinca", entidad.IdFinca),
                new OracleParameter("IdCultivo", entidad.IdCultivo),
                new OracleParameter("Nombre", entidad.Nombre ?? (object)DBNull.Value),
                new OracleParameter("AreaCalculada", entidad.AreaCalculada ?? (object)DBNull.Value),
                new OracleParameter("Poligonos", OracleDbType.Clob) { Value = entidad.Poligonos ?? (object)DBNull.Value },
                new OracleParameter("TipoSuelo", OracleDbType.Varchar2) { Value = entidad.TipoSuelo ?? (object)DBNull.Value },
                new OracleParameter("PhSuelo", OracleDbType.Decimal) { Value = entidad.PhSuelo ?? (object)DBNull.Value },
                new OracleParameter("Id", entidad.IdParcela)
                    };

                    command.Parameters.AddRange(parametros);
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
            string query = "DELETE FROM PARCELA WHERE IDPARCELA = :Id";

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
            string query = "SELECT * FROM PARCELA WHERE IDPARCELA = :Id";

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
            string query = "SELECT * FROM PARCELA ORDER BY IDPARCELA";

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
    }
}
