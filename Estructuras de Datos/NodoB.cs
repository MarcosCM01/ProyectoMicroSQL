using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estructuras_de_Datos
{
    public class NodoB<T> : IComparable
    {
        public NodoB<T> Padre { get; set; }
        public List<NodoB<T>> Hijos { get; set; }

        public int id { get; set; }
        public int max { get; set; }
        public int min { get; set; }

        public List<Registro> Valores { get; set; }
        public NodoB()
        {
            Padre = null;
            Hijos = new List<NodoB<T>>();
            Valores = new List<Registro>();
            id = 0;
            max = 0;
            min = 0;
        }

        public void AsignarGrado(NodoB<T> Nodo, int grado)
        {
            Nodo.max = grado - 1;
            Nodo.min = Nodo.max / 2;
        }

        public int CompareTo(object obj)
        {
            var comparado = (NodoB<T>)obj;
            return id.CompareTo(comparado.id);
        }

    }
}
