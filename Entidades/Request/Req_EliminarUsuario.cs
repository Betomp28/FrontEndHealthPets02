using FrontEndHealthPets.Entidades.entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontEndHealthPets.Entidades
{
    public class Req_EliminarUsuario
    {
        public EliminarUsuarioDetails eliminar_usuario { get; set; }

        // Clase anidada para detalles de eliminación de usuario
        public class EliminarUsuarioDetails
        {
            public string correoElectronico { get; set; }
        }
    }
}