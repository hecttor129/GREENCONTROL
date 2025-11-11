using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENTITY;
using Oracle.ManagedDataAccess.Client;


namespace DAL
{
    public class UsuarioRepository : ConexionOracle, IRepository<Usuario>
    {
        public Response<Usuario> Insertar(Usuario entidad)
        {
            Response<Usuario> response = new Response<Usuario>();

            string queryId = "SELECT SEQ_USUARIO.NEXTVAL FROM DUAL";
            string queryInsert = @"INSERT INTO USUARIO 
                                   (ID_USUARIO, NOMBRE, EMAIL, PASSWORD, TELEFONO)
                                   VALUES (:Id, :Nombre, :Email, :Password, :Telefono)";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                // Obtener siguiente valor de la secuencia
                int nuevoId;
                using (OracleCommand commandId = new OracleCommand(queryId, conexion))
                {
                    commandId.Transaction = transaction;
                    nuevoId = Convert.ToInt32(commandId.ExecuteScalar());
                }

                // Insertar usuario
                using (OracleCommand command = new OracleCommand(queryInsert, conexion))
                {
                    command.Transaction = transaction;
                    command.Parameters.Add(new OracleParameter("Id", nuevoId));
                    command.Parameters.Add(new OracleParameter("Nombre", entidad.Nombre));
                    command.Parameters.Add(new OracleParameter("Email", entidad.Email));
                    command.Parameters.Add(new OracleParameter("Password", entidad.Password));
                    command.Parameters.Add(new OracleParameter("Telefono", entidad.Telefono ?? (object)DBNull.Value));
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                entidad.Id = nuevoId;

                response.Estado = true;
                response.Mensaje = "Usuario registrado exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al insertar usuario: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Usuario> Actualizar(Usuario entidad)
        {
            Response<Usuario> response = new Response<Usuario>();
            string query = @"UPDATE USUARIO 
                             SET NOMBRE = :Nombre, EMAIL = :Email, PASSWORD = :Password, TELEFONO = :Telefono
                             WHERE ID_USUARIO = :Id";

            OracleTransaction transaction = null;

            try
            {
                AbrirConexion();
                transaction = conexion.BeginTransaction();

                using (OracleCommand command = new OracleCommand(query, conexion))
                {
                    command.Transaction = transaction;
                    command.Parameters.Add(new OracleParameter("Nombre", entidad.Nombre));
                    command.Parameters.Add(new OracleParameter("Email", entidad.Email));
                    command.Parameters.Add(new OracleParameter("Password", entidad.Password));
                    command.Parameters.Add(new OracleParameter("Telefono", entidad.Telefono ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("Id", entidad.Id));

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                response.Estado = true;
                response.Mensaje = "Usuario actualizado exitosamente";
                response.Entidad = entidad;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al actualizar usuario: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Usuario> Eliminar(int id)
        {
            Response<Usuario> response = new Response<Usuario>();
            string query = "DELETE FROM USUARIO WHERE ID_USUARIO = :Id";

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
                response.Mensaje = "Usuario eliminado exitosamente";
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                response.Estado = false;
                response.Mensaje = "Error al eliminar usuario: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Usuario> ObtenerPorId(int id)
        {
            Response<Usuario> response = new Response<Usuario>();
            string query = "SELECT * FROM USUARIO WHERE ID_USUARIO = :Id";

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
                            Usuario usuario = Mapear(reader);
                            response.Estado = true;
                            response.Mensaje = "Usuario encontrado";
                            response.Entidad = usuario;
                        }
                        else
                        {
                            response.Estado = false;
                            response.Mensaje = "Usuario no encontrado";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener usuario: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        public Response<Usuario> ObtenerTodos()
        {
            Response<Usuario> response = new Response<Usuario>();
            List<Usuario> listaUsuarios = new List<Usuario>();
            string query = "SELECT * FROM USUARIO ORDER BY ID_USUARIO";

            try
            {
                AbrirConexion();

                using (OracleCommand command = new OracleCommand(query, conexion))
                using (OracleDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listaUsuarios.Add(Mapear(reader));
                    }
                }

                response.Estado = true;
                response.Mensaje = "Usuarios obtenidos exitosamente";
                response.Lista = listaUsuarios;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = "Error al obtener usuarios: " + ex.Message;
            }
            finally
            {
                CerrarConexion();
            }

            return response;
        }

        private Usuario Mapear(OracleDataReader reader)
        {
            return new Usuario
            {
                Id = Convert.ToInt32(reader["ID_USUARIO"]),
                Nombre = Convert.ToString(reader["NOMBRE"]),
                Email = Convert.ToString(reader["EMAIL"]),
                Password = Convert.ToString(reader["PASSWORD"]),
                Telefono = reader["TELEFONO"] != DBNull.Value ? Convert.ToString(reader["TELEFONO"]) : null
            };
        }
    }
}
