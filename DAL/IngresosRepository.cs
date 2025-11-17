using ENTITY;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class IngresosRepository : ConexionOracle, IRepository<Ingresos>
    {
        private Ingresos Mapear(OracleDataReader reader)
        {
            return new Ingresos
            {
                IdIngresos = Convert.ToInt32(reader["IDINGRESOS"]),
                IdSiembra = Convert.ToInt32(reader["IDSIEMBRA"]),
                FechaIngresos = reader["FECHAINGRESOS"] != DBNull.Value ? Convert.ToDateTime(reader["FECHAINGRESOS"]) : (DateTime?)null,
                Tipo = reader["TIPO"] != DBNull.Value ? reader["TIPO"].ToString() : null,
                Concepto = reader["CONCEPTO"] != DBNull.Value ? reader["CONCEPTO"].ToString() : null,
                Monto = reader["MONTO"] != DBNull.Value ? Convert.ToDecimal(reader["MONTO"]) : (decimal?)null,
                Nota = reader["NOTA"] != DBNull.Value ? reader["NOTA"].ToString() : null,
                Estado = reader["ESTADO"] != DBNull.Value ? reader["ESTADO"].ToString() : "1"
            };
        }

        public Response<Ingresos> Insertar(Ingresos entidad)
        {
            Response<Ingresos> response = new Response<Ingresos>();
            string queryId = "SELECT SEQ_INGRESOS.NEXTVAL FROM DUAL";
            string queryInsert = @"INSERT INTO INGRESOS (IDINGRESOS, IDSIEMBRA, FECHAINGRESOS, TIPO, CONCEPTO, MONTO, NOTA, ESTADO)
                                   VALUES (:Id, :IdSiembra, :Fecha, :Tipo, :Concepto, :Monto, :Nota, :Estado)";

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
                    command.Parameters.Add(new OracleParameter("IdSiembra", entidad.IdSiembra));
                    command.Parameters.Add(new OracleParameter("Fecha", entidad.FechaIngresos ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Tipo", entidad.Tipo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Concepto", OracleDbType.Clob) { Value = entidad.Concepto ?? (object)DBNull.Value });
                    command.Parameters.Add(new OracleParameter("Monto", entidad.Monto ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Nota", OracleDbType.Clob) { Value = entidad.Nota ?? (object)DBNull.Value });
                    command.Parameters.Add(new OracleParameter("Estado", entidad.Estado ?? "1"));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                entidad.IdIngresos = nuevoId;
                response.Estado = true;
                response.Mensaje = "Ingreso registrado exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al insertar ingreso: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Ingresos> Actualizar(Ingresos entidad)
        {
            Response<Ingresos> response = new Response<Ingresos>();
            string query = @"UPDATE INGRESOS SET
                             IDSIEMBRA = :IdSiembra,
                             FECHAINGRESOS = :Fecha,
                             TIPO = :Tipo,
                             CONCEPTO = :Concepto,
                             MONTO = :Monto,
                             NOTA = :Nota,
                             ESTADO = :Estado
                             WHERE IDINGRESOS = :Id";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    command.Transaction = transaction;
                    command.Parameters.Add(new OracleParameter("IdSiembra", entidad.IdSiembra));
                    command.Parameters.Add(new OracleParameter("Fecha", entidad.FechaIngresos ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Tipo", entidad.Tipo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Concepto", OracleDbType.Clob) { Value = entidad.Concepto ?? (object)DBNull.Value });
                    command.Parameters.Add(new OracleParameter("Monto", entidad.Monto ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Nota", OracleDbType.Clob) { Value = entidad.Nota ?? (object)DBNull.Value });
                    command.Parameters.Add(new OracleParameter("Estado", entidad.Estado ?? "1"));
                    command.Parameters.Add(new OracleParameter("Id", entidad.IdIngresos));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                response.Estado = true;
                response.Mensaje = "Ingreso actualizado exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al actualizar ingreso: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Ingresos> Eliminar(int id)
        {
            Response<Ingresos> response = new Response<Ingresos>();
            string query = "UPDATE INGRESOS SET ESTADO = '0' WHERE IDINGRESOS = :Id";

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
                response.Mensaje = "Ingreso eliminado exitosamente";
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al eliminar ingreso: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Ingresos> ObtenerPorId(int id)
        {
            Response<Ingresos> response = new Response<Ingresos>();
            string query = "SELECT * FROM INGRESOS WHERE IDINGRESOS = :Id AND ESTADO = '1'";

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
                            response.Mensaje = "Ingreso encontrado";
                            response.Entidad = Mapear(reader);
                        }
                        else
                        {
                            response.Estado = false;
                            response.Mensaje = "Ingreso no encontrado";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener ingreso: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Ingresos> ObtenerTodos()
        {
            Response<Ingresos> response = new Response<Ingresos>();
            List<Ingresos> lista = new List<Ingresos>();
            string query = "SELECT * FROM INGRESOS WHERE ESTADO = '1' ORDER BY FECHAINGRESOS DESC";

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
                response.Lista = lista;
                response.Mensaje = lista.Count > 0
                    ? $"Se encontraron {lista.Count} ingresos"
                    : "No hay ingresos registrados";
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener ingresos: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Ingresos> ObtenerPorSiembra(int idSiembra)
        {
            Response<Ingresos> response = new Response<Ingresos>();
            List<Ingresos> lista = new List<Ingresos>();
            string query = "SELECT * FROM INGRESOS WHERE IDSIEMBRA = :IdSiembra AND ESTADO = '1' ORDER BY FECHAINGRESOS DESC";

            try
            {
                AbrirConexion();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    command.Parameters.Add(new OracleParameter("IdSiembra", idSiembra));

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
                    ? $"Se encontraron {lista.Count} ingresos para esta siembra"
                    : "Esta siembra no tiene ingresos registrados";
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener ingresos de la siembra: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }
    }
}
