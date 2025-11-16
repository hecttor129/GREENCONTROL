using ENTITY;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class PrediccionesSiembraRepository : ConexionOracle, IRepository<PrediccionesSiembra>
    {
        public Response<PrediccionesSiembra> Insertar(PrediccionesSiembra entidad)
        {
            Response<PrediccionesSiembra> response = new Response<PrediccionesSiembra>();

            string queryId = "SELECT SEQ_PREDICCIONESSIEMBRA.NEXTVAL FROM DUAL";

            string queryInsert = @"
            INSERT INTO PREDICCIONESSIEMBRA
            (IDPREDICCION, IDSIEMBRA, FECHACALCULO,
             DIASGERMINACIONESTIMADO, DIASFLORACIONESTIMADO,
             DIASCOSECHAESTIMADO, DURACIONCICLOESTIMADO,
             CALIDADESPERADA, CANTIDADESTIMADA, RENTABILIDADESPERADA)
            VALUES
            (:Id, :IdSiembra, :FechaCalculo,
             :DiasGerminacion, :DiasFloracion,
             :DiasCosecha, :DuracionCiclo,
             :Calidad, :Cantidad, :Rentabilidad)
        ";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                // Obtener ID por secuencia
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
                    cmd.Parameters.Add(new OracleParameter("IdSiembra", entidad.IdSiembra));
                    cmd.Parameters.Add(new OracleParameter("FechaCalculo", entidad.FechaCalculo ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("DiasGerminacion", entidad.DiasGerminacionEstimado ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("DiasFloracion", entidad.DiasFloracionEstimado ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("DiasCosecha", entidad.DiasCosechaEstimado ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("DuracionCiclo", entidad.DuracionCicloEstimado ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("Calidad", entidad.CalidadEsperada ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("Cantidad", entidad.CantidadEstimada ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("Rentabilidad", entidad.RentabilidadEsperada ?? (object)DBNull.Value));

                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();

                entidad.IdPrediccion = nuevoId;
                response.Estado = true;
                response.Mensaje = "Predicción registrada exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al insertar predicción: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<PrediccionesSiembra> Actualizar(PrediccionesSiembra entidad)
        {
            Response<PrediccionesSiembra> response = new Response<PrediccionesSiembra>();

            string query = @"
            UPDATE PREDICCIONESSIEMBRA SET
                IDSIEMBRA = :IdSiembra,
                FECHACALCULO = :FechaCalculo,
                DIASGERMINACIONESTIMADO = :DiasGerminacion,
                DIASFLORACIONESTIMADO = :DiasFloracion,
                DIASCOSECHAESTIMADO = :DiasCosecha,
                DURACIONCICLOESTIMADO = :DuracionCiclo,
                CALIDADESPERADA = :Calidad,
                CANTIDADESTIMADA = :Cantidad,
                RENTABILIDADESPERADA = :Rentabilidad
            WHERE IDPREDICCION = :Id
        ";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                using (OracleCommand cmd = new OracleCommand(query, conexion))
                {
                    cmd.Transaction = transaction;

                    cmd.Parameters.Add(new OracleParameter("IdSiembra", entidad.IdSiembra));
                    cmd.Parameters.Add(new OracleParameter("FechaCalculo", entidad.FechaCalculo ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("DiasGerminacion", entidad.DiasGerminacionEstimado ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("DiasFloracion", entidad.DiasFloracionEstimado ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("DiasCosecha", entidad.DiasCosechaEstimado ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("DuracionCiclo", entidad.DuracionCicloEstimado ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("Calidad", entidad.CalidadEsperada ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("Cantidad", entidad.CantidadEstimada ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("Rentabilidad", entidad.RentabilidadEsperada ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("Id", entidad.IdPrediccion));

                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
                response.Estado = true;
                response.Mensaje = "Predicción actualizada exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al actualizar predicción: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<PrediccionesSiembra> Eliminar(int id)
        {
            Response<PrediccionesSiembra> response = new Response<PrediccionesSiembra>();
            string query = "DELETE FROM PREDICCIONESSIEMBRA WHERE IDPREDICCION = :Id";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                using (OracleCommand cmd = new OracleCommand(query, conexion))
                {
                    cmd.Transaction = transaction;
                    cmd.Parameters.Add(new OracleParameter("Id", id));
                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();

                response.Estado = true;
                response.Mensaje = "Predicción eliminada exitosamente";
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al eliminar predicción: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<PrediccionesSiembra> ObtenerPorId(int id)
        {
            Response<PrediccionesSiembra> response = new Response<PrediccionesSiembra>();
            string query = "SELECT * FROM PREDICCIONESSIEMBRA WHERE IDPREDICCION = :Id";

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
                            var pred = Mapear(reader);
                            response.Estado = true;
                            response.Mensaje = "Predicción encontrada";
                            response.Entidad = pred;
                        }
                        else
                        {
                            response.Estado = false;
                            response.Mensaje = "Predicción no encontrada";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener predicción: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<PrediccionesSiembra> ObtenerTodos()
        {
            Response<PrediccionesSiembra> response = new Response<PrediccionesSiembra>();
            List<PrediccionesSiembra> lista = new List<PrediccionesSiembra>();

            string query = "SELECT * FROM PREDICCIONESSIEMBRA ORDER BY IDPREDICCION";

            try
            {
                AbrirConexion();

                using (OracleCommand cmd = new OracleCommand(query, conexion))
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(Mapear(reader));
                    }
                }

                response.Estado = true;
                response.Lista = lista;
                response.Mensaje = "Predicciones obtenidas";
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener predicciones: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        private PrediccionesSiembra Mapear(OracleDataReader r)
        {
            return new PrediccionesSiembra
            {
                IdPrediccion = Convert.ToInt32(r["IDPREDICCION"]),
                IdSiembra = Convert.ToInt32(r["IDSIEMBRA"]),
                FechaCalculo = r["FECHACALCULO"] != DBNull.Value ? Convert.ToDateTime(r["FECHACALCULO"]) : (DateTime?)null,
                DiasGerminacionEstimado = r["DIASGERMINACIONESTIMADO"] != DBNull.Value ? Convert.ToInt32(r["DIASGERMINACIONESTIMADO"]) : (int?)null,
                DiasFloracionEstimado = r["DIASFLORACIONESTIMADO"] != DBNull.Value ? Convert.ToInt32(r["DIASFLORACIONESTIMADO"]) : (int?)null,
                DiasCosechaEstimado = r["DIASCOSECHAESTIMADO"] != DBNull.Value ? Convert.ToInt32(r["DIASCOSECHAESTIMADO"]) : (int?)null,
                DuracionCicloEstimado = r["DURACIONCICLOESTIMADO"] != DBNull.Value ? Convert.ToInt32(r["DURACIONCICLOESTIMADO"]) : (int?)null,
                CalidadEsperada = r["CALIDADESPERADA"] != DBNull.Value ? Convert.ToInt32(r["CALIDADESPERADA"]) : (int?)null,
                CantidadEstimada = r["CANTIDADESTIMADA"] != DBNull.Value ? Convert.ToDecimal(r["CANTIDADESTIMADA"]) : (decimal?)null,
                RentabilidadEsperada = r["RENTABILIDADESPERADA"] != DBNull.Value ? Convert.ToDecimal(r["RENTABILIDADESPERADA"]) : (decimal?)null
            };
        }
    }
}
