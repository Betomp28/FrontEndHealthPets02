using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontEndHealthPets.Entidades.response
{
    public class Res_ObtenerImages
    {
        public List<string> Images { get; set; } // Cambia 'Images' a List<string> si tu API devuelve una lista de URLs
        public string Status { get; set; }
    }
}

