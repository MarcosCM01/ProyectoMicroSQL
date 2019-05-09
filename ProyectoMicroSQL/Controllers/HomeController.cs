using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProyectoMicroSQL.Singleton;

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

        public void ContadorDeInstrucciones(string[] instrucciones, int posicioninicial, List<int> bloque)
        {
            for (int i = posicioninicial; i < instrucciones.Length; i++)
            {
                if (instrucciones[i] == "(\r")
                {
                    bloque.Add(i);
                }
                else if (instrucciones[i] == ")\r")
                {
                    bloque.Add(i);
                    break;
                }
            }
        }


        static List<int> Codigobloque1 = new List<int>();
        static List<int> Codigobloque2 = new List<int>();

        public static Estructuras_de_Datos.Info info = new Estructuras_de_Datos.Info();

        public void Instrucciones(string Key, string[] instrucciones)
        {
            string nombreTabla = "";
            if(instrucciones[instrucciones.Length - 1] != "GO" && instrucciones[instrucciones.Length - 1] != "GO\r")
            {
                ViewBag.ErrorCodigo = "Sintaxis incorrecta: 'GO' omitido al final";
            }
            else
            {
                try
                {
                    //Elimina el \R  del nombre de la tabla
                    nombreTabla = instrucciones[1];
                    string[] aux = nombreTabla.Split('\r');
                    nombreTabla = aux[0];

                    switch (Key)
                    {
                        case "CREATE TABLE\r":
                            
                            Estructuras_de_Datos.ArbolB<Estructuras_de_Datos.Registro> arbolB = new Estructuras_de_Datos.ArbolB<Estructuras_de_Datos.Registro>();

                            if(Data.Instancia.Arboles.ContainsKey(nombreTabla))
                            {
                                ViewBag.MensajeError = "TABLA YA EXISTENTE";
                                break;
                            }
                            else
                            {
                                Data.Instancia.Arboles.Add(nombreTabla, arbolB);

                                if (instrucciones[2] == "(\r")
                                {
                                    ContadorDeInstrucciones(instrucciones, 0, Codigobloque1);
                                    for (int i = Codigobloque1[0] + 1; i < Codigobloque1[1]; i++)
                                    {
                                        char[] Lineachar = instrucciones[i].ToCharArray();
                                        string[] linea = new string[100];
                                        if (Lineachar[Lineachar.Length - 2] == ',')
                                        {
                                            linea = instrucciones[i].Split(',');
                                        }
                                        else
                                        {
                                            linea = instrucciones[i].Split('\r');
                                        }

                                        linea = linea[0].Split(' ');
                                        switch (linea[1])
                                        {
                                            case "INT":
                                                List<string> listaint = new List<string>();
                                                info.Variables.Add(linea[0], listaint);
                                                info.ContadorInt++;
                                                break;
                                            case "VARCHAR(100)":
                                                List<string> listachar = new List<string>();
                                                info.Variables.Add(linea[0], listachar);
                                                info.ContadorChar++;
                                                break;
                                            case "DATETIME":
                                                List<string> listaDT = new List<string>();
                                                info.Variables.Add(linea[0], listaDT);
                                                info.ContadorDT++;
                                                break;
                                            default:
                                                ViewBag.MensajeError = "TIPO DE DATO NO ADMITIDO";
                                                break;
                                        }
                                    }
                                    ViewBag.MensajeError = "Creacion exitosa de " + nombreTabla;
                                }
                                else
                                {
                                    ViewBag.MensajeError = "Sintaxis incorrecta: '(' omitido al inicio";
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
                            if(Data.Instancia.Arboles.ContainsKey(nombreTabla))
                            {
                                Codigobloque1.Clear();//EL QUE EL TIENE EL NOMBRE DE VARIBALES
                                Codigobloque2.Clear();//EL QUE TIENE LOS VALORES DE LAS VARIABLES
                                ContadorDeInstrucciones(instrucciones, 0, Codigobloque1);
                                ContadorDeInstrucciones(instrucciones, Codigobloque1[1] + 1, Codigobloque2);

                                List<string> VariablesAAgregar = new List<string>();
                                List<string> DatosDeVariables = new List<string>();
                                //Depuracion
                                for (int i = Codigobloque1[0] + 1; i < Codigobloque1[1]; i++)
                                {
                                    string[] linea = new string[100];
                                    linea = instrucciones[i].Split('\r');
                                    linea = linea[0].Split(',');
                                    VariablesAAgregar.Add(linea[0]);
                                }
                                for (int i = Codigobloque2[0] + 1; i < Codigobloque2[1]; i++)
                                {
                                    char[] Lineachar = instrucciones[i].ToCharArray();
                                    string[] linea = new string[100];
                                    linea = instrucciones[i].Split('\r');
                                    linea = linea[0].Split(',');
                                    if(Lineachar.Contains('\''))
                                    {
                                        linea = linea[0].Split('\'');
                                        DatosDeVariables.Add(linea[1]);
                                    }
                                    else
                                    {
                                        DatosDeVariables.Add(linea[0]);
                                    }
                                    
                                }
                                try
                                {
                                    Estructuras_de_Datos.Registro Reg = new Estructuras_de_Datos.Registro();
                                    for (int i = 0; i < VariablesAAgregar.Count; i++)
                                    {
                                        if (VariablesAAgregar.Count == DatosDeVariables.Count && info.Variables.ContainsKey(VariablesAAgregar[i]) && VariablesAAgregar.Count == info.Variables.Count)
                                        {
                                            var LAux = info.Variables[VariablesAAgregar[i]];
                                            LAux.Add(DatosDeVariables[i]);
                                            Reg.Valores.Add(DatosDeVariables[i]);
                                            if(i == 0) { Reg.Valores.Remove(Reg.Valores[i]); }
                                        }
                                        else
                                        {
                                            ViewBag.MensajeError = "Falto una variable o excedio la cantidad";
                                            break;
                                        }
                                    }
                                    Reg.IDPrimaryKey = int.Parse(DatosDeVariables[0]);
                                    Data.Instancia.Arboles[nombreTabla].Insertar(Data.Instancia.Arboles[nombreTabla].Raiz, Reg);
                                    ViewBag.MensajeError = "Insercion exitosa en " + nombreTabla;
                                }
                                catch
                                {
                                    ViewBag.MensajeError = "ERROR DE SINTAXIS";
                                }   

                            }
                            else
                            {
                                ViewBag.MensajeError = "NO EXISTE DICHA TABLA";
                            }
                            break;
                        case "VALUES\r":

                            break;
                        case "GO\r":

                            break;
                        default:
                            break;
                    }
                }
                catch
                {
                    ViewBag.MensajeError = "ERROR DE SINTAXIS";
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