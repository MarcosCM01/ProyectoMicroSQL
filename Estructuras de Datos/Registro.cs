using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estructuras_de_Datos
{
    public class Registro : IComparable
    {
        public int IDPrimaryKey { get; set; }

        public List<string> Valores { get; set; }

        public Registro()
        {
            Valores = new List<string>();
            IDPrimaryKey = 0;
        }

        public int CompareTo(object obj)
        {
            var comparado = (Registro)obj;
            return IDPrimaryKey.CompareTo(comparado.IDPrimaryKey);
        }
    }
}
