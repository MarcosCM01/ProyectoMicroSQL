using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProyectoMicroSQL.Singleton;
using Newtonsoft.Json;

namespace ProyectoMicroSQL.Controllers
{
    public class HomeController : Controller
    {
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

        public ActionResult MostrarTablas()
        {
            return View();
        }

        public ActionResult Grid()
        {
            string llave = Data.Instancia.nombreTabla;
            Data.Instancia.Arboles[llave].ExistenElementosEnLista();
            Data.Instancia.Arboles[llave].AlmacenandoNodosEnLista(Data.Instancia.Arboles[llave].Raiz);
            Data.Instancia.listaNodos = Data.Instancia.Arboles[llave].RetornandoListaNodos();
            return View(Data.Instancia.listaNodos);
        }

        public ActionResult VerTablas()
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
            string[] lineas2 = codigo.Trim().Split('\n');
            List<string> lineas = new List<string>();

            SeparandoLineas(lineas, lineas2);

            List<string> Bloques = new List<string>();
            int cantidadBloques = 0;

            if (lineas.Contains("GO") || lineas.Contains("GO\r") && (lineas.Last() == "GO" || lineas.Last() == "GO\r"))
            {
                Bloques = SeparandoInstrucciones(codigo);
                cantidadBloques = Bloques.Count;
            }
            
            else
            {
                cantidadBloques = 1;
            }

            string Key;
            if (lineas.Last() != "GO" && lineas.Last() != "GO\r")
            {
                ViewBag.MensajeError = "Falta de 'GO' al finalizar instrucciones por bloque";
            }
            else if(ViewBag.MensajeError = "ERROR DE SINTAXIS")
            {
                try
                {
                    for (int i = 0; i < cantidadBloques; i++)
                    {
                        string[] lineas1 = Bloques[i].Split('\r');
                        lineas.Clear();
                        SeparandoLineas(lineas, lineas1);


                        if (Data.Instancia.PalabrasReservadas.ContainsValue(lineas[0]) == true)
                        {
                            Key = lineas[0];

                            foreach (var item in Data.Instancia.PalabrasReservadas)
                            {
                                if (item.Value == Key)
                                {
                                    Instrucciones(item.Key, lineas);
                                    break;
                                }
                            }
                        }
                        else if (Data.Instancia.PalabrasReservadas.ContainsKey(lineas[0]) == true)
                        {
                            Key = lineas[0];
                            Instrucciones(Key, lineas);
                        }
                        else
                        {
                            ViewBag.MensajeError = "Se escribio incorrectamente un comando";
                        }
                    }
                }
                catch
                {
                    ViewBag.MensajeError = "Se escribio incorrectamente un comando o falto comando para finalizar 'GO'";
                }
                
            }
            return View();
        }

        public List<string> SeparandoInstrucciones(string codigo)
        {
            int contador = 0;
            int contadorGO = 0;
            string[] CodigoSeparado = codigo.Trim().Split('\n');
            List<string> bloquesDeInstrucciones = new List<string>();
            for (int i = 0; i < CodigoSeparado.Length; i++)
            {
                if(CodigoSeparado[i] == "GO" || CodigoSeparado[i] == "GO\r")
                {
                    contadorGO++;
                }
            }
            if(contador > 1)
            {
                contadorGO--;
            }

            for (int i = 0; i < contadorGO; i++)
            {
                string bloque = "";
                do
                {
                    bloque += CodigoSeparado[contador];
                    contador++;
                } while (CodigoSeparado[contador] != "GO" && CodigoSeparado[contador] != "GO\r");
                bloque += CodigoSeparado[contador];
                contador++;
                bloquesDeInstrucciones.Add(bloque);
            }

            return bloquesDeInstrucciones;
        }

        public void SeparandoLineas(List<string> lista, string[] lineas)
        {
            foreach (var linea in lineas)
            {
                if(linea != "")
                {
                    lista.Add(linea);
                }
            }
        }


        public void ContadorDeInstrucciones(List<string> instrucciones, int posicioninicial, List<int> bloque)
        {
            for (int i = posicioninicial; i < instrucciones.Count; i++)
            {
                if (instrucciones[i] == "(")
                {
                    bloque.Add(i);
                }
                else if (instrucciones[i] == ")")
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

        public void Instrucciones(string Key, List<string> instrucciones)
        {
            string nombreTabla = "";
            if (instrucciones[instrucciones.Count - 1] != "GO" && instrucciones[instrucciones.Count - 1] != "GO\r")
            {
                ViewBag.ErrorCodigo = "Sintaxis incorrecta: 'GO' omitido al final";
            }
            else
            {
                try
                {
                    nombreTabla = instrucciones[1];
                    Data.Instancia.nombreTabla = nombreTabla;
                    switch (Key)
                    {
                        case "CREATE TABLE":
                            CrearTabla(nombreTabla, instrucciones);
                            break;
                        case "SELECT":

                            break;
                        case "FROM":

                            break;
                        case "DELETE FROM":
                            EliminarEnTabla(nombreTabla, instrucciones);
                            break;
                        case "DROP TABLE":
                            DropTable(nombreTabla);
                            break;
                        case "INSERT INTO":
                            InsertarEnTabla(nombreTabla, instrucciones);
                            break;
                        default:
                            ViewBag.MensajeError = "Palabra reservada no reconocible";
                            break;
                    }
                }
                catch
                {
                    ViewBag.MensajeError = "ERROR DE SINTAXIS";
                }

            }

        }

        public void CrearTabla(string nombreTabla, List<string> instrucciones)
        {
            Estructuras_de_Datos.ArbolB<Estructuras_de_Datos.Registro> arbolB = new Estructuras_de_Datos.ArbolB<Estructuras_de_Datos.Registro>();
            Estructuras_de_Datos.ArbolBP<Estructuras_de_Datos.Registro> arbolBPlus = new Estructuras_de_Datos.ArbolBP<Estructuras_de_Datos.Registro>();
            if (Data.Instancia.Arboles.ContainsKey(nombreTabla))
            {
                ViewBag.MensajeError = "TABLA YA EXISTENTE";
            }
            else
            {
                if (instrucciones[2] == "(")
                {
                    Data.Instancia.NombresTabla.Add(nombreTabla);
                    Data.Instancia.Arboles.Add(nombreTabla, arbolB);
                    Data.Instancia.ArbolesBPlus.Add(nombreTabla, arbolBPlus);
                    Codigobloque1.Clear();
                    Codigobloque2.Clear();
                    ContadorDeInstrucciones(instrucciones, 0, Codigobloque1);
                    for (int i = Codigobloque1[0] + 1; i < Codigobloque1[1]; i++)
                    {
                        char[] Lineachar = instrucciones[i].ToCharArray();
                        string[] linea = new string[100];
                        if (Lineachar[Lineachar.Length - 1] == ',')
                        {
                            linea = instrucciones[i].Split(',');
                        }

                        linea = linea[0].Split(' ');
                        Data.Instancia.ListaVariables.Add(linea[0]);
                        Data.Instancia.Arboles[nombreTabla].ListaVariables.Add(linea[0]);
                        info.Variables.Clear();
                        info.ContadorChar = info.ContadorDT = info.ContadorChar = 0;
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
                    
                    ViewBag.MensajeError = nombreTabla + ": Creacion exitosa de la tabla";
                }
                else
                {
                    Data.Instancia.NombresTabla.Remove(nombreTabla);
                    Data.Instancia.Arboles.Remove(nombreTabla);
                    Data.Instancia.ArbolesBPlus.Remove(nombreTabla);

                    ViewBag.MensajeError = "Sintaxis incorrecta: '(' omitido al inicio";
                }
            }
        }

        public void InsertarEnTabla(string nombreTabla, List<string> instrucciones)
        {
            if (Data.Instancia.Arboles.ContainsKey(nombreTabla))
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
                    linea = instrucciones[i].Split(',');
                    VariablesAAgregar.Add(linea[0]);
                }
                for (int i = Codigobloque2[0] + 1; i < Codigobloque2[1]; i++)
                {
                    char[] Lineachar = instrucciones[i].ToCharArray();
                    string[] linea = new string[100];
                    linea = instrucciones[i].Split(',');
                    if (Lineachar.Contains('\''))
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
                    Estructuras_de_Datos.Registro Reg = new Estructuras_de_Datos.Registro();
                    for (int i = 0; i < VariablesAAgregar.Count; i++)
                    {
                        if (VariablesAAgregar.Count == DatosDeVariables.Count && info.Variables.ContainsKey(VariablesAAgregar[i]))
                        {
                            var LAux = info.Variables[VariablesAAgregar[i]];
                            LAux.Add(DatosDeVariables[i]);
                            Reg.Valores.Add(DatosDeVariables[i]);
                            Data.Instancia.reg.Valores.Add(DatosDeVariables[i]);
                        }
                        else
                        {
                            ViewBag.MensajeError = "Falto una variable o excedio la cantidad";
                            break;
                        }
                    }
                    Reg.IDPrimaryKey = int.Parse(DatosDeVariables[0]);
                    if (Data.Instancia.Arboles[nombreTabla].Insertar(Data.Instancia.Arboles[nombreTabla].Raiz, Reg) != 0)
                    {
                        ViewBag.MensajeError = nombreTabla + "ID de registro repetido, cambie el ID para su registro correcto";
                    }
                    Data.Instancia.ArbolesBPlus[nombreTabla].InsertarNodo(Reg);
                    ViewBag.MensajeError = nombreTabla + ": Insercion exitosa";

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
        }

        public void EliminarEnTabla(string nombreTabla, List<string> instrucciones)
        {
            Estructuras_de_Datos.Registro reg = new Estructuras_de_Datos.Registro();
            if (Data.Instancia.Arboles.ContainsKey(nombreTabla))
            {
                if (instrucciones.Count <= 3)
                {
                    Data.Instancia.Arboles[nombreTabla].EliminarTodo(Data.Instancia.Arboles[nombreTabla].Raiz);
                }
                else
                {
                    string[] separador = instrucciones[instrucciones.Count - 2].Split(' ');

                    var indice = int.Parse(separador[2]);
                    reg.IDPrimaryKey = indice;
                    Data.Instancia.Arboles[nombreTabla].Eliminar(reg, Data.Instancia.Arboles[nombreTabla].Raiz);
                    //Data.Instancia.ArbolesBPlus[nombreTabla].Eliminar(reg);
                }
            }
            else
            {
                ViewBag.MensajeError = "NO EXISTE DICHA TABLA";
            }
        }

        public void DropTable(string nombreTabla)
        {
            if (Data.Instancia.Arboles.ContainsKey(nombreTabla))
            {
                Data.Instancia.Arboles.Remove(nombreTabla);
                Data.Instancia.ArbolesBPlus.Remove(nombreTabla);
                ViewBag.MensajeError = nombreTabla + ": Eliminación de tabla correcta";
            }
            else
            {
                ViewBag.MensajeError = "NO EXISTE DICHA TABLA";
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
                string llave = collection["Llave"].Trim();
                string Valor = collection["NuevaPalabra"].Trim();
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


        public ActionResult Reestablecer()
        {
            Data.Instancia.Reestablecer();
            ViewBag.Mensaje = "Restablecimiento completado";
            return View("Menu");
        }
    }
}
