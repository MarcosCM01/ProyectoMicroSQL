using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Collections;

namespace ProyectoMicroSQL.Models
{
    public class Tablas : IEnumerable
    {
        [DisplayName("Nombre de Tabla")]
        string NombreTabla { get; set; }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)NombreTabla).GetEnumerator();
        }
    }
}