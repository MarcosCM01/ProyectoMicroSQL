using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estructuras_de_Datos
{
    public class ArbolBP<T> : IEnumerable<T> where T : IComparable
    {
        #region inicializacionBTree+

        public NodoBP<T> root { get; set; }
        public int siguientePosicion { get; set; }
        public ArbolBP()
        {
            root = null;
            siguientePosicion = 2;

        }
        #endregion

        #region Insercion
        public void InsertarNodo(T value)
        {
            if (root == null)
            {
                root = new NodoBP<T>();
                root.AsignarGrado(root, 5);
                root.values.Add(value);
                root.id = 1;
                //newNodo = root;
            }
            else { Insertar2(root, value); }

        }
        public void Insertar2(NodoBP<T> nodo, T value)
        {
            if (EsHoja(nodo) == true) //Es hoja
            {
                AgregarYOrdenarN(nodo, value);
                //METODO DE ORDENAR
            }
            else if (EsHoja(nodo) == false) //No es hoja
            {
                var NodeSun = new NodoBP<T>();
                NodeSun = root.hijos[HijoAEntrar(root, value)]; //HIJO A DONDE INSERTAR
                Insertar2(NodeSun, value); //RECURSIVIDAD
                //METODO DE BUSCAR POSICION CORRESPONDIENTE
            }
            if (ExisteEspacio(nodo) == false)
            {
                SepararNodo(nodo);
                //METODO DE SEPARAR 
            }
        }

        public bool EsHoja(NodoBP<T> nodo) //Metodo para ver que no tenga hijos
        {
            if (nodo.hijos.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool ExisteEspacio(NodoBP<T> node) //Verifica que pueda meterse en el nodo
        {
            if (node.values.Count <= node.max)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void AgregarYOrdenarN(NodoBP<T> node, T value)
        {
            node.values.Add(value);
            node.values.Sort((x, y) => x.CompareTo(y));
        }
        public int HijoAEntrar(NodoBP<T> node, T valor)
        {
            if (node.values.Count == 1)
            {
                if (valor.CompareTo(node.values[0]) < 0)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            for (int i = 0; i < node.values.Count - 1; i++)
            {
                if (valor.CompareTo(node.values[i]) < 0)
                {
                    return i;
                }
                else if (valor.CompareTo(node.values[i]) > 0 && (valor.CompareTo(node.values[i + 1]) < 0))
                {
                    return i + 1;
                }
            }
            return node.values.Count;
        }
        //TODO ESTO SUCEDE EN UNA HOJA
        public void SepararNodo(NodoBP<T> node)
        {
            NodoBP<T> izq = new NodoBP<T>();
            NodoBP<T> padreAux = new NodoBP<T>();//AUXILIAR para el intercambio de datos
            NodoBP<T> der = new NodoBP<T>();

            for (int i = 0; i < node.min; i++)//Agrego los menores
            {
                izq.values.Add(node.values[i]);
            }
            for (int i = node.min; i <= node.max; i++)//Agrego los mayores
            {
                der.values.Add(node.values[i]);
            }

            if (node.padre != null)
            {
                RelacionPadreHijo(node.padre, izq);
                RelacionPadreHijo(node.padre, der);

                Asignacion(node, izq);
                izq.id = node.id;
                siguientePosicion--;
                Asignacion(node, der);
                node.padre.values.Add(node.values[node.min]);
                node.padre.values.Sort((x, y) => x.CompareTo(y));

                int ind = 0;
                for (int i = 0; i < node.padre.hijos.Count; i++)
                {
                    if (node.padre.hijos[i].values.Count > 4)
                    {
                        ind = i;
                        break;
                    }
                }

                if (node.hijos.Count > 0)
                {
                    NuevosHijos(node, izq, 0, node.min);
                    NuevosHijos(node, der, node.min + 1, node.max + 1);
                }
                node.padre.hijos.RemoveAt(ind);
                node.padre.hijos.Sort((x, y) => x.values[0].CompareTo(y.values[0]));
                if (node.padre.hijos.Count > 0)
                {
                    AsignarHermano(node.padre);
                }
                node = null;

            }
            else if (node.padre == null && node.hijos.Count < 5)
            {
                Asignacion(node, izq);
                Asignacion(izq, der);
                padreAux.values.Add(node.values[node.min]); //SUBE LA MEDIANA
                RelacionPadreHijo(node, izq);
                RelacionPadreHijo(node, der);
                node.values.Sort((x, y) => x.CompareTo(y));
                node.values = padreAux.values; //SE LIMPIA EL NODO
                AsignarHermano(node);
            }
            else if (node.padre == null && node.hijos.Count >= 5)
            {
                Asignacion(node, izq);
                Asignacion(izq, der);
                T value = node.values[node.min];
                NuevosHijos(node, izq, 0, node.min);
                NuevosHijos(node, der, node.min + 1, node.max + 1);
                der.values.RemoveAt(0);//Para que no se triplique el valor que sube como raiz

                node.hijos.Clear();
                RelacionPadreHijo(node, izq);
                RelacionPadreHijo(node, der);

                node.values.Clear();
                node.values.Add(value);
            }

        }
        #endregion

        #region MetodosAuxInsercion
        public void Asignacion(NodoBP<T> original, NodoBP<T> aux)
        {
            aux.max = original.max;
            aux.min = original.min;
            aux.id = siguientePosicion;
            siguientePosicion++;
        }

        public void AsignarHermano(NodoBP<T> nodo)
        {
            for (int i = 0; i < nodo.hijos.Count - 1; i++)
            {
                nodo.hijos[i].hermano = nodo.hijos[i + 1];
            }
        }

        public void RelacionPadreHijo(NodoBP<T> padre, NodoBP<T> hijo)
        {
            padre.hijos.Add(hijo);
            hijo.padre = padre;
        }

        public void NuevosHijos(NodoBP<T> padre, NodoBP<T> hijo, int inicio, int final)
        {
            for (int i = inicio; i <= final; i++)
            {
                hijo.hijos.Add(padre.hijos[i]);
            }
            foreach (var item in hijo.hijos)
            {
                item.padre = hijo;
            }
        }
        #endregion

        #region Busqueda
        //INDICAR EN EL CONTROLADOR QUE SI ES FALSO, INDIQUE QUE NO SE ENCONTRO EL DATO
        public void busquedaEnRaiz(T valor)
        {
            busquedaEnHojas(valor, root);

        }
        static T val;
        public object busquedaEnHojas(T value, NodoBP<T> node)
        {
            bool encontrado = false;
            foreach (var item in node.values)
            {
                if (item.CompareTo(value) == 0 && node.hijos.Count > 0)//PARA QUE LO ENCUENTRE EN LAS HOJAS
                {
                    val = item;
                    encontrado = true;
                    break;
                }
            }

            if (encontrado == false && node.values.Count > 0)
            {
                NodoBP<T> nodoAux = new NodoBP<T>();
                nodoAux = node.hijos[HijoAEntrar(node, value)];
                return busquedaEnHojas(value, nodoAux);
            }
            else if (encontrado == true)
            {
                return val;
            }
            else if (encontrado == false && node.hijos.Count == 0)
            {
                return "El dato que busca no se encuentra";
            }
            else
            {
                return val;
            }

        }
        #endregion


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
