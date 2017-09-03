using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using MVC5.Common;
using MVC5.Models;

namespace MVC5.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            PhotosetsResponse photoSetResponse = null;
            string notificarRespuesta = null;
            AlvaroRestClient clienteRest = new AlvaroRestClient(CommonCode.ApiAdress.FlickrApi.ToString());
            string respToken = await clienteRest.MakeRequestTokenSolicitudAsync();
            if (clienteRest != null && !string.IsNullOrEmpty(respToken) && !respToken.Contains("Error"))
            {
                clienteRest.AutorizarUsuario(respToken);
                //string verificador = await clienteRest.AutorizarUsuarioAsync(respToken);
                photoSetResponse = await clienteRest.MakeRequestPhotoSetsGetList();
                if (photoSetResponse != null)
                {
                    notificarRespuesta = "Se han encontrado Álbums de fotos";
                }
                else
                {
                    notificarRespuesta = "No se encontraron Albums de fotografías";
                }
            }
            else
            {
                notificarRespuesta = "No se encontró la BaseUrl de la API";
            }
            ViewBag.notificarRespuesta = notificarRespuesta;
            return View(photoSetResponse);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}