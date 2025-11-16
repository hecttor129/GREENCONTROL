using ENTITY;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class SiembraRepository : ConexionOracle, IRepository<Siembra>
    {
        private Siembra Mapear(OracleDataReader reader)
        {
            return new Siembra
            {
                IdSiembra = Convert.ToInt32(reader["IDSIEMBRA"]),
                IdParcela = Convert.ToInt32(reader["IDPARCELA"]),
                IdCultivo = Convert.ToInt32(reader["IDCULTIVO"]),
                EstadoChar = reader["ESTADO"] != DBNull.Value ? reader["ESTADO"].ToString() : "1",
                FechaSiembra = Convert.ToDateTime(reader["FECHASIEMBRA"]),
                PorcentajeDesarrollo = reader["PORCENTAJEDESARROLLO"] != DBNull.Value ? Convert.ToDecimal(reader["PORCENTAJEDESARROLLO"]) : (decimal?)null,
                FechaGerminacion = reader["FECHAGERMINACION"] != DBNull.Value ? Convert.ToDateTime(reader["FECHAGERMINACION"]) : (DateTime?)null,
                FechaFloracion = reader["FECHAFLORACION"] != DBNull.Value ? Convert.ToDateTime(reader["FECHAFLORACION"]) : (DateTime?)null,
                FechaCosecha = reader["FECHACOSECHA"] != DBNull.Value ? Convert.ToDateTime(reader["FECHACOSECHA"]) : (DateTime?)null,
                GerminacionConfirmadaChar = reader["GERMINACIONCONFIRMADA"] != DBNull.Value ? reader["GERMINACIONCONFIRMADA"].ToString() : "0",
                FloracionConfirmadaChar = reader["FLORACIONCONFIRMADA"] != DBNull.Value ? reader["FLORACIONCONFIRMADA"].ToString() : "0",
                CosechaConfirmadaChar = reader["COSECHACONFIRMADA"] != DBNull.Value ? reader["COSECHACONFIRMADA"].ToString() : "0"
            };
        }

        public Response<Siembra> Insertar(Siembra entidad)
        {
            Response<Siembra> response = new Response<Siembra>();

            string queryId = "SELECT SEQ_SIEMBRA.NEXTVAL FROM DUAL";
            string queryInsert = @"INSERT INTO SIEMBRA 
                (IDSIEMBRA, IDPARCELA, IDCULTIVO, ESTADO, PORCENTAJEDESARROLLO, 
                 FECHASIEMBRA, FECHAGERMINACION, FECHAFLORACION, FECHACOSECHA,
                 GERMINACIONCONFIRMADA, FLORACIONCONFIRMADA, COSECHACONFIRMADA)
                VALUES (:Id, :IdParcela, :IdCultivo, :Estado, :PorcentajeDesarrollo,
                        :FechaSiembra, :FechaGerminacion, :FechaFloracion, :FechaCosecha,
                        :GerminacionConfirmada, :FloracionConfirmada, :CosechaConfirmada)";

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
                    command.Parameters.Add(new OracleParameter("IdCultivo", entidad.IdCultivo));
                    command.Parameters.Add(new OracleParameter("Estado", entidad.EstadoChar ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("PorcentajeDesarrollo", entidad.PorcentajeDesarrollo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaSiembra", entidad.FechaSiembra));
                    command.Parameters.Add(new OracleParameter("FechaGerminacion", entidad.FechaGerminacion ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaFloracion", entidad.FechaFloracion ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaCosecha", entidad.FechaCosecha ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("GerminacionConfirmada", entidad.GerminacionConfirmadaChar ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FloracionConfirmada", entidad.FloracionConfirmadaChar ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("CosechaConfirmada", entidad.CosechaConfirmadaChar ?? (object)DBNull.Value));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                entidad.IdSiembra = nuevoId;
                response.Estado = true;
                response.Mensaje = "Siembra registrada exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al insertar siembra: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Siembra> Actualizar(Siembra entidad)
        {
            Response<Siembra> response = new Response<Siembra>();
            string query = @"UPDATE SIEMBRA SET 
                                IDPARCELA = :IdParcela, 
                                IDCULTIVO = :IdCultivo,
                                ESTADO = :Estado,
                                PORCENTAJEDESARROLLO = :PorcentajeDesarrollo,
                                FECHASIEMBRA = :FechaSiembra,
                                FECHAGERMINACION = :FechaGerminacion,
                                FECHAFLORACION = :FechaFloracion,
                                FECHACOSECHA = :FechaCosecha,
                                GERMINACIONCONFIRMADA = :GerminacionConfirmada,
                                FLORACIONCONFIRMADA = :FloracionConfirmada,
                                COSECHACONFIRMADA = :CosechaConfirmada
                             WHERE IDSIEMBRA = :Id";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    command.Transaction = transaction;
                    command.Parameters.Add(new OracleParameter("IdParcela", entidad.IdParcela));
                    command.Parameters.Add(new OracleParameter("IdCultivo", entidad.IdCultivo));
                    command.Parameters.Add(new OracleParameter("Estado", entidad.EstadoChar ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("PorcentajeDesarrollo", entidad.PorcentajeDesarrollo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaSiembra", entidad.FechaSiembra));
                    command.Parameters.Add(new OracleParameter("FechaGerminacion", entidad.FechaGerminacion ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaFloracion", entidad.FechaFloracion ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaCosecha", entidad.FechaCosecha ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("GerminacionConfirmada", entidad.GerminacionConfirmadaChar ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FloracionConfirmada", entidad.FloracionConfirmadaChar ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("CosechaConfirmada", entidad.CosechaConfirmadaChar ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Id", entidad.IdSiembra));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                response.Estado = true;
                response.Mensaje = "Siembra actualizada exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al actualizar siembra: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Siembra> Eliminar(int id)
        {
            Response<Siembra> response = new Response<Siembra>();
            string query = "DELETE FROM SIEMBRA WHERE IDSIEMBRA = :Id";

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
                response.Mensaje = "Siembra eliminada exitosamente";
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al eliminar siembra: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Siembra> ObtenerPorId(int id)
        {
            Response<Siembra> response = new Response<Siembra>();
            string query = "SELECT * FROM SIEMBRA WHERE IDSIEMBRA = :Id";

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
                            response.Mensaje = "Siembra encontrada";
                            response.Entidad = Mapear(reader);
                        }
                        else
                        {
                            response.Estado = false;
                            response.Mensaje = "Siembra no encontrada";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener siembra: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Siembra> ObtenerTodos()
        {
            Response<Siembra> response = new Response<Siembra>();
            List<Siembra> listaSiembras = new List<Siembra>();
            string query = "SELECT * FROM SIEMBRA ORDER BY FECHASIEMBRA DESC";

            try
            {
                AbrirConexion();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaSiembras.Add(Mapear(reader));
                        }
                    }
                }

                response.Estado = true;
                response.Mensaje = listaSiembras.Count > 0
                    ? $"Se encontraron {listaSiembras.Count} siembras"
                    : "No hay siembras registradas";
                response.Lista = listaSiembras;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener siembras: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Siembra> ObtenerPorParcela(int idParcela)
        {
            Response<Siembra> response = new Response<Siembra>();
            string query = "SELECT * FROM SIEMBRA WHERE IDPARCELA = :IdParcela";

            try
            {
                AbrirConexion();

                using (OracleCommand cmd = new OracleCommand(query, conexion))
                {
                    cmd.Parameters.Add(new OracleParameter("IdParcela", idParcela));

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            response.Estado = true;
                            response.Entidad = Mapear(reader);
                            response.Mensaje = "Siembra encontrada para la parcela.";
                        }
                        else
                        {
                            response.Estado = false;
                            response.Mensaje = "No hay Siembra registrada para esta parcela.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al consultar Siembra por parcela: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }



    }
}







 


