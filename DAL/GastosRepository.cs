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
                Id = Convert.ToInt32(reader["IDGASTO"]),
                IdParcela = Convert.ToInt32(reader["IDPARCELA"]),
                Fecha = reader["FECHAGASTOS"] != DBNull.Value ? Convert.ToDateTime(reader["FECHAGASTOS"]) : (DateTime?)null,
                Recurrencia = reader["RECURRENCIA"] != DBNull.Value ? Convert.ToInt32(reader["RECURRENCIA"]) : (int?)null,
                Tipo = reader["TIPO"]?.ToString(),
                Descripcion = reader["DESCRIPCION"]?.ToString(),
                Monto = Convert.ToDecimal(reader["MONTO"])
            };
        }

        public Response<Gastos> Insertar(Gastos entidad)
        {
            Response<Gastos> response = new Response<Gastos>();

            string getId = "SELECT SEQ_GASTO.NEXTVAL FROM DUAL";

            string insert = @"INSERT INTO GASTOS
        (IDGASTO, IDPARCELA, FECHAGASTOS, RECURRENCIA, TIPO, DESCRIPCION, MONTO)
        VALUES (:Id, :IdParcela, :Fecha, :Recurrencia, :Tipo, :Descripcion, :Monto)";

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

                    cmd.Parameters.Add("Id", nuevoId);
                    cmd.Parameters.Add("IdParcela", entidad.IdParcela);
                    cmd.Parameters.Add("Fecha", entidad.Fecha ?? (object)DBNull.Value);
                    cmd.Parameters.Add("Recurrencia", entidad.Recurrencia ?? (object)DBNull.Value);
                    cmd.Parameters.Add("Tipo", entidad.Tipo ?? (object)DBNull.Value);
                    cmd.Parameters.Add(new OracleParameter("Descripcion", OracleDbType.Clob)
                    {
                        Value = entidad.Descripcion ?? (object)DBNull.Value
                    });
                    cmd.Parameters.Add("Monto", entidad.Monto);

                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
                entidad.Id = nuevoId;

                response.Estado = true;
                response.Mensaje = "Gasto registrado correctamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                tx?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error: " + ex.Message;
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
            IDPARCELA = :IdParcela,
            FECHAGASTOS = :Fecha,
            RECURRENCIA = :Recurrencia,
            TIPO = :Tipo,
            DESCRIPCION = :Descripcion,
            MONTO = :Monto
        WHERE IDGASTO = :Id";

            OracleTransaction tx = null;

            try
            {
                AbrirConexion();
                tx = conexion.BeginTransaction();

                using (OracleCommand cmd = new OracleCommand(query, conexion))
                {
                    cmd.Transaction = tx;

                    cmd.Parameters.Add("IdParcela", entidad.IdParcela);
                    cmd.Parameters.Add("Fecha", entidad.Fecha ?? (object)DBNull.Value);
                    cmd.Parameters.Add("Recurrencia", entidad.Recurrencia ?? (object)DBNull.Value);
                    cmd.Parameters.Add("Tipo", entidad.Tipo ?? (object)DBNull.Value);
                    cmd.Parameters.Add(new OracleParameter("Descripcion", OracleDbType.Clob)
                    {
                        Value = entidad.Descripcion ?? (object)DBNull.Value
                    });
                    cmd.Parameters.Add("Monto", entidad.Monto);
                    cmd.Parameters.Add("Id", entidad.Id);

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
                response.Mensaje = "Error: " + ex.Message;
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
            string query = "DELETE FROM GASTOS WHERE IDGASTO = :Id";

            OracleTransaction tx = null;

            try
            {
                AbrirConexion();
                tx = conexion.BeginTransaction();

                using (OracleCommand cmd = new OracleCommand(query, conexion))
                {
                    cmd.Transaction = tx;
                    cmd.Parameters.Add("Id", id);
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
                response.Estado = true;
                response.Mensaje = "Gasto eliminado";
            }
            catch (Exception ex)
            {
                tx?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error: " + ex.Message;
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
            string query = "SELECT * FROM GASTOS WHERE IDGASTO = :Id";

            try
            {
                AbrirConexion();

                using (OracleCommand cmd = new OracleCommand(query, conexion))
                {
                    cmd.Parameters.Add("Id", id);

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
                            response.Mensaje = "No encontrado";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error: " + ex.Message;
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

            string query = "SELECT * FROM GASTOS ORDER BY FECHAGASTOS DESC";

            try
            {
                AbrirConexion();

                using (OracleCommand cmd = new OracleCommand(query, conexion))
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        lista.Add(Mapear(reader));
                }

                response.Estado = true;
                response.Lista = lista;
                response.Mensaje = lista.Count > 0
                    ? $"Se encontraron {lista.Count} gastos"
                    : "No hay registros";
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        //public decimal CalcularCostoTotalInsumos(int idParcela)
        //{
        //    const string query = "SELECT NVL(SUM(UNIDAD * COSTOUNITARIO), 0) FROM INSUMO WHERE ID_PARCELA = :IdParcela";
        //    decimal costoTotal = 0;

        //    try
        //    {
        //        AbrirConexion();

        //        using (OracleCommand command = new OracleCommand(query, conexion))
        //        {
        //            command.Parameters.Add(new OracleParameter("IdParcela", idParcela));
        //            object resultado = command.ExecuteScalar();

        //            if (resultado != null && resultado != DBNull.Value)
        //                costoTotal = Convert.ToDecimal(resultado);
        //        }
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return costoTotal;
        //}

        //public Response<Insumo> ObtenerPorParcela(int idParcela)
        //{
        //    Response<Insumo> response = new Response<Insumo>();
        //    List<Insumo> listaInsumos = new List<Insumo>();
        //    string query = "SELECT * FROM INSUMO WHERE ID_PARCELA = :IdParcela ORDER BY TIPO";

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
        //                    listaInsumos.Add(Mapear(reader));
        //                }
        //            }
        //        }

        //        response.Estado = true;
        //        response.Mensaje = listaInsumos.Count > 0
        //            ? $"Se encontraron {listaInsumos.Count} insumos para esta parcela"
        //            : "Esta parcela no tiene insumos registrados";
        //        response.Lista = listaInsumos;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Estado = false;
        //        response.Mensaje = "Error al obtener insumos de la parcela: " + ex.Message;
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return response;
        //}

        //public Response<Insumo> ObtenerPorTipo(string tipo)
        //{
        //    Response<Insumo> response = new Response<Insumo>();
        //    List<Insumo> listaInsumos = new List<Insumo>();
        //    string query = "SELECT * FROM INSUMO WHERE TIPO = :Tipo ORDER BY COSTOUNITARIO";

        //    try
        //    {
        //        AbrirConexion();

        //        using (OracleCommand command = new OracleCommand(query, conexion))
        //        {
        //            command.Parameters.Add(new OracleParameter("Tipo", tipo));

        //            using (OracleDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    listaInsumos.Add(Mapear(reader));
        //                }
        //            }
        //        }

        //        response.Estado = true;
        //        response.Mensaje = listaInsumos.Count > 0
        //            ? $"Se encontraron {listaInsumos.Count} insumos de tipo '{tipo}'"
        //            : $"No hay insumos de tipo '{tipo}'";
        //        response.Lista = listaInsumos;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Estado = false;
        //        response.Mensaje = "Error al obtener insumos por tipo: " + ex.Message;
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return response;
        //}
    }
}
