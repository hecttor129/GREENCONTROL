using ENTITY;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class HistorialGastosRepository : ConexionOracle, IRepository<HistorialGastos>
    {
        private HistorialGastos Mapear(OracleDataReader reader)
        {
            return new HistorialGastos
            {
                IdHistorialGasto = Convert.ToInt32(reader["IDHISTORIALGASTO"]),
                IdHistorial = Convert.ToInt32(reader["IDHISTORIAL"]),
                FechaGasto = reader["FECHAGASTO"] != DBNull.Value ? Convert.ToDateTime(reader["FECHAGASTO"]) : (DateTime?)null,
                Recurrencia = reader["RECURRENCIA"] != DBNull.Value ? Convert.ToInt32(reader["RECURRENCIA"]) : (int?)null,
                Tipo = reader["TIPO"] != DBNull.Value ? reader["TIPO"].ToString() : null,
                Descripcion = reader["DESCRIPCION"] != DBNull.Value ? reader["DESCRIPCION"].ToString() : null,
                Monto = reader["MONTO"] != DBNull.Value ? Convert.ToDecimal(reader["MONTO"]) : (decimal?)null
            };
        }

        public Response<HistorialGastos> Insertar(HistorialGastos entidad)
        {
            Response<HistorialGastos> response = new Response<HistorialGastos>();

            string queryId = "SELECT SEQ_HISTORIALGASTOS.NEXTVAL FROM DUAL";
            string queryInsert = @"INSERT INTO HISTORIALGASTOS 
                (IDHISTORIALGASTO, IDHISTORIAL, FECHAGASTO, RECURRENCIA, TIPO, DESCRIPCION, MONTO)
                VALUES (:Id, :IdHistorial, :FechaGasto, :Recurrencia, :Tipo, :Descripcion, :Monto)";

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
                    command.Parameters.Add(new OracleParameter("IdHistorial", entidad.IdHistorial));
                    command.Parameters.Add(new OracleParameter("FechaGasto", entidad.FechaGasto ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Recurrencia", entidad.Recurrencia ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Tipo", entidad.Tipo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Descripcion", OracleDbType.Clob)
                    {
                        Value = entidad.Descripcion ?? (object)DBNull.Value
                    });
                    command.Parameters.Add(new OracleParameter("Monto", entidad.Monto ?? (object)DBNull.Value));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                entidad.IdHistorialGasto = nuevoId;
                response.Estado = true;
                response.Mensaje = "Historial de gasto registrado exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al insertar historial de gasto: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<HistorialGastos> Actualizar(HistorialGastos entidad)
        {
            Response<HistorialGastos> response = new Response<HistorialGastos>();
            string query = @"UPDATE HISTORIALGASTOS SET 
                             IDHISTORIAL = :IdHistorial,
                             FECHAGASTO = :FechaGasto,
                             RECURRENCIA = :Recurrencia,
                             TIPO = :Tipo,
                             DESCRIPCION = :Descripcion,
                             MONTO = :Monto
                             WHERE IDHISTORIALGASTO = :Id";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    command.Transaction = transaction;
                    command.Parameters.Add(new OracleParameter("IdHistorial", entidad.IdHistorial));
                    command.Parameters.Add(new OracleParameter("FechaGasto", entidad.FechaGasto ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Recurrencia", entidad.Recurrencia ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Tipo", entidad.Tipo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Descripcion", OracleDbType.Clob)
                    {
                        Value = entidad.Descripcion ?? (object)DBNull.Value
                    });
                    command.Parameters.Add(new OracleParameter("Monto", entidad.Monto ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Id", entidad.IdHistorialGasto));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                response.Estado = true;
                response.Mensaje = "Historial de gasto actualizado exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al actualizar historial de gasto: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<HistorialGastos> Eliminar(int id)
        {
            Response<HistorialGastos> response = new Response<HistorialGastos>();
            string query = "DELETE FROM HISTORIALGASTOS WHERE IDHISTORIALGASTO = :Id";

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
                response.Mensaje = "Historial de gasto eliminado exitosamente";
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al eliminar historial de gasto: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<HistorialGastos> ObtenerPorId(int id)
        {
            Response<HistorialGastos> response = new Response<HistorialGastos>();
            string query = "SELECT * FROM HISTORIALGASTOS WHERE IDHISTORIALGASTO = :Id";

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
                            response.Mensaje = "Historial de gasto encontrado";
                            response.Entidad = Mapear(reader);
                        }
                        else
                        {
                            response.Estado = false;
                            response.Mensaje = "Historial de gasto no encontrado";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener historial de gasto: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<HistorialGastos> ObtenerTodos()
        {
            Response<HistorialGastos> response = new Response<HistorialGastos>();
            List<HistorialGastos> lista = new List<HistorialGastos>();
            string query = "SELECT * FROM HISTORIALGASTOS ORDER BY FECHAGASTO DESC";

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
                    ? $"Se encontraron {lista.Count} registros de historial de gastos"
                    : "No hay registros de historial de gastos";
                response.Lista = lista;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener historial de gastos: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }
    }
}
