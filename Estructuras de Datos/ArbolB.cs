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
        public int siguienteposicion { get; set; }
        public List<string> ListaVariables { get; set; }

        public ArbolB()
        {
            Raiz = new NodoB<T>();
            ListaVariables = new List<string>();
            siguienteposicion = 2;
        }

        public static bool ArbolVacio(NodoB<T> nodo) => nodo == null;
        public static List<NodoB<T>> ListaNodos = new List<NodoB<T>>();

        public void ExistenElementosEnLista()
        {
            if(ListaNodos.Count > 0)
            {
                ListaNodos.Clear();
            }
        }
        public void AlmacenandoNodosEnLista(NodoB<T> Nodo)
        {
            if (ExisteNodosHijo(Nodo) == true)
            {
                AlmacenandoNodosEnLista(Nodo.Hijos[0]);

                ListaNodos.Add(Nodo);

                for (int i = 1; i < Nodo.Hijos.Count; i++)
                {
                    AlmacenandoNodosEnLista(Nodo.Hijos[i]);
                }
            }
            else
            {
                ListaNodos.Add(Nodo);
            }
        }

        public List<NodoB<T>> RetornandoListaNodos()
        {
            return ListaNodos;
        }

        public void EliminarTodo(NodoB<T> Nodo)
        {
            if (ExisteNodosHijo(Nodo) == true)
            {
                if (Nodo.Hijos.Count > 0) 
                {
                    EliminarTodo(Nodo.Hijos[0]);
                }
                for (int i = 0; i < Nodo.Valores.Count; i++)
                {
                    Nodo.Valores.Clear();
                }
                if (Nodo.Hijos.Count > 0)
                {
                    EliminarTodo(Nodo.Hijos[Nodo.Hijos.Count - 1]);
                }
                for (int i = 0; i < Nodo.Valores.Count; i++)
                {
                    Nodo.Valores.Clear();
                }
            }
            else
            {
                Nodo.Valores.Clear();
            }
        }

        //public void EscrituraTXT(List<T> InfoLista)
        //{

        //}


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


        public int Insertar(NodoB<T> Nodo, Registro valor)
        {
            int IDIguales = BusquedaIDIgual(valor, Raiz);
            if(IDIguales == 1)
            {
                return 1;
            }
            else if (Raiz.id == 0)
            {
                Raiz.AsignarGrado(Raiz, 5);
                Raiz.Valores.Add(valor);
                Raiz.id = 1;
                Nodo = Raiz;
                return 0;
            }
            //ES HOJA
            else if (ExisteNodosHijo(Nodo) == false)
            {
                AgregarYOrdenarNodo(valor, Nodo);

                if (ExisteEspacio(Nodo) == false)
                {
                    Separar(Nodo);
                }

                return 0;
            }
            //NO ES HOJA
            else if (ExisteNodosHijo(Nodo) == true)
            {
                var NodoHijo = new NodoB<T>();
                NodoHijo = Nodo.Hijos[PosicionHijo(Nodo, valor)]; //BUSCA POSICION CORRESPONDIENTE
                Insertar(NodoHijo, valor);
                if (ExisteEspacio(Nodo) == false)
                {
                    Separar(Nodo);
                }
                return 0;
            }
            else
            {
                if (ExisteEspacio(Nodo) == false)
                {
                    Separar(Nodo);
                }
                return 0;
            }

            
        }


        public void CreandoNodo(NodoB<T> nuevo, NodoB<T> nodo)
        {
            nuevo.max = nodo.max;
            nuevo.min = nodo.min;
            nuevo.id = siguienteposicion;
            siguienteposicion++;
        }

        public void Separar(NodoB<T> Nodo)
        {
            NodoB<T> izq = new NodoB<T>();
            NodoB<T> padreAux = new NodoB<T>();
            NodoB<T> der = new NodoB<T>();

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
                ConectarPadreEHijos(Nodo.Padre, izq);
                ConectarPadreEHijos(Nodo.Padre, der);

                CreandoNodo(izq, Nodo);
                izq.id = Nodo.id;
                siguienteposicion--;
                CreandoNodo(der, izq);

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
                Nodo.Padre.Hijos.Sort((x, y) => x.Valores[0].CompareTo(y.Valores[0]));
                Nodo = null;
            }//Si es la raiz y aun caben valores en el nodo
            else if (Nodo.Padre == null && Nodo.Hijos.Count < 5)
            {
                CreandoNodo(izq, Nodo);
                CreandoNodo(der, izq);
                padreAux.Valores.Add(Nodo.Valores[Nodo.min]);
                ConectarPadreEHijos(Nodo, izq);
                ConectarPadreEHijos(Nodo, der);
                Nodo.Valores.Sort((x, y) => x.CompareTo(y));
                Nodo.Valores = padreAux.Valores;
            }//Si es raiz y no caben valores
            else if (Nodo.Padre == null && Nodo.Hijos.Count >= 5)
            {
                CreandoNodo(izq, Nodo);
                CreandoNodo(der, izq);

                Registro val = Nodo.Valores[Nodo.min];

                HijosDeHijos(Nodo, izq, 0, Nodo.min);
                HijosDeHijos(Nodo, der, Nodo.min + 1, Nodo.max + 1);

                Nodo.Hijos.Clear();
                ConectarPadreEHijos(Nodo, izq);
                ConectarPadreEHijos(Nodo, der);

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

        public void ConectarPadreEHijos(NodoB<T> Padre, NodoB<T> Hijo)
        {
            Padre.Hijos.Add(Hijo);
            Hijo.Padre = Padre;
        }

        public int PosicionHijo(NodoB<T> Nodo, Registro valor)
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

        public void AgregarYOrdenarNodo(Registro valor, NodoB<T> Nodo)
        {
            Nodo.Valores.Add(valor);
            Nodo.Valores.Sort((x, y) => x.CompareTo(y));
        }


        static Registro val;
        public Registro Busqueda(Registro valor, NodoB<T> Nodo)
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
            else if (BEncontrado == true)
            {
                return val;
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

        public int BusquedaIDIgual(Registro valor, NodoB<T> Nodo)
        {
            bool BEncontrado = false;
            foreach (var item in Nodo.Valores)
            {
                if (item.IDPrimaryKey.CompareTo(valor.IDPrimaryKey) == 0)
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
                return BusquedaIDIgual(valor, NodoHijo);
            }
            else if (BEncontrado == true)
            {
                return 1;
            }
            else if (Nodo.Hijos.Count == 0)
            {
                return 0;
            }
            else
            {
                return 0;
            }
        }

        public bool ExisteUnderFlow(NodoB<T> Nodo)
        {
            if (Nodo.Valores.Count < Nodo.min)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static int contador = 0;
        static NodoB<T> NodoAModificar = new NodoB<T>();
        static NodoB<T> NodoDar = new NodoB<T>();

        public void Eliminar(Registro valor, NodoB<T> Nodo)
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
                        indice = i;  //INDICE DE VALOR A SUSTITUIR
                        NodoAModificar = Nodo; //NODO A SUSTITUIR VALOR DE NODO MAS IZQUIERDO DEL HIJO DERECHO
                        contador++; //revisar para que se usa
                        NodoInicial = true;

                        NodoDar = MasIzquierdoDeDerecho(indice, Nodo, false);

                        NodoAModificar.Valores.Add(NodoDar.Valores[0]);
                        NodoAModificar.Valores.RemoveAt(indice);
                        NodoDar.Valores.RemoveAt(0);

                        NodoAModificar.Valores.Sort((x, y) => x.CompareTo(y));

                        if (ExisteUnderFlow(NodoDar) == true)
                        {
                            //Llamo a un metodo para ver si un hermano VECINO puede prestar
                            VerificarHermanos(NodoDar.Padre, NodoDar);
                        }
                    }
                    else
                    {
                        Nodo.Valores.RemoveAt(i);
                    }

                    if (NodoInicial == true)
                    {
                        if (Nodo.Valores.Count == 0)
                        {
                            Nodo = NodoAModificar;
                            NodoB<T> NodoAux = new NodoB<T>();
                            for (int j = 0; j < Nodo.Hijos.Count; j++)
                            {
                                for (int k = 0; k < Nodo.Hijos[i].Valores.Count; k++)
                                {
                                    NodoAux.Valores.Add(Nodo.Hijos[j].Valores[k]);
                                }
                            }
                            Nodo = null;
                            Raiz = NodoAux;
                        }
                    }

                    if (ExisteUnderFlow(Nodo) == true && Nodo.Padre != null)
                    {
                        //Llamo a un metodo para ver si un hermano VECINO puede prestar
                        VerificarHermanos(Nodo.Padre, Nodo);
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


        public void VerificarHermanos(NodoB<T> padre, NodoB<T> nodo)
        {

            NodoB<T> hermano = new NodoB<T>();
            int posicionvalorpadre = 0;
            for (int i = 0; i < padre.Hijos.Count; i++)
            {   //Si nodo es el primero y el hermano a la derecha puede prestar
                if (padre.Hijos[i] == nodo && i == 0 && padre.Hijos[i + 1].Valores.Count > padre.min)
                {
                    hermano = padre.Hijos[i + 1];
                    posicionvalorpadre = i;
                    break;
                }
                //Si nodo es el ultimo y el hermano a la izquierda puede prestar
                else if (padre.Hijos[i] == nodo && i == (padre.Hijos.Count - 1) && padre.Hijos[i - 1].Valores.Count > padre.min)
                {
                    hermano = padre.Hijos[i - 1];
                    posicionvalorpadre = i - 1;
                    break;
                }
                //Si el nodo se encuentra en medio, y el hermano a prestar es el de la izquierda
                else if (padre.Hijos[i] == nodo && i > 0 && i < padre.Hijos.Count - 1 && padre.Hijos[i - 1].Valores.Count > padre.min)
                {
                    hermano = padre.Hijos[i - 1];
                    posicionvalorpadre = i - 1;
                    break;
                }
                //Si el nodo se encuentra en medio, y el hermano a prestar es el de la derecha
                else if (padre.Hijos[i] == nodo && i > 0 && i < padre.Hijos.Count - 1 && padre.Hijos[i + 1].Valores.Count > padre.min)
                {
                    hermano = padre.Hijos[i + 1];
                    posicionvalorpadre = i;
                    break;
                }

            }

            //Si no encontro hermano para prestar valor
            if (hermano.Padre == null)
            {
                for (int i = 0; i < padre.Hijos.Count; i++)
                {
                    //SI NO SE PUEDEN PRESTAR VALORES SOLO PASA LA POSICION DEL PADRE
                    //Si el nodo que tiene underflow es el primero 
                    if (padre.Hijos[i] == nodo && i == 0)
                    {
                        posicionvalorpadre = i;
                        hermano = padre.Hijos[posicionvalorpadre + 1];
                        break;
                    }
                    //Si el nodo que tiene underflow es el ultimo
                    else if (padre.Hijos[i] == nodo && i == (padre.Hijos.Count - 1))
                    {
                        posicionvalorpadre = i - 1;
                        hermano = padre.Hijos[posicionvalorpadre];
                        break;
                    }
                    //Si el nodo que tiene underflow esta en medio
                    else if (padre.Hijos[i] == nodo && i > 0 && i < padre.Hijos.Count - 1)
                    {
                        posicionvalorpadre = i;
                        if (padre.Hijos[posicionvalorpadre + 1].Valores.Count > padre.Hijos[posicionvalorpadre - 1].Valores.Count)
                        {
                            hermano = padre.Hijos[posicionvalorpadre + 1];
                        }
                        else if (padre.Hijos[posicionvalorpadre + 1].Valores.Count < padre.Hijos[posicionvalorpadre - 1].Valores.Count)
                        {
                            hermano = padre.Hijos[posicionvalorpadre - 1];
                        }
                        else
                        {
                            hermano = padre.Hijos[posicionvalorpadre + 1];
                        }
                        break;
                    }
                }

                JuntarNodos(padre, posicionvalorpadre, nodo, hermano);
            }
            //Encontro valor y solo se traslada
            else
            {
                TrasladarValor(hermano, posicionvalorpadre, padre, nodo);
            }
        }

        public void TrasladarValor(NodoB<T> hijoEmisor, int posicionvalorpadre, NodoB<T> padre, NodoB<T> nodoReceptor)
        {

            //NODO HERMANO IZQUIERDO: Si debo de pasar el ultimo dato
            if (hijoEmisor.Valores[hijoEmisor.Valores.Count - 1].CompareTo(padre.Valores[posicionvalorpadre]) < 0) //Hijo a la izquierda
            {
                padre.Valores.Add(hijoEmisor.Valores[hijoEmisor.Valores.Count - 1]); //Subir valor
                hijoEmisor.Valores.Remove(hijoEmisor.Valores[hijoEmisor.Valores.Count - 1]); //quita valor que se paso a padre
                nodoReceptor.Valores.Add(padre.Valores[posicionvalorpadre]); //bajar valor de padre
                padre.Valores.Remove(padre.Valores[posicionvalorpadre]);
                nodoReceptor.Valores.Sort((x, y) => x.CompareTo(y)); // ordenar hijo que recibe

                //hijoEmisor.Valores.RemoveAt(hijoEmisor.Valores.Count - 1);

                padre.Valores.Sort((x, y) => x.CompareTo(y));
            }

            //HIJO A LA DERECHA: Si debo de pasar el primer dato
            else if (hijoEmisor.Valores[0].CompareTo(padre.Valores[posicionvalorpadre]) > 0)
            {
                padre.Valores.Add(hijoEmisor.Valores[0]);
                nodoReceptor.Valores.Add(padre.Valores[posicionvalorpadre]);
                nodoReceptor.Valores.Sort((x, y) => x.CompareTo(y));

                hijoEmisor.Valores.Remove(hijoEmisor.Valores[0]);

                padre.Valores.Remove(padre.Valores[posicionvalorpadre]);

                padre.Valores.Sort((x, y) => x.CompareTo(y));
            }

        }

        public void JuntarNodos(NodoB<T> padre, int posicionvalorpadre, NodoB<T> hijo, NodoB<T> hermano)
        {

            if (hijo.Valores[0].CompareTo(padre.Valores[padre.Valores.Count - 1]) > 0) //HIJO A LA DERECHA: Si es el ultimo nodo
            {
                hermano = padre.Hijos[posicionvalorpadre];
                for (int i = 0; i < hermano.Valores.Count; i++)
                {
                    hijo.Valores.Add(hermano.Valores[i]);
                }

                if (ExisteNodosHijo(hijo) == true)
                {
                    for (int i = 0; i < hermano.Hijos.Count; i++)
                    {
                        hijo.Hijos.Add(hermano.Hijos[i]);
                    }
                }

                padre.Hijos.RemoveAt(posicionvalorpadre); //Elimina el nodo hermano
                hijo.Valores.Add(padre.Valores[posicionvalorpadre]); //Manda valor de raiz
                hijo.Valores.Sort((x, y) => x.CompareTo(y));

                padre.Valores.Remove(padre.Valores[posicionvalorpadre]);

                if (padre.Valores.Count < padre.min)
                {
                    VerificarHermanos(padre.Padre, padre);
                }
            }

            else if (hijo.Valores[hijo.Valores.Count - 1].CompareTo(padre.Valores[0]) < 0) //HIJO A LA IZQUIERDA DEL PADRE
            {
                hermano = padre.Hijos[posicionvalorpadre + 1];
                for (int i = 0; i < hermano.Valores.Count; i++)
                {
                    hijo.Valores.Add(hermano.Valores[i]);
                }

                if (ExisteNodosHijo(hijo) == true)
                {
                    for (int i = 0; i < hermano.Hijos.Count; i++)
                    {
                        hijo.Hijos.Add(hermano.Hijos[i]);
                    }
                }

                padre.Hijos.RemoveAt(posicionvalorpadre + 1); //Elimina el nodo hermano
                hijo.Valores.Add(padre.Valores[0]); //Manda valor de raiz
                hijo.Valores.Sort((x, y) => x.CompareTo(y));

                padre.Valores.Remove(padre.Valores[0]);

                if (padre.Valores.Count < padre.min && padre.Padre != null)
                {
                    VerificarHermanos(padre.Padre, padre);
                }
            }

            else if (hijo.Valores[0].CompareTo(padre.Valores[0]) > 0 && hijo.Valores[hijo.Valores.Count - 1].CompareTo(padre.Valores[padre.Valores.Count - 1]) < 0) //Si es uno de enmedio
            {
                if (hijo.Valores[0].CompareTo(padre.Valores[posicionvalorpadre]) > 0)
                {
                    hermano = padre.Hijos[posicionvalorpadre];
                    for (int i = 0; i < hermano.Valores.Count; i++)
                    {
                        hijo.Valores.Add(hermano.Valores[i]);
                    }

                    if (ExisteNodosHijo(hijo) == true)
                    {
                        for (int i = 0; i < hermano.Hijos.Count; i++)
                        {
                            hijo.Hijos.Add(hermano.Hijos[i]);
                        }
                    }

                    padre.Hijos.RemoveAt(posicionvalorpadre); //Elimina el nodo hermano
                    hijo.Valores.Add(padre.Valores[padre.Valores.Count - 1]); //Manda valor de raiz
                    hijo.Valores.Sort((x, y) => x.CompareTo(y));
                }
                else
                {
                    hermano = padre.Hijos[posicionvalorpadre + 1];
                    for (int i = 0; i < hermano.Valores.Count; i++)
                    {
                        hijo.Valores.Add(hermano.Valores[i]);
                    }

                    if (ExisteNodosHijo(hijo) == true)
                    {
                        for (int i = 0; i < hermano.Hijos.Count; i++)
                        {
                            hijo.Hijos.Add(hermano.Hijos[i]);
                        }
                    }

                    padre.Hijos.RemoveAt(posicionvalorpadre + 1); //Elimina el nodo hermano
                    hijo.Valores.Add(padre.Valores[posicionvalorpadre]); //Manda valor de raiz
                    padre.Valores.RemoveAt(posicionvalorpadre);
                    hijo.Valores.Sort((x, y) => x.CompareTo(y));
                }





            }

        }

        public NodoB<T> MasIzquierdoDeDerecho(int IndiceValor, NodoB<T> Nodo, bool IrDerecha)
        {
            if (IrDerecha == false && Nodo.Hijos.Count > 0)
            {
                IrDerecha = true;
                return MasIzquierdoDeDerecho(IndiceValor, Nodo.Hijos[IndiceValor + 1], IrDerecha);
            }
            else if (IrDerecha == true && Nodo.Hijos.Count > 0)
            {
                return MasIzquierdoDeDerecho(IndiceValor, Nodo.Hijos[0], IrDerecha);
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
