using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estructuras_de_Datos
{
    public class NodoB<T> : IEnumerable<T> where T : IComparable
    {
        public NodoB<T> Padre { get; set; }
        public List<NodoB<T>> Hijos { get; set; }

        public List<T> Valores { get; set; }

        public int id { get; set; }
        public int max { get; set; }
        public int min { get; set; }

        public NodoB()
        {
            Padre = null;
            Hijos = new List<NodoB<T>>();
            Valores = new List<T>();
            id = 0;
            max = 0;
            min = 0;
        }



        public void AsignarGrado(NodoB<T> Nodo, int grado)
        {
            Nodo.max = grado - 1;
            Nodo.min = Nodo.max / 2;
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

    }
}
