using ENTITY;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class SiembraRepository : ConexionOracle, IRepository<Siembra>
    {
        private Siembra Mapear(OracleDataReader reader)
        {
            return new Siembra
            {
                IdSiembra = Convert.ToInt32(reader["IDSIEMBRA"]),
                IdParcela = Convert.ToInt32(reader["IDPARCELA"]),
                IdCultivo = Convert.ToInt32(reader["IDCULTIVO"]),
                FechaSiembra = Convert.ToDateTime(reader["FECHASIEMBRA"]),
                FechaCosecha = reader["FECHACOSECHA"] != DBNull.Value ? Convert.ToDateTime(reader["FECHACOSECHA"]) : (DateTime?)null,
                Estado = reader["ESTADO"] != DBNull.Value ? reader["ESTADO"].ToString() : "1"
            };
        }

        public Response<Siembra> Insertar(Siembra entidad)
        {
            Response<Siembra> response = new Response<Siembra>();
            string queryId = "SELECT SEQ_SIEMBRA.NEXTVAL FROM DUAL";
            string queryInsert = @"INSERT INTO SIEMBRA (IDSIEMBRA, IDPARCELA, IDCULTIVO, FECHASIEMBRA, FECHACOSECHA, ESTADO)
                                   VALUES (:Id, :IdParcela, :IdCultivo, :FechaSiembra, :FechaCosecha, :Estado)";

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
                    command.Parameters.Add(new OracleParameter("IdCultivo", entidad.IdCultivo));
                    command.Parameters.Add(new OracleParameter("FechaSiembra", entidad.FechaSiembra));
                    command.Parameters.Add(new OracleParameter("FechaCosecha", entidad.FechaCosecha ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Estado", entidad.Estado ?? "1"));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                entidad.IdSiembra = nuevoId;
                response.Estado = true;
                response.Mensaje = "Siembra registrada exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al insertar siembra: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Siembra> Actualizar(Siembra entidad)
        {
            Response<Siembra> response = new Response<Siembra>();
            string query = @"UPDATE SIEMBRA SET 
                             IDPARCELA = :IdParcela, 
                             IDCULTIVO = :IdCultivo,
                             FECHASIEMBRA = :FechaSiembra,
                             FECHACOSECHA = :FechaCosecha,
                             ESTADO = :Estado
                             WHERE IDSIEMBRA = :Id";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    command.Transaction = transaction;
                    command.Parameters.Add(new OracleParameter("IdParcela", entidad.IdParcela));
                    command.Parameters.Add(new OracleParameter("IdCultivo", entidad.IdCultivo));
                    command.Parameters.Add(new OracleParameter("FechaSiembra", entidad.FechaSiembra));
                    command.Parameters.Add(new OracleParameter("FechaCosecha", entidad.FechaCosecha ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Estado", entidad.Estado ?? "1"));
                    command.Parameters.Add(new OracleParameter("Id", entidad.IdSiembra));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                response.Estado = true;
                response.Mensaje = "Siembra actualizada exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al actualizar siembra: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Siembra> Eliminar(int id)
        {
            Response<Siembra> response = new Response<Siembra>();
            string query = "UPDATE SIEMBRA SET ESTADO = '0' WHERE IDSIEMBRA = :Id";

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
                response.Mensaje = "Siembra eliminada exitosamente";
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al eliminar siembra: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Siembra> ObtenerPorId(int id)
        {
            Response<Siembra> response = new Response<Siembra>();
            string query = "SELECT * FROM SIEMBRA WHERE IDSIEMBRA = :Id AND ESTADO = '1'";

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
                            response.Mensaje = "Siembra encontrada";
                            response.Entidad = Mapear(reader);
                        }
                        else
                        {
                            response.Estado = false;
                            response.Mensaje = "Siembra no encontrada";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener siembra: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Siembra> ObtenerTodos()
        {
            Response<Siembra> response = new Response<Siembra>();
            List<Siembra> listaSiembras = new List<Siembra>();
            string query = "SELECT * FROM SIEMBRA WHERE ESTADO = '1' ORDER BY FECHASIEMBRA DESC";

            try
            {
                AbrirConexion();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaSiembras.Add(Mapear(reader));
                        }
                    }
                }

                response.Estado = true;
                response.Mensaje = listaSiembras.Count > 0
                    ? $"Se encontraron {listaSiembras.Count} siembras"
                    : "No hay siembras registradas";
                response.Lista = listaSiembras;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener siembras: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Siembra> ObtenerPorParcela(int idParcela)
        {
            Response<Siembra> response = new Response<Siembra>();
            List<Siembra> listaSiembras = new List<Siembra>();
            string query = "SELECT * FROM SIEMBRA WHERE IDPARCELA = :IdParcela AND ESTADO = '1' ORDER BY FECHASIEMBRA DESC";

            try
            {
                AbrirConexion();

                using (OracleCommand cmd = new OracleCommand(query, conexion))
                {
                    cmd.Parameters.Add(new OracleParameter("IdParcela", idParcela));

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaSiembras.Add(Mapear(reader));
                        }
                    }
                }

                response.Estado = true;
                response.Lista = listaSiembras;
                response.Mensaje = listaSiembras.Count > 0
                    ? $"Se encontraron {listaSiembras.Count} siembras para la parcela"
                    : "No hay siembras registradas para esta parcela";
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al consultar siembras por parcela: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }
    }
}







 


