using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Collections;
using Newtonsoft.Json;

namespace ProyectoMicroSQL.Singleton
{
    public class Data
    {
        private static Data instancia = null;
        public static Data Instancia
        {
            get
            {
                if (instancia == null)
                {
                    instancia = new Data();
                }
                return instancia;
            }
        }

        public Dictionary<string, string> PalabrasReservadas = new Dictionary<string, string>();
        public Dictionary<string, string> PalabrasReservadasPredeterminadas = new Dictionary<string, string>();

        public List<string> ListaV = new List<string>();
        public List<string> ListaK = new List<string>();

        public string nombreTabla { get; set; }
        public List<string> NombresTabla = new List<string>();

        public Estructuras_de_Datos.Registro reg = new Estructuras_de_Datos.Registro();

        public List<Estructuras_de_Datos.NodoB<Estructuras_de_Datos.Registro>> listaNodos = new List<Estructuras_de_Datos.NodoB<Estructuras_de_Datos.Registro>>();

        public Dictionary<string, Estructuras_de_Datos.ArbolB<Estructuras_de_Datos.Registro>> Arboles = new Dictionary<string, Estructuras_de_Datos.ArbolB<Estructuras_de_Datos.Registro>>();
        public Dictionary<string, Estructuras_de_Datos.ArbolBP<Estructuras_de_Datos.Registro>> ArbolesBPlus = new Dictionary<string, Estructuras_de_Datos.ArbolBP<Estructuras_de_Datos.Registro>>();
        public Estructuras_de_Datos.Info Informacion = new Estructuras_de_Datos.Info();

        public List<Estructuras_de_Datos.NodoB<Estructuras_de_Datos.Registro>> NodosAMostrar = new List<Estructuras_de_Datos.NodoB<Estructuras_de_Datos.Registro>>();

        public int LecturaCSV(string path)
        {
            try
            {
                PalabrasReservadas.Clear();
                ListaV.Clear();
                ListaK.Clear();
                string[] lineas = File.ReadAllLines(path);
                var contador = 0;
                foreach (var item in lineas)
                {
                    if (contador > 0)
                    {
                        string[] infolinea = item.Split(';');

                        for (int i = 0; i < infolinea.Length; i++)
                        {
                            infolinea[i] = infolinea[i];
                        }

                        PalabrasReservadas.Add(infolinea[0], infolinea[1]);
                        ListaK.Add(infolinea[0]);
                        ListaV.Add(infolinea[1]);
                    }
                    else
                    {
                        contador++;
                    }
                }
                return 1;
            }
            catch
            {
                return 0;
            }

        }

        public void EscribirArbol(Estructuras_de_Datos.NodoB<Estructuras_de_Datos.Registro> Nodo)
        {
            
                if (Nodo.Hijos.Count > 0)
                {
                    EscribirArbol(Nodo.Hijos[0]);

                    string NodoAEscribir = JsonConvert.SerializeObject(Nodo, Formatting.Indented);
                    //var NodoAEscribir = JsonConvert.SerializeObject(Nodo);
                    using (var Writer = new StreamWriter("C:/microSQL/arbolesb/" + nombreTabla + ".arbolb"))
                    {
                        Writer.WriteLine(NodoAEscribir);
                    }

                    for (int i = 1; i < Nodo.Hijos.Count; i++)
                    {
                        using (var Writer = new StreamWriter("C:/microSQL/arbolesb/" + nombreTabla + ".arbolb"))
                        {
                            Writer.WriteLine(NodoAEscribir);
                        }
                    }
                }
                else
                {
                    string NodoAEscribir = JsonConvert.SerializeObject(Nodo, Formatting.Indented);
                    using (var Writer = new StreamWriter("C:/microSQL/arbolesb/" + nombreTabla + ".arbolb"))
                    {
                        Writer.WriteLine(NodoAEscribir);
                    }
                }
            
            
        }
        public void LecturaTablas(string path)
        {
            string[] lineas = File.ReadAllLines(path);
            string[] linea;
            foreach (var item in lineas)
            {
                linea = item.Split('|');

            }
        }

    }
}
