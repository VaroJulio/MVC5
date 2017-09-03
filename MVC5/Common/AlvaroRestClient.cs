using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using MVC5.Models;
using System.Configuration;

namespace MVC5.Common
{
    public class AlvaroRestClient
    {
        public string EndPoint { get; set; }
        public CommonCode.HttpVerb HttpVerb {get; set;}

        public AlvaroRestClient(string endPointStr)
        {
            EndPoint = endPointStr;
            HttpVerb = CommonCode.HttpVerb.GET;
        }

        /// <summary>
        /// Método que reliza la petición de Token de solicitud a Flickr para iniciar la autenticación.
        /// </summary>
        /// <returns>String con la respuesta de Flick. Puede ser un mensaje de error o si es exitosa llevar incluido el Token de solicitud</returns>
        public async Task<string> MakeRequestTokenSolicitudAsync()
        {
            string respuestaTokenSoli, tokenSolicitud = null;
            //Se obtiene la información de seguridad del usuario Flickr para autenticación
            string[] infoSegApp = CommonCode.ObtenerInfoSeguridadApp();
            string signature = CommonCode.ObtenerFirma(infoSegApp, out string nonce, out string timeStamp);
            //Se contruye la url donde  obtener token de solicitud
            string requestUrlSolicitudToken = CommonCode.ConstruirUrlTokenSolicitud(infoSegApp, signature, timeStamp, nonce);
            //Se realiza petición para obtener token de solicitud
            using (var clieneRequestToken = new HttpClient())
            {
                try
                {
                    string endpointUrlTokenSoli = requestUrlSolicitudToken;
                    clieneRequestToken.BaseAddress = new Uri(endpointUrlTokenSoli);
                    clieneRequestToken.DefaultRequestHeaders.Clear();
                    clieneRequestToken.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
                    HttpResponseMessage responseTokenSolicitud = await clieneRequestToken.GetAsync(string.Empty);
                    if (responseTokenSolicitud.IsSuccessStatusCode)
                    {
                        respuestaTokenSoli = responseTokenSolicitud.Content.ReadAsStringAsync().Result;
                        tokenSolicitud = CommonCode.ObtenerTokenAutorizacion(respuestaTokenSoli);
                    }
                    else
                    {
                        tokenSolicitud = Resources.StringResources.ResourceManager.GetString("ErrorToken"); ;
                    }
                }
                catch (HttpRequestException ex)
                {
                    respuestaTokenSoli = ex.Message;
                }
            }
            return tokenSolicitud;
        }
        
        //Todavía no se ha probado su correcto funcionamiento
        public async Task<PhotosetsResponse> MakeRequestPhotoSetsGetList()
        {
            PhotosetsResponse photoSetResponse = new PhotosetsResponse();
            using (var client = new HttpClient())
            {
                string endpointUri = CommonCode.ObtenerEndPoint(EndPoint);
                if (!endpointUri.Equals("Not Found"))
                {
                    //Se especifica la url de la api a consumir
                    client.BaseAddress = new Uri(endpointUri);
                    client.DefaultRequestHeaders.Clear();
                    //Se especifica el formato del request - response
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringBuilder complementUri = new StringBuilder();
                    complementUri.Append("method=flickr.photosets.getList&api_key=266921bda37a11190ae63e14c0858c85&format=json&per_page=200");
                    HttpResponseMessage response = await client.GetAsync(complementUri.ToString());
                    if (response.IsSuccessStatusCode)
                    {
                        //Storing the response details recieved from web api   
                        var photosetResponse = response.Content.ReadAsStringAsync().Result;
                        //Deserializing the response recieved from web api and storing into the Employee list  
                        photoSetResponse = JsonConvert.DeserializeObject<PhotosetsResponse>(photosetResponse);

                    }
                }
            }
            return photoSetResponse;
        }

        public async Task<string> AutorizarUsuarioAsync(string tokenAurtorizacion)
        {
            string respuestaVerificador, verificador = null;
            using (var clienteVerificador = new HttpClient())
            {
                try
                {
                    string endpointUrlVerificador = ConfigurationManager.AppSettings["AutorizacionUsuario"] + tokenAurtorizacion;
                    clienteVerificador.BaseAddress = new Uri(endpointUrlVerificador);
                    clienteVerificador.DefaultRequestHeaders.Clear();
                    clienteVerificador.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
                    HttpResponseMessage responseAutorizacion = await clienteVerificador.GetAsync(string.Empty);
                    if (responseAutorizacion.IsSuccessStatusCode)
                    {
                        respuestaVerificador = responseAutorizacion.Content.ReadAsStringAsync().Result;
                        verificador = CommonCode.ObtenerVerificador(respuestaVerificador);
                    }
                    else
                    {
                        verificador = Resources.StringResources.ResourceManager.GetString("ErrorVerificador");
                    }
                }
                catch (HttpRequestException ex)
                {
                    verificador = ex.Message;
                }
            }
            return verificador;
        }

        public  void AutorizarUsuario(string tokenAurtorizacion)
        {
           System.Diagnostics.Process.Start(ConfigurationManager.AppSettings["AutorizacionUsuario"] + tokenAurtorizacion);
        }
    }
}