using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

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

        public Dictionary<string, string> Dictionary = new Dictionary<string, string>();
        public List<string> ListaV = new List<string>();
        public List<string> ListaK = new List<string>();

        public void LecturaCSV(string path)
        {
            string[] lineas = File.ReadAllLines(path);
            var contador = 0;
            foreach (var item in lineas)
            {
                if (contador > 0)
                {
                    string[] infolinea = item.Split(';');
                    
                    Dictionary.Add(infolinea[0], infolinea[1]);
                    ListaK.Add(infolinea[0]);
                    ListaV.Add(infolinea[1]);
                }
                else
                {
                    contador++;
                }
            }
        }
        public void Reestablecer()
        {
            for (int i = 0; i < ListaV.Count; i++)
            {
                Dictionary[ListaK[i]] = ListaV[i];
              
            }
            
        }
    }
}
