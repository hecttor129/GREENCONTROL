using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENTITY;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class RentabilidadRepository : ConexionOracle, IRepository<Rentabilidad>
    {
        private Rentabilidad Mapear(OracleDataReader reader)
        {
            return new Rentabilidad
            {
                Id = Convert.ToInt32(reader["ID_RENTABILIDAD"]),
                IdParcela = Convert.ToInt32(reader["ID_PARCELA"]),
                PrecioVentaUnitario = reader["PRECIOVENTAUNITARIO"] != DBNull.Value ? Convert.ToDecimal(reader["PRECIOVENTAUNITARIO"]) : (decimal?)null,
                CostoTotalProduccion = reader["COSTOTOTALPRODUCCION"] != DBNull.Value ? Convert.ToDecimal(reader["COSTOTOTALPRODUCCION"]) : (decimal?)null,
                CantidadCosechada = reader["CANTIDADCOSECHADA"] != DBNull.Value ? Convert.ToDecimal(reader["CANTIDADCOSECHADA"]) : (decimal?)null,
                IngresoTotal = reader["INGRESOTOTAL"] != DBNull.Value ? Convert.ToDecimal(reader["INGRESOTOTAL"]) : (decimal?)null,
                RentabilidadPorcentual = reader["RENTABILIDADPORCENTUAL"] != DBNull.Value ? Convert.ToDecimal(reader["RENTABILIDADPORCENTUAL"]) : (decimal?)null,
                CalidadCosechada = reader["CALIDADCOSECHADA"] != DBNull.Value ? reader["CALIDADCOSECHADA"].ToString() : null
            };
        }

        // 🟢 INSERTAR
        public Response<Rentabilidad> Insertar(Rentabilidad entidad)
        {
            Response<Rentabilidad> response = new Response<Rentabilidad>();
            string queryId = "SELECT SEQ_RENTABILIDAD.NEXTVAL FROM DUAL";
            string queryInsert = @"INSERT INTO RENTABILIDAD 
                (ID_RENTABILIDAD, ID_PARCELA, PRECIOVENTAUNITARIO, COSTOTOTALPRODUCCION, 
                CANTIDADCOSECHADA, INGRESOTOTAL, RENTABILIDADPORCENTUAL, CALIDADCOSECHADA)
                VALUES (:Id, :IdParcela, :PrecioVentaUnitario, :CostoTotalProduccion, 
                :CantidadCosechada, :IngresoTotal, :RentabilidadPorcentual, :CalidadCosechada)";

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
                    cmd.Parameters.Add(new OracleParameter("PrecioVentaUnitario", entidad.PrecioVentaUnitario ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("CostoTotalProduccion", entidad.CostoTotalProduccion ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("CantidadCosechada", entidad.CantidadCosechada ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("IngresoTotal", entidad.IngresoTotal ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("RentabilidadPorcentual", entidad.RentabilidadPorcentual ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("CalidadCosechada", entidad.CalidadCosechada ?? (object)DBNull.Value));

                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
                entidad.Id = nuevoId;
                response.Estado = true;
                response.Mensaje = "Rentabilidad registrada exitosamente.";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al insertar rentabilidad: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        // 🟡 ACTUALIZAR
        public Response<Rentabilidad> Actualizar(Rentabilidad entidad)
        {
            Response<Rentabilidad> response = new Response<Rentabilidad>();
            string query = @"UPDATE RENTABILIDAD SET 
                ID_PARCELA = :IdParcela,
                PRECIOVENTAUNITARIO = :PrecioVentaUnitario,
                COSTOTOTALPRODUCCION = :CostoTotalProduccion,
                CANTIDADCOSECHADA = :CantidadCosechada,
                INGRESOTOTAL = :IngresoTotal,
                RENTABILIDADPORCENTUAL = :RentabilidadPorcentual,
                CALIDADCOSECHADA = :CalidadCosechada
                WHERE ID_RENTABILIDAD = :Id";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                using (OracleCommand cmd = new OracleCommand(query, conexion))
                {
                    cmd.Transaction = transaction;
                    cmd.Parameters.Add(new OracleParameter("IdParcela", entidad.IdParcela));
                    cmd.Parameters.Add(new OracleParameter("PrecioVentaUnitario", entidad.PrecioVentaUnitario ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("CostoTotalProduccion", entidad.CostoTotalProduccion ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("CantidadCosechada", entidad.CantidadCosechada ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("IngresoTotal", entidad.IngresoTotal ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("RentabilidadPorcentual", entidad.RentabilidadPorcentual ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("CalidadCosechada", entidad.CalidadCosechada ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("Id", entidad.Id));

                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
                response.Estado = true;
                response.Mensaje = "Rentabilidad actualizada correctamente.";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al actualizar rentabilidad: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        // 🔴 ELIMINAR
        public Response<Rentabilidad> Eliminar(int id)
        {
            Response<Rentabilidad> response = new Response<Rentabilidad>();
            string query = "DELETE FROM RENTABILIDAD WHERE ID_RENTABILIDAD = :Id";

            try
            {
                AbrirConexion();

                using (OracleCommand cmd = new OracleCommand(query, conexion))
                {
                    cmd.Parameters.Add(new OracleParameter("Id", id));
                    int filas = cmd.ExecuteNonQuery();

                    response.Estado = filas > 0;
                    response.Mensaje = filas > 0 ? "Rentabilidad eliminada correctamente." : "No se encontró la rentabilidad.";
                }
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al eliminar rentabilidad: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        // 🔍 OBTENER POR ID
        public Response<Rentabilidad> ObtenerPorId(int id)
        {
            Response<Rentabilidad> response = new Response<Rentabilidad>();
            string query = "SELECT * FROM RENTABILIDAD WHERE ID_RENTABILIDAD = :Id";

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
                            response.Mensaje = "Rentabilidad encontrada.";
                        }
                        else
                        {
                            response.Estado = false;
                            response.Mensaje = "No se encontró la rentabilidad.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener rentabilidad: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        // 📋 OBTENER TODOS
        public Response<Rentabilidad> ObtenerTodos()
        {
            Response<Rentabilidad> response = new Response<Rentabilidad>();
            List<Rentabilidad> lista = new List<Rentabilidad>();
            string query = "SELECT * FROM RENTABILIDAD ORDER BY ID_RENTABILIDAD";

            try
            {
                AbrirConexion();

                using (OracleCommand cmd = new OracleCommand(query, conexion))
                {
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
                response.Mensaje = lista.Count > 0 ? $"Se encontraron {lista.Count} registros." : "No hay registros de rentabilidad.";
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener rentabilidades: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        // 📈 CALCULAR RENTABILIDAD POR PARCELA
        //public Response<Rentabilidad> CalcularRentabilidadPorParcela(int idParcela)
        //{
        //    Response<Rentabilidad> response = new Response<Rentabilidad>();
        //    string query = @"SELECT ID_RENTABILIDAD, 
        //                            (INGRESOTOTAL - COSTOTOTALPRODUCCION) / COSTOTOTALPRODUCCION * 100 AS RENTABILIDAD_CALCULADA
        //                     FROM RENTABILIDAD WHERE ID_PARCELA = :IdParcela";

        //    try
        //    {
        //        AbrirConexion();

        //        using (OracleCommand cmd = new OracleCommand(query, conexion))
        //        {
        //            cmd.Parameters.Add(new OracleParameter("IdParcela", idParcela));

        //            using (OracleDataReader reader = cmd.ExecuteReader())
        //            {
        //                if (reader.Read())
        //                {
        //                    decimal rentabilidad = reader["RENTABILIDAD_CALCULADA"] != DBNull.Value
        //                        ? Convert.ToDecimal(reader["RENTABILIDAD_CALCULADA"])
        //                        : 0;

        //                    response.Estado = true;
        //                    response.Mensaje = $"Rentabilidad calculada: {rentabilidad:N2}%";
        //                }
        //                else
        //                {
        //                    response.Estado = false;
        //                    response.Mensaje = "No se encontró registro de rentabilidad para esta parcela.";
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Estado = false;
        //        response.Mensaje = "Error al calcular rentabilidad: " + ex.Message;
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return response;
        //}
    }
}
