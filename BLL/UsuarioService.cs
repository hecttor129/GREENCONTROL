using DAL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class UsuarioService : ICrudEscritura<Usuario>, ICrudLectura<Usuario>
    {
        private readonly UsuarioRepository usuarioRepository;

        public UsuarioService()
        {
            usuarioRepository = new UsuarioRepository();
        }

        public string Guardar(Usuario entidad)
        {
            var response = usuarioRepository.Insertar(entidad);
            return response.Mensaje;
        }

        public bool Actualizar(Usuario entidad)
        {
            var response = usuarioRepository.Actualizar(entidad);
            return response.Estado;
        }

        public bool Eliminar(Usuario entidad)
        {
            var response = usuarioRepository.Eliminar(entidad.IdUsuario);
            return response.Estado;
        }

        public ReadOnlyCollection<Usuario> Consultar()
        {
            var response = usuarioRepository.ObtenerTodos();
            var lista = response.Lista ?? new List<Usuario>();
            return new ReadOnlyCollection<Usuario>(lista);
        }

        public Usuario ObtenerPorId(string id)
        {
            var response = usuarioRepository.ObtenerPorId(Convert.ToInt32(id));
            return response.Entidad;
        }
    }
}
