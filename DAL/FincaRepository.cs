using ENTITY;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class FincaRepository : ConexionOracle, IRepository<Finca>
    {
        private Finca Mapear(OracleDataReader reader)
        {
            return new Finca
            {
                Id = Convert.ToInt32(reader["IDFINCA"]),
                IdUsuario = Convert.ToInt32(reader["IDUSUARIO"]),
                AreaCalculada = reader["AREACALCULADA"] != DBNull.Value ? Convert.ToDouble(reader["AREACALCULADA"]) : (double?)null,
                Poligono = reader["POLIGONO"] != DBNull.Value ? reader["POLIGONO"].ToString() : null
            };
        }

        public Response<Finca> Insertar(Finca entidad)
        {
            Response<Finca> response = new Response<Finca>();

            string queryId = "SELECT SEQ_FINCA.NEXTVAL FROM DUAL";
            string queryInsert = @"INSERT INTO FINCA (IDFINCA, IDUSUARIO, AREACALCULADA, POLIGONO)
                                   VALUES (:Id, :IdUsuario, :AreaCalculada, :Poligono)";

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
                    command.Parameters.Add(new OracleParameter("AreaCalculada", entidad.AreaCalculada ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Poligono", OracleDbType.Clob)
                    {
                        Value = entidad.Poligono ?? (object)DBNull.Value
                    });

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                entidad.Id = nuevoId;
                response.Estado = true;
                response.Mensaje = "Finca registrada exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al insertar finca: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Finca> Actualizar(Finca entidad)
        {
            Response<Finca> response = new Response<Finca>();
            string query = @"UPDATE FINCA SET 
                             IDUSUARIO = :IdUsuario,
                             AREACALCULADA = :AreaCalculada,
                             POLIGONO = :Poligono
                             WHERE IDFINCA = :Id";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    command.Transaction = transaction;
                    command.Parameters.Add(new OracleParameter("IdUsuario", entidad.IdUsuario));
                    command.Parameters.Add(new OracleParameter("AreaCalculada", entidad.AreaCalculada ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Poligono", OracleDbType.Clob)
                    {
                        Value = entidad.Poligono ?? (object)DBNull.Value
                    });
                    command.Parameters.Add(new OracleParameter("Id", entidad.Id));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                response.Estado = true;
                response.Mensaje = "Finca actualizada exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al actualizar finca: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Finca> Eliminar(int id)
        {
            Response<Finca> response = new Response<Finca>();
            string query = "DELETE FROM FINCA WHERE IDFINCA = :Id";

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
                response.Mensaje = "Finca eliminada exitosamente";
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al eliminar finca: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Finca> ObtenerPorId(int id)
        {
            Response<Finca> response = new Response<Finca>();
            string query = "SELECT * FROM FINCA WHERE IDFINCA = :Id";

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
                            response.Mensaje = "Finca encontrada";
                            response.Entidad = Mapear(reader);
                        }
                        else
                        {
                            response.Estado = false;
                            response.Mensaje = "Finca no encontrada";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener finca: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Finca> ObtenerTodos()
        {
            Response<Finca> response = new Response<Finca>();
            List<Finca> listaFincas = new List<Finca>();
            string query = "SELECT * FROM FINCA ORDER BY IDFINCA";

            try
            {
                AbrirConexion();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaFincas.Add(Mapear(reader));
                        }
                    }
                }

                response.Estado = true;
                response.Mensaje = listaFincas.Count > 0
                    ? $"Se encontraron {listaFincas.Count} fincas"
                    : "No hay fincas registradas";
                response.Lista = listaFincas;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener fincas: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }
    }
}
