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
                Id = Convert.ToInt32(reader["ID_CULTIVO"]),
                Nombre = reader["NOMBRE"].ToString(),
                Variedad = reader["VARIEDAD"] != DBNull.Value ? reader["VARIEDAD"].ToString() : null,
                DuracionCiclo_Fecha1 = reader["DURACIONCICLO_FECHA1"] != DBNull.Value ? Convert.ToInt32(reader["DURACIONCICLO_FECHA1"]) : (int?)null,
                DuracionCiclo_Fecha2 = reader["DURACIONCICLO_FECHA2"] != DBNull.Value ? Convert.ToInt32(reader["DURACIONCICLO_FECHA2"]) : (int?)null,
                DiasGerminacion_Fecha1 = reader["DIASGERMINACION_FECHA1"] != DBNull.Value ? Convert.ToInt32(reader["DIASGERMINACION_FECHA1"]) : (int?)null,
                DiasGerminacion_Fecha2 = reader["DIASGERMINACION_FECHA2"] != DBNull.Value ? Convert.ToInt32(reader["DIASGERMINACION_FECHA2"]) : (int?)null,
                DiasFloracion_Fecha1 = reader["DIASFLORACION_FECHA1"] != DBNull.Value ? Convert.ToInt32(reader["DIASFLORACION_FECHA1"]) : (int?)null,
                DiasFloracion_Fecha2 = reader["DIASFLORACION_FECHA2"] != DBNull.Value ? Convert.ToInt32(reader["DIASFLORACION_FECHA2"]) : (int?)null,
                DiasCosecha_Fecha1 = reader["DIASCOSECHA_FECHA1"] != DBNull.Value ? Convert.ToInt32(reader["DIASCOSECHA_FECHA1"]) : (int?)null,
                DiasCosecha_Fecha2 = reader["DIASCOSECHA_FECHA2"] != DBNull.Value ? Convert.ToInt32(reader["DIASCOSECHA_FECHA2"]) : (int?)null,
                TipoSuelo = reader["TIPOSUELO"] != DBNull.Value ? reader["TIPOSUELO"].ToString() : null,
                TemperaturaOptima = reader["TEMPERATURAOPTIMA"] != DBNull.Value ? Convert.ToDecimal(reader["TEMPERATURAOPTIMA"]) : (decimal?)null,
                PhSuelo = reader["PHSUELO"] != DBNull.Value ? Convert.ToDecimal(reader["PHSUELO"]) : (decimal?)null,
                HumedadOptima = reader["HUMEDADOPTIMA"] != DBNull.Value ? Convert.ToDecimal(reader["HUMEDADOPTIMA"]) : (decimal?)null,
                Descripcion = reader["DESCRIPCION"] != DBNull.Value ? reader["DESCRIPCION"].ToString() : null
            };
        }

        public Response<Cultivo> Insertar(Cultivo entidad)
        {
            Response<Cultivo> response = new Response<Cultivo>();

            string queryId = "SELECT SEQ_CULTIVO.NEXTVAL FROM DUAL";
            string queryInsert = @"INSERT INTO CULTIVO 
                (ID_CULTIVO, NOMBRE, VARIEDAD, DURACIONCICLO_FECHA1, DURACIONCICLO_FECHA2,
                DIASGERMINACION_FECHA1, DIASGERMINACION_FECHA2, DIASFLORACION_FECHA1, DIASFLORACION_FECHA2,
                DIASCOSECHA_FECHA1, DIASCOSECHA_FECHA2, TIPOSUELO, TEMPERATURAOPTIMA, PHSUELO, HUMEDADOPTIMA, DESCRIPCION)
                VALUES (:Id, :Nombre, :Variedad, :DuracionCiclo_Fecha1, :DuracionCiclo_Fecha2,
                :DiasGerminacion_Fecha1, :DiasGerminacion_Fecha2, :DiasFloracion_Fecha1, :DiasFloracion_Fecha2,
                :DiasCosecha_Fecha1, :DiasCosecha_Fecha2, :TipoSuelo, :TemperaturaOptima, :PhSuelo, :HumedadOptima, :Descripcion)";

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
                    command.Parameters.Add(new OracleParameter("DuracionCiclo_Fecha1", entidad.DuracionCiclo_Fecha1 ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("DuracionCiclo_Fecha2", entidad.DuracionCiclo_Fecha2 ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("DiasGerminacion_Fecha1", entidad.DiasGerminacion_Fecha1 ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("DiasGerminacion_Fecha2", entidad.DiasGerminacion_Fecha2 ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("DiasFloracion_Fecha1", entidad.DiasFloracion_Fecha1 ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("DiasFloracion_Fecha2", entidad.DiasFloracion_Fecha2 ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("DiasCosecha_Fecha1", entidad.DiasCosecha_Fecha1 ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("DiasCosecha_Fecha2", entidad.DiasCosecha_Fecha2 ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("TipoSuelo", entidad.TipoSuelo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("TemperaturaOptima", entidad.TemperaturaOptima ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("PhSuelo", entidad.PhSuelo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("HumedadOptima", entidad.HumedadOptima ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Descripcion", entidad.Descripcion ?? (object)DBNull.Value));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                entidad.Id = nuevoId;
                response.Estado = true;
                response.Mensaje = "Cultivo registrado exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al insertar cultivo: " + ex.Message;
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
                DURACIONCICLO_FECHA1 = :DuracionCiclo_Fecha1,
                DURACIONCICLO_FECHA2 = :DuracionCiclo_Fecha2,
                DIASGERMINACION_FECHA1 = :DiasGerminacion_Fecha1,
                DIASGERMINACION_FECHA2 = :DiasGerminacion_Fecha2,
                DIASFLORACION_FECHA1 = :DiasFloracion_Fecha1,
                DIASFLORACION_FECHA2 = :DiasFloracion_Fecha2,
                DIASCOSECHA_FECHA1 = :DiasCosecha_Fecha1,
                DIASCOSECHA_FECHA2 = :DiasCosecha_Fecha2,
                TIPOSUELO = :TipoSuelo,
                TEMPERATURAOPTIMA = :TemperaturaOptima,
                PHSUELO = :PhSuelo,
                HUMEDADOPTIMA = :HumedadOptima,
                DESCRIPCION = :Descripcion
                WHERE ID_CULTIVO = :Id";

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
                    command.Parameters.Add(new OracleParameter("DuracionCiclo_Fecha1", entidad.DuracionCiclo_Fecha1 ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("DuracionCiclo_Fecha2", entidad.DuracionCiclo_Fecha2 ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("DiasGerminacion_Fecha1", entidad.DiasGerminacion_Fecha1 ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("DiasGerminacion_Fecha2", entidad.DiasGerminacion_Fecha2 ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("DiasFloracion_Fecha1", entidad.DiasFloracion_Fecha1 ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("DiasFloracion_Fecha2", entidad.DiasFloracion_Fecha2 ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("DiasCosecha_Fecha1", entidad.DiasCosecha_Fecha1 ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("DiasCosecha_Fecha2", entidad.DiasCosecha_Fecha2 ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("TipoSuelo", entidad.TipoSuelo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("TemperaturaOptima", entidad.TemperaturaOptima ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("PhSuelo", entidad.PhSuelo ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("HumedadOptima", entidad.HumedadOptima ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Descripcion", entidad.Descripcion ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Id", entidad.Id));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                response.Estado = true;
                response.Mensaje = "Cultivo actualizado exitosamente";
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
            string query = "DELETE FROM CULTIVO WHERE ID_CULTIVO = :Id";

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
                response.Mensaje = "Cultivo eliminado exitosamente";
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
            string query = "SELECT * FROM CULTIVO WHERE ID_CULTIVO = :Id";

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
                            response.Mensaje = "Cultivo encontrado";
                            response.Entidad = Mapear(reader);
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
            string query = "SELECT * FROM CULTIVO ORDER BY NOMBRE";

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
                response.Mensaje = lista.Count > 0 ? $"Se encontraron {lista.Count} cultivos" : "No hay cultivos registrados";
                response.Lista = lista;
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

        //public Response<Cultivo> BuscarPorNombre(string nombre)
        //{
        //    Response<Cultivo> response = new Response<Cultivo>();
        //    List<Cultivo> listaCultivos = new List<Cultivo>();
        //    string query = "SELECT * FROM CULTIVO WHERE UPPER(NOMBRE) LIKE UPPER(:Nombre) ORDER BY NOMBRE";

        //    try
        //    {
        //        AbrirConexion();

        //        using (OracleCommand command = new OracleCommand(query, conexion))
        //        {
        //            // El patrón de búsqueda con comodines
        //            command.Parameters.Add(new OracleParameter("Nombre", "%" + nombre + "%"));

        //            using (OracleDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    listaCultivos.Add(Mapear(reader));
        //                }
        //            }
        //        }

        //        response.Estado = true;
        //        if (listaCultivos.Count > 0)
        //        {
        //            response.Mensaje = $"Se encontraron {listaCultivos.Count} cultivos que coinciden con '{nombre}'";
        //            response.Lista = listaCultivos;
        //        }
        //        else
        //        {
        //            response.Mensaje = $"No se encontraron cultivos con el nombre '{nombre}'";
        //            response.Lista = new List<Cultivo>();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Estado = false;
        //        response.Mensaje = "Error al buscar cultivos por nombre: " + ex.Message;
        //    }
        //    finally
        //    {
        //        CerrarConexion();
        //    }

        //    return response;
        //}


    }
}
