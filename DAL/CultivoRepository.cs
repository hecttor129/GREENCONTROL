using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENTITY;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class CultivoRepository : ConexionOracle, IRepository<Cultivo>
    {
        private Cultivo Mapear(OracleDataReader reader)
        {
            return new Cultivo
            {
                IdCultivo = Convert.ToInt32(reader["IDCULTIVO"]),
                Nombre = reader["NOMBRE"].ToString(),
                Variedad = reader["VARIEDAD"] != DBNull.Value ? reader["VARIEDAD"].ToString() : null,
                DuracionCiclo = reader["DURACIONCICLO"] != DBNull.Value ? Convert.ToInt32(reader["DURACIONCICLO"]) : (int?)null,
                FechaSiembra = Convert.ToDateTime(reader["FECHASIEMBRA"]),
                FechaGerminacion = reader["FECHAGERMINACION"] != DBNull.Value ? Convert.ToDateTime(reader["FECHAGERMINACION"]) : (DateTime?)null,
                FechaFloracion = reader["FECHAFLORACION"] != DBNull.Value ? Convert.ToDateTime(reader["FECHAFLORACION"]) : (DateTime?)null,
                FechaCosecha = reader["FECHACOSECHA"] != DBNull.Value ? Convert.ToDateTime(reader["FECHACOSECHA"]) : (DateTime?)null,
                EstadoChar = reader["ESTADO"] != DBNull.Value ? reader["ESTADO"].ToString() : "1"
            };
        }

        public Response<Cultivo> Insertar(Cultivo entidad)
        {
            Response<Cultivo> response = new Response<Cultivo>();
            string queryId = "SELECT SEQ_CULTIVO.NEXTVAL FROM DUAL";
            string queryInsert = @"INSERT INTO CULTIVO (IDCULTIVO, NOMBRE, VARIEDAD, DURACIONCICLO, FECHASIEMBRA, 
                                   FECHAGERMINACION, FECHAFLORACION, FECHACOSECHA, ESTADO)
                                   VALUES (:Id, :Nombre, :Variedad, :DuracionCiclo, :FechaSiembra, 
                                   :FechaGerminacion, :FechaFloracion, :FechaCosecha, :Estado)";

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
                    command.Parameters.Add(new OracleParameter("Nombre", entidad.Nombre));
                    command.Parameters.Add(new OracleParameter("Variedad", entidad.Variedad ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("DuracionCiclo", entidad.DuracionCiclo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaSiembra", entidad.FechaSiembra));
                    command.Parameters.Add(new OracleParameter("FechaGerminacion", entidad.FechaGerminacion ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaFloracion", entidad.FechaFloracion ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaCosecha", entidad.FechaCosecha ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Estado", entidad.EstadoChar ?? "1"));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                entidad.IdCultivo = nuevoId;
                response.Estado = true;
                response.Mensaje = "Cultivo guardado correctamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al guardar cultivo: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Cultivo> Actualizar(Cultivo entidad)
        {
            Response<Cultivo> response = new Response<Cultivo>();
            string query = @"UPDATE CULTIVO SET
                             NOMBRE = :Nombre,
                             VARIEDAD = :Variedad,
                             DURACIONCICLO = :DuracionCiclo,
                             FECHASIEMBRA = :FechaSiembra,
                             FECHAGERMINACION = :FechaGerminacion,
                             FECHAFLORACION = :FechaFloracion,
                             FECHACOSECHA = :FechaCosecha,
                             ESTADO = :Estado
                             WHERE IDCULTIVO = :Id";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    command.Transaction = transaction;
                    command.Parameters.Add(new OracleParameter("Nombre", entidad.Nombre));
                    command.Parameters.Add(new OracleParameter("Variedad", entidad.Variedad ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("DuracionCiclo", entidad.DuracionCiclo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaSiembra", entidad.FechaSiembra));
                    command.Parameters.Add(new OracleParameter("FechaGerminacion", entidad.FechaGerminacion ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaFloracion", entidad.FechaFloracion ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaCosecha", entidad.FechaCosecha ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Estado", entidad.EstadoChar ?? "1"));
                    command.Parameters.Add(new OracleParameter("Id", entidad.IdCultivo));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                response.Estado = true;
                response.Mensaje = "Cultivo actualizado correctamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al actualizar cultivo: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Cultivo> Eliminar(int id)
        {
            Response<Cultivo> response = new Response<Cultivo>();
            string query = "UPDATE CULTIVO SET ESTADO = '0' WHERE IDCULTIVO = :Id";

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
                response.Mensaje = "Cultivo eliminado correctamente";
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al eliminar cultivo: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Cultivo> ObtenerPorId(int id)
        {
            Response<Cultivo> response = new Response<Cultivo>();
            string query = "SELECT * FROM CULTIVO WHERE IDCULTIVO = :Id";

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
                            response.Entidad = Mapear(reader);
                            response.Mensaje = "Cultivo encontrado";
                        }
                        else
                        {
                            response.Estado = false;
                            response.Mensaje = "Cultivo no encontrado";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener cultivo: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Cultivo> ObtenerTodos()
        {
            Response<Cultivo> response = new Response<Cultivo>();
            List<Cultivo> lista = new List<Cultivo>();
            string query = "SELECT * FROM CULTIVO WHERE ESTADO = '1' ORDER BY IDCULTIVO";

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
                    ? $"Se encontraron {lista.Count} cultivos"
                    : "No hay cultivos registrados";
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener cultivos: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }
    }
}

