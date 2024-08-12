using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontEndHealthPets.Entidades.Entitys
{
   public class Sesion
    {
        public static long id_usuario { get; set; }
        public static string nombre { get; set; }
        public static string apellidos { get; set; }
        public static string token { get; set; }

        public static bool validarSesion()
        {
            if (String.IsNullOrEmpty(Sesion.token))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static void cerrarSesion()
        {
            Sesion.id_usuario = 0;
            Sesion.nombre = String.Empty;
            Sesion.apellidos = String.Empty;
            Sesion.token = String.Empty;

        }
        }
}
