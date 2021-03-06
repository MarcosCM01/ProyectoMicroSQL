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

        public string nombreTabla { get; set; }
        public List<string> NombresTabla = new List<string>();

        public Estructuras_de_Datos.Registro reg = new Estructuras_de_Datos.Registro();

        public List<Estructuras_de_Datos.NodoB<Estructuras_de_Datos.Registro>> listaNodos = new List<Estructuras_de_Datos.NodoB<Estructuras_de_Datos.Registro>>();

        public Dictionary<string, Estructuras_de_Datos.ArbolB<Estructuras_de_Datos.Registro>> Arboles = new Dictionary<string, Estructuras_de_Datos.ArbolB<Estructuras_de_Datos.Registro>>();
        public Dictionary<string, Estructuras_de_Datos.ArbolBP<Estructuras_de_Datos.Registro>> ArbolesBPlus = new Dictionary<string, Estructuras_de_Datos.ArbolBP<Estructuras_de_Datos.Registro>>();
        public Dictionary<string, Estructuras_de_Datos.Info> listaVariables = new Dictionary<string, Estructuras_de_Datos.Info>();

        public List<Estructuras_de_Datos.NodoB<Estructuras_de_Datos.Registro>> NodosAMostrar = new List<Estructuras_de_Datos.NodoB<Estructuras_de_Datos.Registro>>();

        public List<Estructuras_de_Datos.NodoB<Estructuras_de_Datos.Registro>> listaNodosFiltrados = new List<Estructuras_de_Datos.NodoB<Estructuras_de_Datos.Registro>>();
        public List<string> VariablesFiltradas = new List<string>();
        public List<Estructuras_de_Datos.Registro> RegistrosVariablesFiltradas = new List<Estructuras_de_Datos.Registro>();
        public List<Estructuras_de_Datos.Registro> RegistrosCompletos = new List<Estructuras_de_Datos.Registro>();
        public int IDEncontrado { get; set; }

        public int LecturaCSV(string path)
        {
            try
            {
                PalabrasReservadas.Clear();
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
            //string NodosAEscribir = JsonConvert.SerializeObject(listaNodos, Formatting.Indented);
            //using (var Writer = new StreamWriter("C:/microSQL/arbolesb/" + nombreTabla + ".arbolb"))
            //{
            //    Writer.WriteLine(NodosAEscribir);
            //}

            if (Nodo.Hijos.Count > 0)
            {
                EscribirArbol(Nodo.Hijos[0]);

                var NodoAEscribir = JsonConvert.SerializeObject(Nodo);
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
