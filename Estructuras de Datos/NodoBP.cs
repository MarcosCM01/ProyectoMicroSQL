using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estructuras_de_Datos
{
    public class NodoBP<T> : IEnumerable<T> where T : IComparable
    {
        public NodoBP<T> padre { get; set; }

        public List<NodoBP<T>> hijos { get; set; }

        public NodoBP<T> hermano { get; set; }

        public List<Registro> values { get; set; }

        public int id { get; set; }
        public int max { get; set; }
        public int min { get; set; }

        public NodoBP()
        {
            padre = null;
            hermano = null;
            hijos = new List<NodoBP<T>>();
            values = new List<Registro>();
            id = 0;
            max = 0;
            min = 0;
        }


        public void AsignarGrado(NodoBP<T> Nodo, int grado)
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
