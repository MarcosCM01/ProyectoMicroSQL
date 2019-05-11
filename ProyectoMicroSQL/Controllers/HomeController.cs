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
        static string diccionarioPredeterminado = "~/Upload/MicroSQL.init";

        public ActionResult CargarTablas()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CargarTablas(HttpPostedFileBase file)
        {
            ViewBag.Msg = "Archivo cargado con exito";
            return RedirectToAction("Index");
        }

        public ActionResult Grid()
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
        static List<int> ids = new List<int>();
        public static Estructuras_de_Datos.Info info = new Estructuras_de_Datos.Info();
        static string NTabla = "";
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
                    //Data.Instancia.NTabla = nombreTabla;
                    switch (Key)
                    {
                        case "CREATE TABLE\r":
                            
                            Estructuras_de_Datos.ArbolB<Estructuras_de_Datos.Registro> arbolB = new Estructuras_de_Datos.ArbolB<Estructuras_de_Datos.Registro>();
                                Estructuras_de_Datos.ArbolBP<Estructuras_de_Datos.Registro> arbolBPlus = new Estructuras_de_Datos.ArbolBP<Estructuras_de_Datos.Registro>();
                            if(Data.Instancia.Arboles.ContainsKey(nombreTabla))
                            {
                                ViewBag.MensajeError = "TABLA YA EXISTENTE";
                                break;
                            }
                            else
                            {
                                Data.Instancia.Arboles.Add(nombreTabla, arbolB);
                                Data.Instancia.ArbolesBPlus.Add(nombreTabla, arbolBPlus);

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
                                        Data.Instancia.ListaVariables.Add(linea[0]);
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
                        case "DELETE FROM\r":
                                Estructuras_de_Datos.Registro reg = new Estructuras_de_Datos.Registro();
                                if (Data.Instancia.Arboles.ContainsKey(nombreTabla))
                                {
                                    if (instrucciones.Length <= 3)
                                    {
                                        Data.Instancia.Arboles[nombreTabla].EliminarTodo(Data.Instancia.Arboles[nombreTabla].Raiz);
                                        //Data.Instancia.ArbolesBPlus[nombreTabla].EliminarTodo(Data.Instancia.ArbolesBPlus[nombreTabla].root);
                                        
                                    }
                                    else
                                    {

                                        string[] separador = instrucciones[instrucciones.Length - 2].Split(' ');
                                        
                                        var indice = int.Parse(separador[2]);
                                        reg.IDPrimaryKey = indice;
                                        Data.Instancia.Arboles[nombreTabla].Eliminar(reg, Data.Instancia.Arboles[nombreTabla].Raiz);
                                        Data.Instancia.ArbolesBPlus[nombreTabla].Eliminar(reg);
                                    }
                                }
                                else
                                {
                                    ViewBag.MensajeError = "NO EXISTE DICHA TABLA";
                                }
                                
                            
                            break;
                        
                        case "DROP TABLE\r":
                            {
                                if (Data.Instancia.Arboles.ContainsKey(nombreTabla) && Data.Instancia.ArbolesBPlus.ContainsKey(nombreTabla))
                                {
                                    Data.Instancia.Arboles.Remove(nombreTabla);
                                    Data.Instancia.ArbolesBPlus.Remove(nombreTabla);
                                }
                                else
                                {
                                    ViewBag.MensajeError = "NO EXISTE DICHA TABLA";
                                }
                            }
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
                                    int idRepetido = int.Parse(DatosDeVariables[0]);
                                    Estructuras_de_Datos.Registro registroID = new Estructuras_de_Datos.Registro();
                                    registroID.IDPrimaryKey = idRepetido;
                                    if (Data.Instancia.Arboles[nombreTabla].BusquedaIDIgual(registroID, Data.Instancia.Arboles[nombreTabla].Raiz) == 0)
                                    {
                                        ViewBag.MensajeError = "ID de registro repetido, cambie el ID para su registro correcto";
                                        break;
                                    }else
                                    {
                                        Estructuras_de_Datos.Registro Reg = new Estructuras_de_Datos.Registro();
                                        for (int i = 0; i < VariablesAAgregar.Count; i++)
                                        {
                                            if (VariablesAAgregar.Count == DatosDeVariables.Count && info.Variables.ContainsKey(VariablesAAgregar[i]) && VariablesAAgregar.Count == info.Variables.Count)
                                            {
                                                var LAux = info.Variables[VariablesAAgregar[i]];
                                                LAux.Add(DatosDeVariables[i]);
                                                Reg.Valores.Add(DatosDeVariables[i]);
                                                Data.Instancia.reg.Valores.Add(DatosDeVariables[i]);
                                                if (i == 0) { Reg.Valores.Remove(Reg.Valores[i]); }
                                            }
                                            else
                                            {
                                                ViewBag.MensajeError = "Falto una variable o excedio la cantidad";
                                                break;
                                            }
                                        }
                                        Reg.IDPrimaryKey = int.Parse(DatosDeVariables[0]);
                                        Data.Instancia.Arboles[nombreTabla].Insertar(Data.Instancia.Arboles[nombreTabla].Raiz, Reg);
                                        Data.Instancia.ArbolesBPlus[nombreTabla].InsertarNodo(Reg);
                                        //Data.Instancia.Arboles[nombreTabla].EliminarTodo(Data.Instancia.Arboles[nombreTabla].Raiz);
                                        //Data.Instancia.Arboles[nombreTabla].EscrituraTXT(Data.Instancia.Arboles[nombreTabla].ListaAux);
                                        ViewBag.MensajeError = "Insercion exitosa en " + nombreTabla;
                                    }
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
            try
            {
                ViewBag.Msg = "Carga exitosa";
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
            catch
            {
                return ViewBag.Msg = "ERROR AL CARGAR EL ARCHIVO, INTENTE DE NUEVO";
            }
            
        }

        
        public ActionResult Upload (HttpPostedFileBase file)
        {
            string model = "";
            if (file != null && file.ContentLength > 0)
            {
                model = Server.MapPath("~/Upload/") + file.FileName + ".ini";
                file.SaveAs(model);
                if(Data.Instancia.LecturaCSV(model) == 1)
                {
                    ViewBag.Msg = "Carga del archivo correcta";
                    return RedirectToAction("Menu");
                }
                else
                {
                    ViewBag.Msg = "Carga del archivo incorrecta";
                    return View("Carga");
                }
                
                 //VERIFICAR
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
            if(Data.Instancia.PalabrasReservadas.Count == 0)
            {
                ViewBag.Msg = "No se ha cargado diccionario aun";
                return RedirectToAction("Menu");
            }
            else
            {
                return View(Data.Instancia.PalabrasReservadas);
            }
        }

        [HttpPost]
        public ActionResult Personalizar(FormCollection collection)
        {
            try
            {
                string llave = collection["Llave"].Trim() + "\r";
                string Valor = collection["NuevaPalabra"].Trim() + "\r";
                if (Data.Instancia.PalabrasReservadas.ContainsKey(llave))
                {
                    Data.Instancia.PalabrasReservadas[llave] = Valor;
                    ViewBag.Error = "Modificacion correcta";
                }
                else
                {
                    ViewBag.Error = "Palabra reservada no encontrada, revise de nuevo";
                }
                return View(Data.Instancia.PalabrasReservadas);
            }
            catch
            {
                ViewBag.Error = "ERROR";
                return View();
            }
        }



        public string EliminandoEnter(string llave)
        {
            string[] llaveDepurada = llave.Split('\r');
            return llaveDepurada[0].Trim();
        }

        public ActionResult Reestablecer()
        {
            Data.Instancia.Reestablecer();
            ViewBag.Mensaje = "Restablecimiento completado";
            return View("Menu");
        }
    }
}
