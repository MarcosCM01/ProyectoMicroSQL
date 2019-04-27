using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estructuras_de_Datos
{
    public class ArbolB<T> : IEnumerable<T> where T : IComparable
    {
        public NodoB<T> Raiz { get; set; }

        public ArbolB()
        {
            Raiz = null;

        }


        public bool ExisteNodosHijo(NodoB<T> Nodo)
        {

            if (Nodo.Hijos.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool ExisteEspacio(NodoB<T> Nodo)
        {
            if (Nodo.Valores.Count <= Nodo.max)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public void Insertar(NodoB<T> Nodo, T valor)
        {
            if (Raiz == null)
            {
                Raiz = new NodoB<T>();
                Raiz.AsignarGrado(Raiz, 5);
                Raiz.Valores.Add(valor);
                Raiz.id = 1;
                Nodo = Raiz;
            }
            //ES HOJA
            else if (ExisteNodosHijo(Nodo) == false)
            {
                AgregarYOrdenarNodo(valor, Nodo);
            }
            //NO ES HOJA
            else if (ExisteNodosHijo(Nodo) == true)
            {
                var NodoHijo = new NodoB<T>();
                NodoHijo = Nodo.Hijos[PosicionHijo(Nodo, valor)]; //BUSCA POSICION CORRESPONDIENTE
                Insertar(NodoHijo, valor);
            }

            if (ExisteEspacio(Nodo) == false)
            {
                Separar(Nodo);
            }
        }

        public void CreandoNodo(NodoB<T> nuevo, NodoB<T> nodo)
        {
            nuevo.max = nodo.max;
            nuevo.min = nodo.min;
            nuevo.id = nodo.id++;
        }

        public void Separar(NodoB<T> Nodo)
        {
            NodoB<T> izq = new NodoB<T>();
            NodoB<T> padreAux = new NodoB<T>();
            NodoB<T> der = new NodoB<T>();
            CreandoNodo(izq, Nodo);
            CreandoNodo(der, Nodo);

            for (int i = 0; i < Nodo.min; i++)
            {
                izq.Valores.Add(Nodo.Valores[i]);
            }

            for (int i = Nodo.min + 1; i <= Nodo.max; i++)
            {
                der.Valores.Add(Nodo.Valores[i]);
            }


            if (Nodo.Padre != null) //Si es cualquier hijo
            {
                PadreHijo(Nodo.Padre, izq);
                PadreHijo(Nodo.Padre, der);

                Nodo.Padre.Valores.Add(Nodo.Valores[Nodo.min]);
                Nodo.Padre.Valores.Sort((x, y) => x.CompareTo(y));

                int indice = 0;

                for (int i = 0; i < Nodo.Padre.Hijos.Count; i++)
                {
                    if (Nodo.Padre.Hijos[i].Valores.Count > 4)
                    {
                        indice = i;
                        break;
                    }
                }

                if (Nodo.Hijos.Count > 0)
                {
                    HijosDeHijos(Nodo, izq, 0, Nodo.min);
                    HijosDeHijos(Nodo, der, Nodo.min + 1, Nodo.max + 1);
                }

                Nodo.Padre.Hijos.RemoveAt(indice);
                Nodo = null;
            }//Si es la raiz y aun caben valores en el nodo
            else if (Nodo.Padre == null && Nodo.Hijos.Count < 5)
            {
                padreAux.Valores.Add(Nodo.Valores[Nodo.min]);
                PadreHijo(Nodo, izq);
                PadreHijo(Nodo, der);
                Nodo.Valores.Sort((x, y) => x.CompareTo(y));
                Nodo.Valores = padreAux.Valores;
            }//Si es raiz y no caben valores
            else if (Nodo.Padre == null && Nodo.Hijos.Count >= 5)
            {
                T val = Nodo.Valores[Nodo.min];

                HijosDeHijos(Nodo, izq, 0, Nodo.min);
                HijosDeHijos(Nodo, der, Nodo.min + 1, Nodo.max + 1);

                Nodo.Hijos.Clear();
                PadreHijo(Nodo, izq);
                PadreHijo(Nodo, der);

                Nodo.Valores.Clear();
                Nodo.Valores.Add(val);
            }

        }

        public void HijosDeHijos(NodoB<T> Nodo, NodoB<T> hijo, int inicio, int fin)
        {
            for (int i = inicio; i <= fin; i++)
            {
                hijo.Hijos.Add(Nodo.Hijos[i]);
            }
            foreach (var item in hijo.Hijos)
            {
                item.Padre = hijo;
            }
        }

        public void PadreHijo(NodoB<T> Padre, NodoB<T> Hijo)
        {
            Padre.Hijos.Add(Hijo);
            Hijo.Padre = Padre;
        }

        public int PosicionHijo(NodoB<T> Nodo, T valor)
        {
            if (Nodo.Valores.Count == 1)
            {
                if (valor.CompareTo(Nodo.Valores[0]) < 0)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                for (int i = 0; i < Nodo.Valores.Count - 1; i++)
                {
                    if (valor.CompareTo(Nodo.Valores[i]) < 0)
                    {
                        return i;
                    }
                    else if (valor.CompareTo(Nodo.Valores[i]) > 0 && valor.CompareTo(Nodo.Valores[i + 1]) < 0)
                    {
                        return i + 1;
                    }
                }
                return Nodo.Valores.Count;
            }
        }

        public void AgregarYOrdenarNodo(T valor, NodoB<T> Nodo)
        {
            Nodo.Valores.Add(valor);
            Nodo.Valores.Sort((x, y) => x.CompareTo(y));
        }


        static T val;
        public T Busqueda(T valor, NodoB<T> Nodo)
        {
            bool BEncontrado = false;
            foreach (var item in Nodo.Valores)
            {
                if (item.CompareTo(valor) == 0)
                {
                    BEncontrado = true;
                    val = item;
                    break;
                }
            }

            if (BEncontrado == false && Nodo.Hijos.Count > 0)
            {
                NodoB<T> NodoHijo = new NodoB<T>();
                NodoHijo = Nodo.Hijos[PosicionHijo(Nodo, valor)];
                return Busqueda(valor, NodoHijo);
            }
            else if (Nodo.Hijos.Count == 0)
            {
                throw new NotImplementedException();
            }
            else
            {
                return val;
            }

        }

        public bool ExisteUnderFlow(NodoB<T> Nodo)
        {
            if (Nodo.Hijos.Count < Nodo.min)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static int contador = 0;
        static bool IrDerecha = false;
        static NodoB<T> NodoAModificar = new NodoB<T>();
        static NodoB<T> NodoASustituir = new NodoB<T>();

        public void Eliminar(T valor, NodoB<T> Nodo)
        {
            int indice = 0;
            bool NodoInicial = false;
            bool BEncontrado = false;
            for (int i = 0; i < Nodo.Valores.Count; i++)
            {
                if (Nodo.Valores[i].CompareTo(valor) == 0)
                {
                    BEncontrado = true;
                    if (ExisteNodosHijo(Nodo) == true)
                    {
                        if (contador == 0)
                        {
                            indice = i;  //INDICE DE VALOR A SUSTITUIR
                            NodoAModificar = Nodo; //NODO A SUSTITUIR VALOR DE NODO MAS IZQUIERDO DEL HIJO DERECHO
                            contador++;
                            NodoInicial = true;
                        }

                        NodoASustituir = MasIzquierdoDeDerecho(indice, Nodo);

                        NodoAModificar.Valores.Add(NodoASustituir.Valores[0]);
                        NodoAModificar.Valores.RemoveAt(indice);
                        NodoASustituir.Valores.RemoveAt(0);

                        NodoAModificar.Valores.Sort((x, y) => x.CompareTo(y));

                        if (NodoInicial == true)
                        {
                            Nodo = NodoAModificar;
                        }
                    }
                    else
                    {
                        Nodo.Valores.RemoveAt(i);
                    }

                    if (ExisteUnderFlow(Nodo) == true)
                    {

                    }
                }
            }

            if (BEncontrado == false && Nodo.Hijos.Count > 0)
            {
                NodoB<T> NodoHijo = new NodoB<T>();
                NodoHijo = Nodo.Hijos[PosicionHijo(Nodo, valor)];
                Eliminar(valor, NodoHijo);
            }
        }

        public NodoB<T> MasIzquierdoDeDerecho(int IndiceValor, NodoB<T> Nodo)
        {
            if (IrDerecha == false && Nodo.Hijos.Count > 0)
            {
                IrDerecha = true;
                return MasIzquierdoDeDerecho(IndiceValor, Nodo.Hijos[IndiceValor + 1]);
            }
            else if (IrDerecha == true && Nodo.Hijos.Count > 0)
            {
                return MasIzquierdoDeDerecho(IndiceValor, Nodo.Hijos[0]);
            }
            else if (IrDerecha == true && Nodo.Hijos.Count == 0)
            {
                return Nodo;
            }
            else
            {
                return Nodo;
            }

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
