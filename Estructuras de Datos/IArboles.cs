using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estructuras_de_Datos
{
    interface IArboles<T>
    {
        void Insertar(T valor);
        void Eliminar(T valor);
    }
}
