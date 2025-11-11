using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENTITY;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class InsumoRepository : ConexionOracle, IRepository<Insumo>
    {
       private Insumo Mapear(OracleDataReader reader)
        {
            return new Insumo
            {
                Id = Convert.ToInt32(reader["ID_INSUMO"]),
                IdParcela = Convert.ToInt32(reader["ID_PARCELA"]),
                Unidad = Convert.ToInt32(reader["UNIDAD"]),
                Tipo = reader["TIPO"] != DBNull.Value ? reader["TIPO"].ToString() : null,
                CostoUnitario = Convert.ToSingle(reader["COSTOUNITARIO"])
            };
        }

        public Response<Insumo> Insertar(Insumo entidad)
        {
            Response<Insumo> response = new Response<Insumo>();

            string queryId = "SELECT SEQ_INSUMO.NEXTVAL FROM DUAL";
            string queryInsert = "INSERT INTO INSUMO (ID_INSUMO, ID_PARCELA, UNIDAD, TIPO, COSTOUNITARIO) " +
                           "VALUES (:Id, :IdParcela, :Unidad, :Tipo, :CostoUnitario)";

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
                    command.Parameters.Add(new OracleParameter("Unidad", entidad.Unidad));
                    command.Parameters.Add(new OracleParameter("Tipo", entidad.Tipo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("CostoUnitario", entidad.CostoUnitario));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                entidad.Id = nuevoId;
                response.Estado = true;
                response.Mensaje = "Insumo registrado exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al insertar insumo: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Insumo> Actualizar(Insumo entidad)
        {
            Response<Insumo> response = new Response<Insumo>();
            string query = "UPDATE INSUMO SET ID_PARCELA = :IdParcela, UNIDAD = :Unidad, " +
                           "TIPO = :Tipo, COSTOUNITARIO = :CostoUnitario " +
                           "WHERE ID_INSUMO = :Id";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    command.Transaction = transaction;
                    command.Parameters.Add(new OracleParameter("IdParcela", entidad.IdParcela));
                    command.Parameters.Add(new OracleParameter("Unidad", entidad.Unidad));
                    command.Parameters.Add(new OracleParameter("Tipo", entidad.Tipo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("CostoUnitario", entidad.CostoUnitario));
                    command.Parameters.Add(new OracleParameter("Id", entidad.Id));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                response.Estado = true;
                response.Mensaje = "Insumo actualizado exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al actualizar insumo: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Insumo> Eliminar(int id)
        {
            Response<Insumo> response = new Response<Insumo>();
            string query = "DELETE FROM INSUMO WHERE ID_INSUMO = :Id";

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
                response.Mensaje = "Insumo eliminado exitosamente";
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al eliminar insumo: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Insumo> ObtenerPorId(int id)
        {
            Response<Insumo> response = new Response<Insumo>();
            string query = "SELECT * FROM INSUMO WHERE ID_INSUMO = :Id";

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
                            response.Mensaje = "Insumo encontrado";
                            response.Entidad = Mapear(reader);
                        }
                        else
                        {
                            response.Estado = false;
                            response.Mensaje = "Insumo no encontrado";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener insumo: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Insumo> ObtenerTodos()
        {
            Response<Insumo> response = new Response<Insumo>();
            List<Insumo> listaInsumos = new List<Insumo>();
            string query = "SELECT * FROM INSUMO ORDER BY TIPO";

            try
            {
                AbrirConexion();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaInsumos.Add(Mapear(reader));
                        }
                    }
                }

                response.Estado = true;
                response.Mensaje = listaInsumos.Count > 0
                    ? $"Se encontraron {listaInsumos.Count} insumos"
                    : "No hay insumos registrados";
                response.Lista = listaInsumos;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener insumos: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

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

        //public Response<Insumo> CalcularCostoTotalParcela(int idParcela)
        //{
        //    Response<Insumo> response = new Response<Insumo>();
        //    string query = "SELECT SUM(UNIDAD * COSTOUNITARIO) AS COSTO_TOTAL FROM INSUMO WHERE ID_PARCELA = :IdParcela";

        //    try
        //    {
        //        AbrirConexion();

        //        using (OracleCommand command = new OracleCommand(query, conexion))
        //        {
        //            command.Parameters.Add(new OracleParameter("IdParcela", idParcela));

        //            object resultado = command.ExecuteScalar();

        //            if (resultado != null && resultado != DBNull.Value)
        //            {
        //                float costoTotal = Convert.ToSingle(resultado);
        //                response.Estado = true;
        //                response.Mensaje = $"Costo total de insumos: ${costoTotal:N2}";
        //            }
        //            else
        //            {
        //                response.Estado = true;
        //                response.Mensaje = "Esta parcela no tiene insumos registrados. Costo total: $0.00";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Estado = false;
        //        response.Mensaje = "Error al calcular costo total: " + ex.Message;
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return response;
        //}
    }
}
