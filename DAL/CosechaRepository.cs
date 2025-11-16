using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENTITY;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class CosechaRepository : ConexionOracle, IRepository<Cosecha>
    {
        public Response<Cosecha> Insertar(Cosecha entidad)
        {
            Response<Cosecha> response = new Response<Cosecha>();

            string queryId = "SELECT SEQ_COSECHA.NEXTVAL FROM DUAL";

            string queryInsert = @"INSERT INTO COSECHA (
                    IDCOSECHA, IDPARCELA,
                    CALIDADCOSECHADA, CANTIDADCOSECHADA, PRECIOVENTAUNITARIO
                ) VALUES (
                    :Id, :IdParcela,
                    :Calidad, :Cantidad, :Precio
                )";

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
                    command.Parameters.Add(new OracleParameter("Calidad", entidad.CalidadCosechada ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Cantidad", entidad.CantidadCosechada ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Precio", entidad.PrecioVentaUnitario ?? (object)DBNull.Value));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();

                entidad.Id = nuevoId;
                response.Estado = true;
                response.Mensaje = "Cosecha registrada exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al insertar cosecha: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }


        public Response<Cosecha> Actualizar(Cosecha entidad)
        {
            Response<Cosecha> response = new Response<Cosecha>();

            string query = @"UPDATE COSECHA SET 
                    IDPARCELA = :IdParcela,
                    CALIDADCOSECHADA = :Calidad,
                    CANTIDADCOSECHADA = :Cantidad,
                    PRECIOVENTAUNITARIO = :Precio
                WHERE IDCOSECHA = :Id";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    command.Transaction = transaction;

                    command.Parameters.Add(new OracleParameter("IdParcela", entidad.IdParcela));
                    command.Parameters.Add(new OracleParameter("Calidad", entidad.CalidadCosechada ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Cantidad", entidad.CantidadCosechada ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Precio", entidad.PrecioVentaUnitario ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Id", entidad.Id));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();

                response.Estado = true;
                response.Mensaje = "Cosecha actualizada exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al actualizar cosecha: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }


        public Response<Cosecha> Eliminar(int id)
        {
            Response<Cosecha> response = new Response<Cosecha>();
            string query = "DELETE FROM COSECHA WHERE IDCOSECHA = :Id";

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
                response.Mensaje = "Cosecha eliminada exitosamente";
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al eliminar cosecha: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }


        public Response<Cosecha> ObtenerPorId(int id)
        {
            Response<Cosecha> response = new Response<Cosecha>();
            string query = "SELECT * FROM COSECHA WHERE IDCOSECHA = :Id";

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
                            response.Mensaje = "Cosecha encontrada";
                            response.Entidad = Mapear(reader);
                        }
                        else
                        {
                            response.Estado = false;
                            response.Mensaje = "Cosecha no encontrada";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener cosecha: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }


        public Response<Cosecha> ObtenerTodos()
        {
            Response<Cosecha> response = new Response<Cosecha>();
            List<Cosecha> lista = new List<Cosecha>();
            string query = "SELECT * FROM COSECHA ORDER BY IDCOSECHA DESC";

            try
            {
                AbrirConexion();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            lista.Add(Mapear(reader));
                    }
                }

                response.Estado = true;
                response.Lista = lista;
                response.Mensaje = lista.Count > 0
                    ? $"Se encontraron {lista.Count} cosechas"
                    : "No hay cosechas registradas";
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener cosechas: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }


        private Cosecha Mapear(OracleDataReader reader)
        {
            return new Cosecha
            {
                Id = Convert.ToInt32(reader["IDCOSECHA"]),
                IdParcela = Convert.ToInt32(reader["IDPARCELA"]),
                CalidadCosechada = reader["CALIDADCOSECHADA"] != DBNull.Value ? Convert.ToInt32(reader["CALIDADCOSECHADA"]) : (int?)null,
                CantidadCosechada = reader["CANTIDADCOSECHADA"] != DBNull.Value ? Convert.ToDecimal(reader["CANTIDADCOSECHADA"]) : (decimal?)null,
                PrecioVentaUnitario = reader["PRECIOVENTAUNITARIO"] != DBNull.Value ? Convert.ToDecimal(reader["PRECIOVENTAUNITARIO"]) : (decimal?)null
            };
        }
    }
}
