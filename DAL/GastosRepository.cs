using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENTITY;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class GastosRepository : ConexionOracle, IRepository<Gastos>
    {
        private Gastos Mapear(OracleDataReader reader)
        {
            return new Gastos
            {
                IdGasto = Convert.ToInt32(reader["IDGASTO"]),
                IdSiembra = Convert.ToInt32(reader["IDSIEMBRA"]),
                FechaGastos = reader["FECHAGASTOS"] != DBNull.Value ? Convert.ToDateTime(reader["FECHAGASTOS"]) : (DateTime?)null,
                Tipo = reader["TIPO"] != DBNull.Value ? reader["TIPO"].ToString() : null,
                Concepto = reader["CONCEPTO"] != DBNull.Value ? reader["CONCEPTO"].ToString() : null,
                Monto = Convert.ToDecimal(reader["MONTO"]),
                Nota = reader["NOTA"] != DBNull.Value ? reader["NOTA"].ToString() : null,
                Estado = reader["ESTADO"] != DBNull.Value ? Convert.ToChar(reader["ESTADO"]) : '1'
            };
        }

        public Response<Gastos> Insertar(Gastos entidad)
        {
            Response<Gastos> response = new Response<Gastos>();
            string getId = "SELECT SEQ_GASTO.NEXTVAL FROM DUAL";
            string insert = @"INSERT INTO GASTOS (IDGASTO, IDSIEMBRA, FECHAGASTOS, TIPO, CONCEPTO, MONTO, NOTA, ESTADO)
                              VALUES (:Id, :IdSiembra, :Fecha, :Tipo, :Concepto, :Monto, :Nota, :Estado)";

            OracleTransaction tx = null;

            try
            {
                AbrirConexion();
                tx = conexion.BeginTransaction();

                int nuevoId;
                using (OracleCommand cmdId = new OracleCommand(getId, conexion))
                {
                    cmdId.Transaction = tx;
                    nuevoId = Convert.ToInt32(cmdId.ExecuteScalar());
                }

                using (OracleCommand cmd = new OracleCommand(insert, conexion))
                {
                    cmd.Transaction = tx;
                    cmd.Parameters.Add(new OracleParameter("Id", nuevoId));
                    cmd.Parameters.Add(new OracleParameter("IdSiembra", entidad.IdSiembra));
                    cmd.Parameters.Add(new OracleParameter("Fecha", entidad.FechaGastos ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("Tipo", entidad.Tipo ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("Concepto", OracleDbType.Clob) { Value = entidad.Concepto ?? (object)DBNull.Value });
                    cmd.Parameters.Add(new OracleParameter("Monto", entidad.Monto));
                    cmd.Parameters.Add(new OracleParameter("Nota", OracleDbType.Clob) { Value = entidad.Nota ?? (object)DBNull.Value });
                    cmd.Parameters.Add(new OracleParameter("Estado", entidad.Estado));

                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
                entidad.IdGasto = nuevoId;
                response.Estado = true;
                response.Mensaje = "Gasto registrado correctamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                tx?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al insertar gasto: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Gastos> Actualizar(Gastos entidad)
        {
            Response<Gastos> response = new Response<Gastos>();
            string query = @"UPDATE GASTOS SET
                             IDSIEMBRA = :IdSiembra,
                             FECHAGASTOS = :Fecha,
                             TIPO = :Tipo,
                             CONCEPTO = :Concepto,
                             MONTO = :Monto,
                             NOTA = :Nota,
                             ESTADO = :Estado
                             WHERE IDGASTO = :Id";

            OracleTransaction tx = null;

            try
            {
                AbrirConexion();
                tx = conexion.BeginTransaction();

                using (OracleCommand cmd = new OracleCommand(query, conexion))
                {
                    cmd.Transaction = tx;
                    cmd.Parameters.Add(new OracleParameter("IdSiembra", entidad.IdSiembra));
                    cmd.Parameters.Add(new OracleParameter("Fecha", entidad.FechaGastos ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("Tipo", entidad.Tipo ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("Concepto", OracleDbType.Clob) { Value = entidad.Concepto ?? (object)DBNull.Value });
                    cmd.Parameters.Add(new OracleParameter("Monto", entidad.Monto));
                    cmd.Parameters.Add(new OracleParameter("Nota", OracleDbType.Clob) { Value = entidad.Nota ?? (object)DBNull.Value });
                    cmd.Parameters.Add(new OracleParameter("Estado", entidad.Estado));
                    cmd.Parameters.Add(new OracleParameter("Id", entidad.IdGasto));

                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
                response.Estado = true;
                response.Mensaje = "Gasto actualizado correctamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                tx?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al actualizar gasto: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Gastos> Eliminar(int id)
        {
            Response<Gastos> response = new Response<Gastos>();
            string query = "UPDATE GASTOS SET ESTADO = '0' WHERE IDGASTO = :Id";

            OracleTransaction tx = null;

            try
            {
                AbrirConexion();
                tx = conexion.BeginTransaction();

                using (OracleCommand cmd = new OracleCommand(query, conexion))
                {
                    cmd.Transaction = tx;
                    cmd.Parameters.Add(new OracleParameter("Id", id));
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
                response.Estado = true;
                response.Mensaje = "Gasto eliminado correctamente";
            }
            catch (Exception ex)
            {
                tx?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al eliminar gasto: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Gastos> ObtenerPorId(int id)
        {
            Response<Gastos> response = new Response<Gastos>();
            string query = "SELECT * FROM GASTOS WHERE IDGASTO = :Id AND ESTADO = '1'";

            try
            {
                AbrirConexion();

                using (OracleCommand cmd = new OracleCommand(query, conexion))
                {
                    cmd.Parameters.Add(new OracleParameter("Id", id));

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            response.Estado = true;
                            response.Entidad = Mapear(reader);
                            response.Mensaje = "Gasto encontrado";
                        }
                        else
                        {
                            response.Estado = false;
                            response.Mensaje = "Gasto no encontrado";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener gasto: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Gastos> ObtenerTodos()
        {
            Response<Gastos> response = new Response<Gastos>();
            List<Gastos> lista = new List<Gastos>();
            string query = "SELECT * FROM GASTOS WHERE ESTADO = '1' ORDER BY FECHAGASTOS DESC";

            try
            {
                AbrirConexion();

                using (OracleCommand cmd = new OracleCommand(query, conexion))
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(Mapear(reader));
                    }
                }

                response.Estado = true;
                response.Lista = lista;
                response.Mensaje = lista.Count > 0
                    ? $"Se encontraron {lista.Count} gastos"
                    : "No hay gastos registrados";
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener gastos: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Gastos> ObtenerPorSiembra(int idSiembra)
        {
            Response<Gastos> response = new Response<Gastos>();
            List<Gastos> lista = new List<Gastos>();
            string query = "SELECT * FROM GASTOS WHERE IDSIEMBRA = :IdSiembra AND ESTADO = '1' ORDER BY FECHAGASTOS DESC";

            try
            {
                AbrirConexion();

                using (OracleCommand cmd = new OracleCommand(query, conexion))
                {
                    cmd.Parameters.Add(new OracleParameter("IdSiembra", idSiembra));

                    using (OracleDataReader reader = cmd.ExecuteReader())
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
                    ? $"Se encontraron {lista.Count} gastos para esta siembra"
                    : "Esta siembra no tiene gastos registrados";
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener gastos de la siembra: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }
    }
}
