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
                Id = Convert.ToInt32(reader["ID_SIEMBRA"]),
                IdParcela = Convert.ToInt32(reader["ID_PARCELA"]),
                IdCultivo = Convert.ToInt32(reader["ID_CULTIVO"]),
                Estado = reader["ESTADO"] != DBNull.Value ? reader["ESTADO"].ToString() : null,
                PorcentajeDesarrollo = reader["PORCENTADEDESARROLLO"] != DBNull.Value ? Convert.ToDecimal(reader["PORCENTADEDESARROLLO"]) : (decimal?)null,
                FechaSiembra = Convert.ToDateTime(reader["FECHASIEMBRA"]),
                FechaGerminacion = reader["FECHAGERMINACION"] != DBNull.Value ? Convert.ToDateTime(reader["FECHAGERMINACION"]) : (DateTime?)null,
                FechaFloracion = reader["FECHAFLORACION"] != DBNull.Value ? Convert.ToDateTime(reader["FECHAFLORACION"]) : (DateTime?)null,
                FechaCosechaEstimada = reader["FECHACOSECHAESTIMADA"] != DBNull.Value ? Convert.ToDateTime(reader["FECHACOSECHAESTIMADA"]) : (DateTime?)null,
                FechaCosechaReal = reader["FECHACOSECHAREAL"] != DBNull.Value ? Convert.ToDateTime(reader["FECHACOSECHAREAL"]) : (DateTime?)null
            };
        }

        public Response<Siembra> Insertar(Siembra entidad)
        {
            Response<Siembra> response = new Response<Siembra>();

            string queryId = "SELECT SEQ_SIEMBRA.NEXTVAL FROM DUAL";
            string queryInsert = @"INSERT INTO SIEMBRA 
                                (ID_SIEMBRA, ID_PARCELA, ID_CULTIVO, ESTADO, PORCENTADEDESARROLLO, 
                                 FECHASIEMBRA, FECHAGERMINACION, FECHAFLORACION, 
                                 FECHACOSECHAESTIMADA, FECHACOSECHAREAL)
                                 VALUES (:Id, :IdParcela, :IdCultivo, :Estado, :PorcentajeDesarrollo,
                                         :FechaSiembra, :FechaGerminacion, :FechaFloracion,
                                         :FechaCosechaEstimada, :FechaCosechaReal)";

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
                    command.Parameters.Add(new OracleParameter("Estado", entidad.Estado ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("PorcentajeDesarrollo", entidad.PorcentajeDesarrollo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaSiembra", entidad.FechaSiembra));
                    command.Parameters.Add(new OracleParameter("FechaGerminacion", entidad.FechaGerminacion ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaFloracion", entidad.FechaFloracion ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaCosechaEstimada", entidad.FechaCosechaEstimada ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaCosechaReal", entidad.FechaCosechaReal ?? (object)DBNull.Value));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                entidad.Id = nuevoId;
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
                                ID_PARCELA = :IdParcela, 
                                ID_CULTIVO = :IdCultivo,
                                ESTADO = :Estado,
                                PORCENTADEDESARROLLO = :PorcentajeDesarrollo,
                                FECHASIEMBRA = :FechaSiembra,
                                FECHAGERMINACION = :FechaGerminacion,
                                FECHAFLORACION = :FechaFloracion,
                                FECHACOSECHAESTIMADA = :FechaCosechaEstimada,
                                FECHACOSECHAREAL = :FechaCosechaReal
                             WHERE ID_SIEMBRA = :Id";

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
                    command.Parameters.Add(new OracleParameter("Estado", entidad.Estado ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("PorcentajeDesarrollo", entidad.PorcentajeDesarrollo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaSiembra", entidad.FechaSiembra));
                    command.Parameters.Add(new OracleParameter("FechaGerminacion", entidad.FechaGerminacion ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaFloracion", entidad.FechaFloracion ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaCosechaEstimada", entidad.FechaCosechaEstimada ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("FechaCosechaReal", entidad.FechaCosechaReal ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Id", entidad.Id));

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
            string query = "DELETE FROM SIEMBRA WHERE ID_SIEMBRA = :Id";

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
            string query = "SELECT * FROM SIEMBRA WHERE ID_SIEMBRA = :Id";

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

        ////public Response<Siembra> BuscarPorEstado(string estado)
        ////{
        ////    Response<Siembra> response = new Response<Siembra>();
        ////    List<Siembra> listaSiembras = new List<Siembra>();
        ////    string query = "SELECT * FROM SIEMBRA WHERE ESTADO = :Estado ORDER BY FECHASIEMBRA";

        ////    try
        ////    {
        ////        AbrirConexion();

        ////        using (OracleCommand command = new OracleCommand(query, conexion))
        ////        {
        ////            command.Parameters.Add(new OracleParameter("Estado", estado));

        ////            using (OracleDataReader reader = command.ExecuteReader())
        ////            {
        ////                while (reader.Read())
        ////                {
        ////                    listaSiembras.Add(Mapear(reader));
        ////                }
        ////            }
        ////        }

        ////        if (listaSiembras.Count > 0)
        ////        {
        ////            response.Estado = true;
        ////            response.Mensaje = $"Se encontraron {listaSiembras.Count} siembras con estado '{estado}'.";
        ////            response.Lista = listaSiembras;
        ////        }
        ////        else
        ////        {
        ////            response.Estado = false;
        ////            response.Mensaje = $"No se encontraron siembras con estado '{estado}'.";
        ////        }
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        response.Estado = false;
        ////        response.Mensaje = "Error al buscar siembras por estado: " + ex.Message;
        ////    }
        ////    finally
        ////    {
        ////        CerrarConexion();
        ////    }

        ////    return response;
        ////}

    }
 }


