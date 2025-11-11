using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENTITY;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class SimuladorRiegoRepository : ConexionOracle, IRepository<SimuladorRiego>
    {

        private SimuladorRiego Mapear(OracleDataReader reader)
        {
            return new SimuladorRiego
            {
                Id = Convert.ToInt32(reader["ID_RIEGO"]),
                IdParcela = Convert.ToInt32(reader["ID_PARCELA"]),
                EstadoRiego = reader["ESTADORIEGO"] != DBNull.Value ? reader["ESTADORIEGO"].ToString() : null,
                UmbralActivacion = reader["UMBRALACTIVACION"] != DBNull.Value ? Convert.ToDecimal(reader["UMBRALACTIVACION"]) : (decimal?)null
            };
        }

        public Response<SimuladorRiego> Insertar(SimuladorRiego entidad)
        {
            Response<SimuladorRiego> response = new Response<SimuladorRiego>();

            string queryId = "SELECT SEQ_RIEGO.NEXTVAL FROM DUAL";
            string queryInsert = "INSERT INTO SIMULADORRIEGO (ID_RIEGO, ID_PARCELA, ESTADORIEGO, UMBRALACTIVACION) " +
                                 "VALUES (:Id, :IdParcela, :EstadoRiego, :UmbralActivacion)";

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
                    command.Parameters.Add(new OracleParameter("EstadoRiego", entidad.EstadoRiego ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("UmbralActivacion", entidad.UmbralActivacion ?? (object)DBNull.Value));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                entidad.Id = nuevoId;
                response.Estado = true;
                response.Mensaje = "Simulación de riego registrada exitosamente.";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al insertar simulación de riego: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<SimuladorRiego> Actualizar(SimuladorRiego entidad)
        {
            Response<SimuladorRiego> response = new Response<SimuladorRiego>();
            string query = "UPDATE SIMULADORRIEGO SET ID_PARCELA = :IdParcela, ESTADORIEGO = :EstadoRiego, " +
                           "UMBRALACTIVACION = :UmbralActivacion WHERE ID_RIEGO = :Id";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    command.Transaction = transaction;
                    command.Parameters.Add(new OracleParameter("IdParcela", entidad.IdParcela));
                    command.Parameters.Add(new OracleParameter("EstadoRiego", entidad.EstadoRiego ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("UmbralActivacion", entidad.UmbralActivacion ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Id", entidad.Id));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                response.Estado = true;
                response.Mensaje = "Simulación de riego actualizada exitosamente.";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al actualizar simulación de riego: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<SimuladorRiego> Eliminar(int id)
        {
            Response<SimuladorRiego> response = new Response<SimuladorRiego>();
            string query = "DELETE FROM SIMULADORRIEGO WHERE ID_RIEGO = :Id";

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
                response.Mensaje = "Simulación de riego eliminada exitosamente.";
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al eliminar simulación de riego: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<SimuladorRiego> ObtenerPorId(int id)
        {
            Response<SimuladorRiego> response = new Response<SimuladorRiego>();
            string query = "SELECT * FROM SIMULADORRIEGO WHERE ID_RIEGO = :Id";

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
                            response.Mensaje = "Simulación de riego encontrada.";
                            response.Entidad = Mapear(reader);
                        }
                        else
                        {
                            response.Estado = false;
                            response.Mensaje = "Simulación de riego no encontrada.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener simulación de riego: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<SimuladorRiego> ObtenerTodos()
        {
            Response<SimuladorRiego> response = new Response<SimuladorRiego>();
            List<SimuladorRiego> listaRiegos = new List<SimuladorRiego>();
            string query = "SELECT * FROM SIMULADORRIEGO ORDER BY ID_PARCELA";

            try
            {
                AbrirConexion();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaRiegos.Add(Mapear(reader));
                        }
                    }
                }

                response.Estado = true;
                response.Mensaje = listaRiegos.Count > 0
                    ? $"Se encontraron {listaRiegos.Count} simulaciones de riego."
                    : "No hay simulaciones de riego registradas.";
                response.Lista = listaRiegos;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener simulaciones de riego: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        // 🔍 Método adicional: Buscar por estado de riego
        //public Response<SimuladorRiego> BuscarPorEstado(string estadoRiego)
        //{
        //    Response<SimuladorRiego> response = new Response<SimuladorRiego>();
        //    List<SimuladorRiego> lista = new List<SimuladorRiego>();
        //    string query = "SELECT * FROM SIMULADORRIEGO WHERE ESTADORIEGO = :EstadoRiego";

        //    try
        //    {
        //        AbrirConexion();

        //        using (OracleCommand command = new OracleCommand(query, conexion))
        //        {
        //            command.Parameters.Add(new OracleParameter("EstadoRiego", estadoRiego));

        //            using (OracleDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    lista.Add(Mapear(reader));
        //                }
        //            }
        //        }

        //        if (lista.Count > 0)
        //        {
        //            response.Estado = true;
        //            response.Mensaje = $"Se encontraron {lista.Count} registros con estado '{estadoRiego}'.";
        //            response.Lista = lista;
        //        }
        //        else
        //        {
        //            response.Estado = false;
        //            response.Mensaje = $"No se encontraron registros con estado '{estadoRiego}'.";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Estado = false;
        //        response.Mensaje = "Error al buscar por estado: " + ex.Message;
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return response;
        //}



    }
}
