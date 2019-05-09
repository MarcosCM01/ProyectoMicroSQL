using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estructuras_de_Datos
{
    interface IArboles<T> : IEnumerable<T> where  T : IComparable
    {
        void Insertar(NodoB<T> Nodo, T valor);
        void Eliminar(T valor, NodoB<T> Nodo);
    }
}
