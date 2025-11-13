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
            string queryInsert = @"
                INSERT INTO COSECHA (ID_COSECHA, ID_SIEMBRA, ESTADO, CALIDAD, CANTIDAD)
                VALUES (:Id, :IdSiembra, :Estado, :Calidad, :Cantidad)";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                // Obtener el siguiente ID
                int nuevoId;
                using (OracleCommand commandId = new OracleCommand(queryId, conexion))
                {
                    commandId.Transaction = transaction;
                    nuevoId = Convert.ToInt32(commandId.ExecuteScalar());
                }

                // Insertar con el ID obtenido
                using (OracleCommand command = new OracleCommand(queryInsert, conexion))
                {
                    command.Transaction = transaction;
                    command.Parameters.Add(new OracleParameter("Id", nuevoId));
                    command.Parameters.Add(new OracleParameter("IdSiembra", entidad.IdSiembra));
                    command.Parameters.Add(new OracleParameter("Estado", entidad.Estado ?? "1"));
                    command.Parameters.Add(new OracleParameter("Calidad",
                        string.IsNullOrEmpty(entidad.Calidad) ? (object)DBNull.Value : entidad.Calidad));
                    command.Parameters.Add(new OracleParameter("Cantidad",
                        entidad.Cantidad.HasValue ? (object)entidad.Cantidad.Value : DBNull.Value));

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

            string query = @"
                UPDATE COSECHA SET 
                    ID_SIEMBRA = :IdSiembra,
                    ESTADO = :Estado,
                    CALIDAD = :Calidad,
                    CANTIDAD = :Cantidad
                WHERE ID_COSECHA = :Id";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    command.Transaction = transaction;
                    command.Parameters.Add(new OracleParameter("IdSiembra", entidad.IdSiembra));
                    command.Parameters.Add(new OracleParameter("Estado", entidad.Estado ?? "1"));
                    command.Parameters.Add(new OracleParameter("Calidad",
                        string.IsNullOrEmpty(entidad.Calidad) ? (object)DBNull.Value : entidad.Calidad));
                    command.Parameters.Add(new OracleParameter("Cantidad",
                        entidad.Cantidad.HasValue ? (object)entidad.Cantidad.Value : DBNull.Value));
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
            string query = "DELETE FROM COSECHA WHERE ID_COSECHA = :Id";

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
            string query = "SELECT * FROM COSECHA WHERE ID_COSECHA = :Id";

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
            string query = "SELECT * FROM COSECHA ORDER BY ID_COSECHA DESC";

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
                Id = Convert.ToInt32(reader["ID_COSECHA"]),
                IdSiembra = Convert.ToInt32(reader["ID_SIEMBRA"]),
                Estado = reader["ESTADO"] != DBNull.Value ? reader["ESTADO"].ToString() : "1",
                Calidad = reader["CALIDAD"] != DBNull.Value ? reader["CALIDAD"].ToString() : null,
                Cantidad = reader["CANTIDAD"] != DBNull.Value ? Convert.ToDecimal(reader["CANTIDAD"]) : (decimal?)null
            };
        }
    }
}
