using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using ProyectoMicroSQL.Models;

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

        public Dictionary<string, List<string>> Dictionary = new Dictionary<string, List<string>>();
        public List<string> auxDictionaries = new List<string>();

        public void LecturaCSV(string path)
        {
            string[] lineas = File.ReadAllLines(path);
            var contador = 0;
            foreach (var item in lineas)
            {
                if (contador > 0)
                {
                    string[] infolinea = item.Split(';');
                    for (int i = 1; i < infolinea.Length; i++)
                    {
                        auxDictionaries.Add(infolinea[i]);
                    }
                    Dictionary.Add(infolinea[0], auxDictionaries);
                }
                else
                {
                    contador++;
                }
            }
        }
    }
}