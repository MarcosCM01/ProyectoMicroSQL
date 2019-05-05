using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProyectoMicroSQL.Singleton;
using ProyectoMicroSQL.Models;
using System;

namespace ProyectoMicroSQL.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CargarTablas()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CargarTablas(HttpPostedFileBase file)
        {
            return View();
        }

        public ActionResult CodigoSQL()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CodigoSQL(string codigo)
        {
            List<List<string>> bloques = new List<List<string>>();

            
            string[] lineas = codigo.Trim().Split('\n');
            string[] Aux = new string[lineas.Length];
            int contador = 0;
            List<string> ComandosAAgregar = new List<string>();

            var Key = "";
            if (lineas.Contains("GO\r") || lineas.Contains("GO"))
            {
                for (int i = 0; i < lineas.Length; i++)
                {
                    if (lineas[i] != "GO\r" && lineas[i] != "GO")
                    {
                        Aux[i] = lineas[i];
                    }
                    
                    else
                    {
                        foreach(var item in Aux)
                        {
                            ComandosAAgregar.Add(item);
                        }
                        
                        bloques.Add(ComandosAAgregar);
                        for (int j = 0; j < Aux.Length; j++)
                        {
                            Aux[j] = null;
                        }
                        contador++;
                        
                    }
                }

                foreach(var item in bloques)
                {
                    if(item[0] == "GO")
                    {
                        item.RemoveAt(0);
                    }
                }
                
            }
            
            if (Data.Instancia.PalabrasReservadas.ContainsValue(lineas[0]) == true)
            {
                Key = lineas[0];
                int indice = 0;
                for (int i = 0; i < Data.Instancia.ListaV.Count; i++)
                {
                    if(Data.Instancia.ListaV[i] == Key)
                    {
                        indice = i;
                        break;
                    }
                }
                Key = Data.Instancia.ListaK[indice];
                Instrucciones(Key, lineas);
            }
            else if (Data.Instancia.PalabrasReservadas.ContainsKey(lineas[0]) == true)
            {
                Key = lineas[0];
                Instrucciones(Key, lineas);
            }
            
            else
            {
                ViewBag.ErrorCodigo = "Se escribio incorrectamente un comando";
            }

            return View();
        }

        public void Separar(string[] instrucciones )
        {

        }
        public void Instrucciones(string Key, string[] instrucciones)
        {
            string nombreTabla = "";
            string[] variablesNuevas;
            string[] comandos;
            switch (Key)
            {
                case "CREATE TABLE\r":
                    Estructuras_de_Datos.ArbolB<Info> arbolB = new Estructuras_de_Datos.ArbolB<Info>();
                    nombreTabla = instrucciones[1];
                    if(instrucciones[2] == "(")
                    {
                        int contador = 2;
                        while(instrucciones[contador] != ")")
                        {
                            variablesNuevas = instrucciones[contador].Split(',');
                            foreach(var item in variablesNuevas)
                            {
                                comandos = item.Split(' ');
                            }
                        }
                    }
                    break;
                case "SELECT\r":

                    break;
                case "FROM\r":

                    break;
                case "DELETE\r":

                    break;
                case "WHERE\r":

                    break;
                case "DROP TABLE\r":

                    break;
                case "INSERT INTO\r":

                    break;
                case "VALUES\r":

                    break;
                case "GO\r":

                    break;
                default:
                    break;
            }
        }

        static int contador = 0;
        public ActionResult Carga()
        {
            if (contador > 0)
            {
                ViewBag.Msg = "ERROR AL CARGAR EL ARCHIVO, INTENTE DE NUEVO";
            }
            contador++;
            return View();
        }

        [HttpPost]
        public ActionResult Carga(HttpPostedFileBase file)
        {
            if (file != null)
            {
                Upload(file);
                return RedirectToAction("Upload");

            }
            else
            {
                ViewBag.Msg = "ERROR AL CARGAR EL ARCHIVO, INTENTE DE NUEVO";
                return View();
            }
        }

        public ActionResult Upload (HttpPostedFileBase file)
        {
            string model = "";
            if (file != null && file.ContentLength > 0)
            {
                model = Server.MapPath("~/Upload/") + file.FileName;
                file.SaveAs(model);
                Data.Instancia.LecturaCSV(model);
                ViewBag.Msg = "Carga del archivo correcta";
                return RedirectToAction("Menu"); //VERIFICAR
            }
            else
            {
                ViewBag.Msg = "ARCHIVO SIN CONTENIDO";
                return RedirectToAction("Carga");
            }
        }

        public ActionResult Menu()
        {
            ViewBag.Mensaje = "";
            return View();
        }

        public ActionResult Personalizar()
        {
            return View(Data.Instancia.PalabrasReservadas);
        }

        public ActionResult Modificar(string NewWord)
        {
            try
            {
                ViewBag.Error = "";
                string[] array = NewWord.Split(',');
                string key = array[0].Trim();
                string value = array[1].Trim();

                Data.Instancia.PalabrasReservadas[key] = value;

                return View("Personalizar");
            }
            catch
            {
                ViewBag.Error = "Mala escritura";
                return View("Personalizar");
            }
            
        }

        public ActionResult Reestablecer()
        {
            Data.Instancia.Reestablecer();
            ViewBag.Mensaje = "Restablecimiento completado";
            return View("Menu");
        }
    }
}