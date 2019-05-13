using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Collections;

namespace ProyectoMicroSQL.Models
{
    public class RegistrosDeTabla : IEnumerable
    {
        public List<Estructuras_de_Datos.Registro> Valores { get; set; }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}