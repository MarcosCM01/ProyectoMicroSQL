using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProyectoMicroSQL.Singleton;
using ProyectoMicroSQL.Models;

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
            return RedirectToAction("Index");
        }

        public ActionResult CodigoSQL()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CodigoSQL(string codigo)
        {
            
            string[] lineas = codigo.Trim().Split('\n');
            int contador = 0;
            List<string> ComandosAAgregar = new List<string>();
            string Key;
            if (Data.Instancia.PalabrasReservadas.ContainsValue(lineas[0]) == true)
            {
                Key = lineas[0];
                int indice = 0;
                for (int i = 0; i < Data.Instancia.ListaV.Count; i++)
                {
                    if (Data.Instancia.ListaV[i] == Key)
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
            if(instrucciones[instrucciones.Length - 1] != "GO" && instrucciones[instrucciones.Length - 1] != "GO\r")
            {
                ViewBag.ErrorCodigo = "Sintaxis incorrecta: 'GO' omitido al final";
            }
            else
            {
                switch (Key)
                {
                    case "CREATE TABLE\r":
                        Estructuras_de_Datos.ArbolB<Info> arbolB = new Estructuras_de_Datos.ArbolB<Info>();
                        nombreTabla = instrucciones[1];
                        string[] aux = nombreTabla.Split('\r');
                        nombreTabla = aux[0];

                        Data.Instancia.Arboles.Add(nombreTabla, arbolB);

                        if (instrucciones[2] == "(\r")
                        {
                            int contador = 3;
                            while (instrucciones[contador] != ")\r")
                            {
                                variablesNuevas = instrucciones[contador].Split(',');
                                if(variablesNuevas[0] == "ID INT PRIMARY KEY")
                                {
                                    comandos = variablesNuevas[0].Split(' ');
                                    Info informacion = new Info();

                                    informacion.id = int.Parse(comandos[0]);
                                }
                                else
                                {
                                    
                                }
                                
                                 
                                
                                contador++;
                            }
                        }
                        else
                        {
                            ViewBag.ErrorCodigo = "Sintaxis incorrecta: '(' omitido al inicio";
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