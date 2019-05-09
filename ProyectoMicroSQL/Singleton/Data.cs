using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Collections;
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
        public List<string> ListaV = new List<string>();
        public List<string> ListaK = new List<string>();
        public List<string> ListaVariables = new List<string>();


        public Dictionary<string, Estructuras_de_Datos.ArbolB<Estructuras_de_Datos.Registro>> Arboles = new Dictionary<string, Estructuras_de_Datos.ArbolB<Estructuras_de_Datos.Registro>>();
        public Dictionary<string, Estructuras_de_Datos.ArbolBP<Estructuras_de_Datos.Registro>> ArbolesBPlus = new Dictionary<string, Estructuras_de_Datos.ArbolBP<Estructuras_de_Datos.Registro>>();
        public Estructuras_de_Datos.Info Informacion = new Estructuras_de_Datos.Info();

        
        public void LecturaCSV(string path)
        {
            

            string[] lineas = File.ReadAllLines(path);
            var contador = 0;
            foreach (var item in lineas)
            {
                if (contador > 0)
                {
                    string[] infolinea = item.Split(';');

                    for (int i = 0; i < infolinea.Length; i++)
                    {
                        infolinea[i] = infolinea[i] + "\r";
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

        public void Reestablecer()
        {
            for (int i = 0; i < ListaV.Count; i++)
            {
                PalabrasReservadas[ListaK[i]] = ListaV[i];
            }
            
        }

    }
}
