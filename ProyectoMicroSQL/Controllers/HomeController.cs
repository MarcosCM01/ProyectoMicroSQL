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
        static List<int> Codigobloque1 = new List<int>();
        static List<int> Codigobloque2 = new List<int>();
        static List<int> ids = new List<int>();
        //public static Estructuras_de_Datos.Info info = new Estructuras_de_Datos.Info();
        static bool ExisteError = false;
        static bool SeCargoDiccionario = false;
        static int Grado;
        static bool SeleccionoSelect = false;
        static bool SeleccionoSelectFiltrado = false;
        static bool SeleccionoCreate = false;

        public ActionResult AsignarGrado()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AsignarGrado(FormCollection collection)
        {
            if(collection["grado"] == "")
            {
                Grado = 5;
            }
            else
            {
                Grado = int.Parse(collection["grado"]);
            }
            
            ViewBag.Mensaje = "Grado de arbol :" + Grado;
            return RedirectToAction("Menu");
        }
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

        public ActionResult Grid(string Nombre)
        {
            try
            {
                if (Nombre != null)
                {
                    Data.Instancia.nombreTabla = Nombre;
                }
                string llave = Data.Instancia.nombreTabla;

                Data.Instancia.Arboles[llave].ExistenElementosEnLista();
                Data.Instancia.Arboles[llave].AlmacenandoNodosEnLista(Data.Instancia.Arboles[llave].Raiz);
                Data.Instancia.listaNodos = Data.Instancia.Arboles[llave].RetornandoListaNodos();
                return View(Data.Instancia.listaNodos);
            }
            catch
            {
                ViewBag.Mensaje = "Tabla inexistente";
                return RedirectToAction("Menu");
            }
            
        }

        public ActionResult VerTablas()
        {
            return View();
        }

        public ActionResult CodigoSQL()
        {
            SeleccionoSelect = false;
            ExisteError = false;
            SeleccionoCreate = false;
            SeleccionoSelectFiltrado = false;
            return View();
        }

        [HttpPost]
        public ActionResult CodigoSQL(string codigo)
        {
            ExisteError = false;
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
            else if(ViewBag.MensajeError == "ERROR DE SINTAXIS")
            {
                
            }
            else
            {
                try
                {
                    for (int i = 0; i < cantidadBloques; i++)
                    {
                        string[] lineas1 = Bloques[i].Split('\r');
                        lineas.Clear();
                        SeparandoLineas(lineas, lineas1);


                        if (Data.Instancia.PalabrasReservadas.ContainsValue(lineas[0]) == true && ExisteError == false)
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
                        else if (Data.Instancia.PalabrasReservadas.ContainsKey(lineas[0]) == true && ExisteError == false)
                        {
                            Key = lineas[0];
                            Instrucciones(Key, lineas);
                        }
                        else
                        {
                            if(i == 0)
                            {
                                ViewBag.MensajeError = "ERROR EN BLOQUE " + (i) + ": Se ejecuto instrucciones previos al bloque con error\rInstrucciones realizadas: 0";
                            }
                            else
                            {
                                ViewBag.MensajeError = "ERROR EN BLOQUE " + (i) + ": Se ejecuto instrucciones previos al bloque con error\rInstrucciones realizadas: " + (i - 1);
                            }
                            
                            break;
                        }
                    }
                    //Data.Instancia.Arboles[Data.Instancia.nombreTabla].AlmacenandoNodosEnLista(Data.Instancia.Arboles[Data.Instancia.nombreTabla].Raiz);
                    //Data.Instancia.listaNodos = Data.Instancia.Arboles[Data.Instancia.nombreTabla].RetornandoListaNodos();
                    //Data.Instancia.EscribirArbol(Data.Instancia.Arboles[Data.Instancia.nombreTabla].Raiz);
                }
                catch
                {
                    ViewBag.MensajeError = "Se escribio incorrectamente un comando o falto comando para finalizar 'GO'";
                }
                
            }
            if(SeleccionoSelect == true && SeleccionoSelectFiltrado == true)
            {
                return View("SelectFiltrado");
            }
            else if (SeleccionoSelect == true)
            {
                return View("Select");
            }
            else if(SeleccionoCreate == true)
            {
                return RedirectToAction("Grid", "Home", new { Nombre = Data.Instancia.nombreTabla });
            }else
            {
                return View();
            }
            
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

        public void Instrucciones(string Key, List<string> instrucciones)
        {
            string nombreTabla = "";
            if (instrucciones[instrucciones.Count - 1] != "GO" && instrucciones[instrucciones.Count - 1] != "GO\r")
            {
                ViewBag.ErrorCodigo = "Sintaxis incorrecta: 'GO' omitido al final";
                ExisteError = true;
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
                            if(Data.Instancia.Arboles.ContainsKey(nombreTabla))
                            {
                                ExisteError = true;
                                ViewBag.MensajeError = nombreTabla + ": Tabla Existente, se ejecuto instrucciones anteriores a esta excepcion";
                            }
                            else
                            {
                                CrearTabla(nombreTabla, instrucciones);
                            }
                            break;
                        case "SELECT":
                            Select(nombreTabla, instrucciones);
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
                    ExisteError = true;
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
                    Estructuras_de_Datos.Info InfoVariables = new Estructuras_de_Datos.Info();
                    if(Data.Instancia.listaVariables.ContainsKey(nombreTabla) == false)
                    {
                        Data.Instancia.listaVariables.Add(nombreTabla, InfoVariables);
                    }else
                    {
                        ViewBag.MensajeError = "Tabla existente";
                        ExisteError = true;
                    }
                    for (int i = Codigobloque1[0] + 1; i < Codigobloque1[1]; i++)
                    {
                        string lineaAux = "";
                        char[] Lineachar = instrucciones[i].ToCharArray();
                        string[] linea = new string[100];
                        if (Lineachar[Lineachar.Length - 1] == ',')
                        {
                            linea = instrucciones[i].Split(',');
                            linea = linea[0].Split(' ');
                        }
                        else
                        {
                            lineaAux = instrucciones[i];
                            linea = lineaAux.Split(' ');
                        }
                        
                        Data.Instancia.Arboles[nombreTabla].ListaVariables.Add(linea[0]);
                        
                        switch (linea[1])
                        {
                            case "INT":
                                if(Data.Instancia.listaVariables[nombreTabla].ContadorInt <= 3)
                                {
                                    List<string> listaint = new List<string>();
                                    Data.Instancia.listaVariables[nombreTabla].Variables.Add(linea[0], listaint);
                                    Data.Instancia.listaVariables[nombreTabla].ContadorInt++;
                                }else
                                {
                                    ViewBag.MensajeError = "No se puede agregar mas variables de este tipo de dato\rSe omitieron las intrucciones siguientes a esta excepcion";
                                    ExisteError = true;
                                }
                                break;
                            case "VARCHAR(100)":
                                if (Data.Instancia.listaVariables[nombreTabla].ContadorChar <= 3)
                                {
                                    List<string> listaChar = new List<string>();
                                    Data.Instancia.listaVariables[nombreTabla].Variables.Add(linea[0], listaChar);
                                    Data.Instancia.listaVariables[nombreTabla].ContadorChar++;
                                }
                                else
                                {
                                    ViewBag.MensajeError = "No se puede agregar mas variables de este tipo de dato\rSe omitieron las intrucciones siguientes a esta excepcion";
                                    ExisteError = true;
                                }
                                break;
                            case "DATETIME":
                                if (Data.Instancia.listaVariables[nombreTabla].ContadorDT <= 3)
                                {
                                    List<string> listaDT = new List<string>();
                                    Data.Instancia.listaVariables[nombreTabla].Variables.Add(linea[0], listaDT);
                                    Data.Instancia.listaVariables[nombreTabla].ContadorDT++;
                                }
                                else
                                {
                                    ViewBag.MensajeError = "No se puede agregar mas variables de este tipo de dato\rSe omitieron las intrucciones siguientes a esta excepcion";
                                    ExisteError = true;
                                }
                                break;
                            default:
                                ViewBag.MensajeError = "TIPO DE DATO NO ADMITIDO\rSe omitio las instrucciones posteriores a esta excepcion";
                                ExisteError = true;
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
                    ExisteError = true;
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
                        if (VariablesAAgregar.Count == DatosDeVariables.Count && Data.Instancia.listaVariables[nombreTabla].Variables.ContainsKey(VariablesAAgregar[i]))
                        {
                            var LAux = Data.Instancia.listaVariables[nombreTabla].Variables[VariablesAAgregar[i]];
                            LAux.Add(DatosDeVariables[i]);
                            Reg.Valores.Add(DatosDeVariables[i]);
                            Data.Instancia.reg.Valores.Add(DatosDeVariables[i]);
                        }
                        else
                        {
                            ViewBag.MensajeError = "Falto una variable o excedio la cantidad";
                            ExisteError = true;
                            break;
                        }
                    }
                    Reg.IDPrimaryKey = int.Parse(DatosDeVariables[0]);
                    if (Data.Instancia.Arboles[nombreTabla].Insertar(Data.Instancia.Arboles[nombreTabla].Raiz, Reg, Grado) != 0)
                    {
                        ViewBag.MensajeError = nombreTabla + ": ID de registro repetido, cambie el ID para su registro correcto, se omitio las instrucciones despues de este bloque";
                        ExisteError = true;
                    }else
                    {
                        SeleccionoCreate = true;
                        Data.Instancia.ArbolesBPlus[nombreTabla].InsertarNodo(Reg, Grado);
                        ViewBag.MensajeError = nombreTabla + ": Insercion exitosa";
                    }
                }
                catch
                {
                    ExisteError = true;
                    ViewBag.MensajeError = "ERROR DE SINTAXIS";
                }

            }
            else
            {
                ExisteError = true;
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
                else if(instrucciones[2] == "WHERE")
                {
                    string[] separador = instrucciones[instrucciones.Count - 2].Split(' ');

                    var indice = int.Parse(separador[2]);
                    reg.IDPrimaryKey = indice;
                    Data.Instancia.Arboles[nombreTabla].Eliminar(reg, Data.Instancia.Arboles[nombreTabla].Raiz);
                    //Data.Instancia.ArbolesBPlus[nombreTabla].Eliminar(reg);
                    ViewBag.MensajeError = nombreTabla + ": Eliminacion correcta";
                }else
                {
                    ViewBag.MensajeError = nombreTabla + ": Error en escritura de comandos al eliminar un dato\rSe ejecutaron las instrucciones anteriores a esta excepcion.";
                    ExisteError = true;
                }
            }
            else
            {
                ViewBag.MensajeError = "NO EXISTE DICHA TABLA";
                ExisteError = true;
            }
        }

        public int Select(string nombreTabla, List<string> instrucciones)
        {
            int IDBuscado = 0;
            string[] linea;
            int cont = 0;
            while(instrucciones[cont] != "FROM")
            {
                cont++;
            }
            string[] nombre = instrucciones[cont + 1].Split(' ');
            nombreTabla = nombre[0];
            Data.Instancia.nombreTabla = nombreTabla;
            if (Data.Instancia.Arboles.ContainsKey(nombreTabla) && instrucciones[1] == "*" && instrucciones[2] == "FROM" && (instrucciones.Last() == "GO" || instrucciones.Last() == "GO\r") && instrucciones[instrucciones.Count - 3] == "WHERE")
            {
                Data.Instancia.Arboles[nombreTabla].VaciandoListaNodosBuscados();
                linea = instrucciones[instrucciones.Count- 2].Split(' ');
                if(linea[0] == "ID" && linea[1] == "=")
                {
                    IDBuscado = int.Parse(linea[2]);
                    Estructuras_de_Datos.Registro RegBuscado = new Estructuras_de_Datos.Registro();
                    RegBuscado.IDPrimaryKey = IDBuscado;
                    if (Data.Instancia.Arboles[nombreTabla].Busqueda(RegBuscado, Data.Instancia.Arboles[nombreTabla].Raiz) == 1)
                    {
                        Data.Instancia.listaNodosFiltrados = Data.Instancia.Arboles[nombreTabla].RetornandoListaDeNodosBuscados();
                        Data.Instancia.IDEncontrado = Data.Instancia.Arboles[nombreTabla].RetornandoIDDeValorBuscado();
                        SeleccionoSelect = true;
                        ExisteError = true;
                        return 1;
                    }
                    else
                    {
                        ViewBag.MensajeError = nombreTabla + ": No se encontro registro buscado, se detuvo en este punto la ejecucion de instrucciones";
                        ExisteError = true;
                        SeleccionoSelect = true;
                        return 0;
                    }
                }
                else
                {
                    ExisteError = true;
                    ViewBag.MensajeError = nombreTabla + ": ERROR DE SINTAXIS";
                    return 0;
                }
            }
            else if (Data.Instancia.Arboles.ContainsKey(nombreTabla) && instrucciones[1] == "*" && instrucciones[2] == "FROM" && (instrucciones.Last() == "GO" || instrucciones.Last() == "GO\r"))
            {
                SeleccionoCreate = true;
                return 1;
            }
            else if(instrucciones.Count > 5)
            {
                SeleccionoSelectFiltrado = true;
                SeleccionoSelect = true;
                Data.Instancia.VariablesFiltradas.Clear();
                Data.Instancia.RegistrosVariablesFiltradas.Clear();
                char[] lineaChar;
                int numerodelineainstrucciones = 1;
                while(instrucciones[numerodelineainstrucciones] != "FROM")
                {
                    lineaChar = instrucciones[numerodelineainstrucciones].ToCharArray();
                    if (lineaChar.Last() == ',' && instrucciones[numerodelineainstrucciones + 1] == "FROM")
                    {
                        ViewBag.MensajeError = "Mala sintaxis, la ultima variable no puede tener coma al final";
                        ExisteError = true;
                        SeleccionoSelect = SeleccionoSelectFiltrado = false ;
                        break;
                    }
                    else if (lineaChar.Last() == ',')
                    {
                        Data.Instancia.VariablesFiltradas.Add(instrucciones[numerodelineainstrucciones].Split(',')[0]);
                    }
                    else
                    {
                        Data.Instancia.VariablesFiltradas.Add(instrucciones[numerodelineainstrucciones]);
                    }
                    numerodelineainstrucciones++;
                }
                Data.Instancia.nombreTabla = instrucciones[numerodelineainstrucciones + 1];
                nombreTabla = Data.Instancia.nombreTabla;
                List<int> posicionesVariables = new List<int>();
                int ContadorVariables = 0;
                for (int i = 0; i < Data.Instancia.Arboles[nombreTabla].ListaVariables.Count; i++)
                {
                    if (Data.Instancia.VariablesFiltradas.Contains(Data.Instancia.Arboles[nombreTabla].ListaVariables[i]))
                    {
                        posicionesVariables.Add(i);
                        ContadorVariables++;
                    }
                }

                if(ContadorVariables != Data.Instancia.VariablesFiltradas.Count)
                {
                    ExisteError = true;
                    ViewBag.MensajeError = "Una variable no existe\rSe detuvo la ejecucion de instrucciones";
                    SeleccionoSelect = false;
                    SeleccionoSelectFiltrado = false;
                }
                else if (instrucciones[numerodelineainstrucciones + 2] != "GO" && instrucciones[numerodelineainstrucciones + 2] != "GO\r")
                {
                    string[] lineaID = instrucciones[numerodelineainstrucciones + 3].Split(' ');
                    int IDABuscar = int.Parse(lineaID[2]);

                    foreach (var nodo in Data.Instancia.listaNodos)
                    {
                        for (int i = 0; i < nodo.Valores.Count; i++)
                        {
                            if (nodo.Valores[i].IDPrimaryKey == IDABuscar)
                            {
                                Estructuras_de_Datos.Registro registrofiltrado = new Estructuras_de_Datos.Registro();
                                registrofiltrado.IDPrimaryKey = nodo.Valores[i].IDPrimaryKey;

                                for (int j = 0; j < nodo.Valores[i].Valores.Count; j++)
                                {
                                    if (posicionesVariables.Contains(j - 1))
                                    {
                                        registrofiltrado.Valores.Add(nodo.Valores[i].Valores[j - 1]);
                                    }
                                }
                                Data.Instancia.RegistrosVariablesFiltradas.Add(registrofiltrado);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var nodo in Data.Instancia.listaNodos)
                    {
                        for (int i = 0; i < nodo.Valores.Count; i++)
                        {
                            Estructuras_de_Datos.Registro registrofiltrado = new Estructuras_de_Datos.Registro();
                            registrofiltrado.IDPrimaryKey = nodo.Valores[i].IDPrimaryKey;

                            for (int j = 0; j < nodo.Valores[i].Valores.Count; j++)
                            {
                                if (posicionesVariables.Contains(j - 1))
                                {
                                    registrofiltrado.Valores.Add(nodo.Valores[i].Valores[j - 1]);
                                }
                            }
                            Data.Instancia.RegistrosVariablesFiltradas.Add(registrofiltrado);
                        }
                    }
                }
                return 1;
            }
            //PARA LA BUSQUEDA LIKE
            else if(Data.Instancia.Arboles.ContainsKey(nombreTabla) == false)
            {
                ExisteError = true;
                ViewBag.MensajeError = nombreTabla + ": No existe tabla, se realizaron las intrucciones de los bloques anteriores a esta excepcion";
                return 0;
            }
            else
            {
                ExisteError = true;
                ViewBag.MensajeError = nombreTabla + ": ERROR DE SINTAXIS";
                return 0;
            }
        }

        public void DropTable(string nombreTabla)
        {
            if (Data.Instancia.Arboles.ContainsKey(nombreTabla))
            {
                Data.Instancia.Arboles[nombreTabla].EliminarTodo(Data.Instancia.Arboles[nombreTabla].Raiz);
                Data.Instancia.NombresTabla.Remove(nombreTabla);
                Data.Instancia.Arboles[nombreTabla].Raiz = null;
                Data.Instancia.Arboles.Remove(nombreTabla);
                //Data.Instancia.ArbolesBPlus.Remove(nombreTabla);
                ViewBag.MensajeError = nombreTabla + ": Eliminación de tabla correcta";
            }
            else
            {
                ExisteError = true;
                ViewBag.MensajeError = "NO EXISTE DICHA TABLA";
            }
        }

        public ActionResult Carga()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Carga(HttpPostedFileBase file)
        {
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    string model = "";
                        SeCargoDiccionario = true;
                        model = Server.MapPath("~/.ini/MicroSQL.ini");
                        file.SaveAs(model);
                        if (Data.Instancia.LecturaCSV(model) == 1)
                        {
                            ViewBag.Msg = "Carga del archivo correcta";
                            ViewBag.Mensaje = "Carga del archivo correcta";
                            return RedirectToAction("Menu");
                        }
                        else
                        {
                            ViewBag.Msg = "Carga del archivo incorrecta";
                            return View();
                        }
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

        public ActionResult Menu()
        {
            if(SeCargoDiccionario == false)
            {
                SeCargoDiccionario = true;
                Data.Instancia.LecturaCSV(Server.MapPath("~/.ini/MicroSQL.ini"));
            }
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
                if(Valor != null && Valor != "" && Data.Instancia.PalabrasReservadas.ContainsKey(llave))
                {
                    Data.Instancia.PalabrasReservadas[llave] = Valor;
                    ViewBag.Error = "Modificacion correcta";
                }
                if ((Valor == null || Valor == "") && Data.Instancia.PalabrasReservadas.ContainsKey(llave))
                {
                    ViewBag.Error = "Palabra a modificar no puede ser espacio en blanco";
                }
                else if(Data.Instancia.PalabrasReservadas.ContainsKey(llave) == false)
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
            Data.Instancia.LecturaCSV(Server.MapPath("~/.ini/MicroSQL Predeterminado.ini"));
            ViewBag.Mensaje = "Restablecimiento completado";
            return View("Menu");
        }
    }
}
