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
        private Parcela Mapear(OracleDataReader reader)
        {
            return new Parcela
            {
                IdParcela = Convert.ToInt32(reader["IDPARCELA"]),
                IdUsuario = Convert.ToInt32(reader["IDUSUARIO"]),
                Area = reader["AREA"] != DBNull.Value ? Convert.ToDecimal(reader["AREA"]) : (decimal?)null,
                Nombre = reader["NOMBRE"] != DBNull.Value ? reader["NOMBRE"].ToString() : null,
                PhSuelo = reader["PHSUELO"] != DBNull.Value ? Convert.ToDecimal(reader["PHSUELO"]) : (decimal?)null,
                TipoSuelo = reader["TIPOSUELO"] != DBNull.Value ? reader["TIPOSUELO"].ToString() : null,
                Estado = reader["ESTADO"] != DBNull.Value ? reader["ESTADO"].ToString() : "1"
            };
        }

        public Response<Parcela> Insertar(Parcela entidad)
        {
            Response<Parcela> response = new Response<Parcela>();
            string queryId = "SELECT SEQ_PARCELA.NEXTVAL FROM DUAL";
            string queryInsert = @"INSERT INTO PARCELA (IDPARCELA, IDUSUARIO, AREA, NOMBRE, PHSUELO, TIPOSUELO, ESTADO) 
                                   VALUES (:Id, :IdUsuario, :Area, :Nombre, :PhSuelo, :TipoSuelo, :Estado)";

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
                    command.Parameters.Add(new OracleParameter("Area", entidad.Area ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Nombre", entidad.Nombre ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("PhSuelo", entidad.PhSuelo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("TipoSuelo", entidad.TipoSuelo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Estado", entidad.Estado ?? "1"));

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
                             IDUSUARIO = :IdUsuario,
                             AREA = :Area,
                             NOMBRE = :Nombre,
                             PHSUELO = :PhSuelo,
                             TIPOSUELO = :TipoSuelo,
                             ESTADO = :Estado
                             WHERE IDPARCELA = :Id";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    command.Transaction = transaction;
                    command.Parameters.Add(new OracleParameter("IdUsuario", entidad.IdUsuario));
                    command.Parameters.Add(new OracleParameter("Area", entidad.Area ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Nombre", entidad.Nombre ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("PhSuelo", entidad.PhSuelo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("TipoSuelo", entidad.TipoSuelo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Estado", entidad.Estado ?? "1"));
                    command.Parameters.Add(new OracleParameter("Id", entidad.IdParcela));

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
            string query = "UPDATE PARCELA SET ESTADO = '0' WHERE IDPARCELA = :Id";

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
            string query = "SELECT * FROM PARCELA WHERE IDPARCELA = :Id AND ESTADO = '1'";

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
                            response.Entidad = Mapear(reader);
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
            string query = "SELECT * FROM PARCELA WHERE ESTADO = '1' ORDER BY IDPARCELA";

            try
            {
                AbrirConexion();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaParcelas.Add(Mapear(reader));
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

        public Response<Parcela> ObtenerPorUsuario(int idUsuario)
        {
            Response<Parcela> response = new Response<Parcela>();
            List<Parcela> listaParcelas = new List<Parcela>();
            string query = "SELECT * FROM PARCELA WHERE IDUSUARIO = :IdUsuario AND ESTADO = '1' ORDER BY IDPARCELA";

            try
            {
                AbrirConexion();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    command.Parameters.Add(new OracleParameter("IdUsuario", idUsuario));

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaParcelas.Add(Mapear(reader));
                        }
                    }
                }

                response.Estado = true;
                response.Mensaje = listaParcelas.Count > 0
                    ? $"Se encontraron {listaParcelas.Count} parcelas del usuario"
                    : "El usuario no tiene parcelas registradas";
                response.Lista = listaParcelas;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener parcelas del usuario: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }
    }
}
