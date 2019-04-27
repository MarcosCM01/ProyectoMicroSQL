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

        public ActionResult CodigoSQL()
        {
            return View();
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
            return View(Data.Instancia.Dictionary);
        }

        public ActionResult Modificar(string NewWord)
        {
            try
            {
                ViewBag.Error = "";
                string[] array = NewWord.Split(',');
                string key = array[0].Trim();
                string value = array[1].Trim();

                Data.Instancia.Dictionary[key] = value;

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