using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//Se agrega para incluir descripción de los miembros de la enumeración
using System.ComponentModel;
using System.Configuration;
using System.Text;
using System.Globalization;

namespace MVC5.Common
{
    public static class CommonCode
    {
        /// <summary>
        /// Método estático que sirve para obtener el endpoint (BaseUrl) de la Api a consumir desde el archivo
        /// de configuración.
        /// </summary>
        /// <param name="urlAddress">String del alias del endpoint a obtener</param>
        /// <returns>String del endpoint de la API a consumir</returns>
        public static string ObtenerEndPoint(string urlAddress)
        {
            string url = null;
            if (urlAddress != null)
            {
                bool hasKeysConf = ConfigurationManager.AppSettings.HasKeys();
                url = hasKeysConf ? ConfigurationManager.AppSettings[urlAddress] : "Not Found";
            }
            return url;
        }
        
        //Enumeración con los verbos HTTP
        public enum HttpVerb
        {
            [Description("1")]
            GET,
            [Description("2")]
            POST,
            [Description("3")]
            PUT,
            [Description("4")]
            DELETE
        }
        
        //Enumeración con los alias de los EndPoint que consume la aplicación
        public enum ApiAdress
        {
            FlickrApi
        }

        /// <summary>
        /// Método que sirve para obtener el argumento timestamp y el nonce 
        /// con los que haremos autenticación Oauth en la API
        /// </summary>
        /// <param name="nonce">Parámetro de salida de tipo entero</param>
        /// <returns>El string con la cantidad de segundos desde el primero de enero de 1970 hasta la fecha.</returns>
        public static string ObtenerNonce(out string nonce)
        {
            string strTimeStamp = null;
            // Int32 segs = (int)(DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
            //Se calcula en milisegudnos
            Int64 segs = ((Int64)(DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds) + Convert.ToInt64(0.5);
            strTimeStamp = segs.ToString(NumberFormatInfo.InvariantInfo);
            //nonce = (segs / 1000) * 3;
            nonce = Guid.NewGuid().ToString().Replace("-", string.Empty);
            return strTimeStamp;
        }

        /// <summary>
        /// Método estático que sirve para obtener la configuración de seguridad para realizar autenticación con OAUTH en Flickr
        /// de configuración.
        /// </summary>
        public static string[] ObtenerInfoSeguridadApp()
        {
            string[] seguParam = new string[6];
            bool hasKeysConf = ConfigurationManager.AppSettings.HasKeys();
            seguParam[0] = hasKeysConf ? ConfigurationManager.AppSettings["ConsumerKey"] : "Not Found";
            seguParam[1] = hasKeysConf ? ConfigurationManager.AppSettings["ConsumerSecret"] : "Not Found";
            seguParam[2] = hasKeysConf ? ConfigurationManager.AppSettings["SignatureMethod"] : "Not Found";
            seguParam[3] = hasKeysConf ? ConfigurationManager.AppSettings["CallBackUrl"] : "Not Found";
            seguParam[4] = hasKeysConf ? ConfigurationManager.AppSettings["VersionOauth"] : "Not Found";
            seguParam[5] = hasKeysConf ? ConfigurationManager.AppSettings["EndPointRequestToken"] : "Not Found";
            return seguParam;
        }

        /// <summary>
        /// Método estático que obtiene la firma de autenticación para obtener el token de solicitud en Flickr 
        /// </summary>
        /// <param name="parametrosSegu">Array de tipo string que contiene la información de configuración de seguridad del APP</param>
        /// <returns>La firma para obtener el token de solicitud en Flckr</returns>
        public static string ObtenerFirma(string[] parametrosSegu, out string nonce, out string timeStamp)
        {
            string firma;
            //Primera parte del Base String
            string key = string.Concat(parametrosSegu[1],"&");
            //Segunda parte del Base String
            string utf8EndPointToken = string.Concat(ObtenerCadenaToUriEncoding(parametrosSegu[5]), "&");
            string[] parametrosBaseStr = new string[6];
            //Tercera parte del Base String
            parametrosBaseStr = ObtenerParametrosBaseString(parametrosSegu, out string timeStamp2, out string nonce2);
            timeStamp = timeStamp2;
            nonce = nonce2;
            //Unión de todas las partes para conformar el BaseString
            string baseString = ObtenerBaseString(parametrosBaseStr, utf8EndPointToken);
            firma = GenerarSignature(baseString, key);
            return ObtenerCadenaToUriEncoding(firma);
        }

        /// <summary>
        /// Devuelve los parametros de seguridad concatenados con los caracteres especiales que luego no permitiran armar la BaseString
        /// </summary>
        /// <param name="parametrosSegu">Vector con los parametros de seguridad de Flickr</param>
        /// <param name="timeStamp2">Sting con el valor del timeStamp en segundos</param>
        /// <param name="nonce2">String con el valor del noce (guid)</param>
        /// <returns>Vector String con los parametros de seguridad de Flickr concatenados con caracteres especiales</returns>
        public static string[] ObtenerParametrosBaseString(string[] parametrosSegu, out string timeStamp2, out string nonce2)
        {
            string[] parametrosBaseString = new string[7];
            
            timeStamp2 = ObtenerNonce(out string oauthNonce); //TimeStamp
            nonce2 = oauthNonce;  //Nonce - Guid
            parametrosBaseString[0] = string.Concat("=", parametrosSegu[3], "&"); //oauthCallBack
            parametrosBaseString[1] = string.Concat("=", parametrosSegu[0], "&"); //oauthConsumerKey
            parametrosBaseString[2] = string.Concat("=", timeStamp2, "&"); //oauthTimesTamp
            parametrosBaseString[3] = string.Concat("=", oauthNonce.ToString(), "&"); //oauthStrNounce
            parametrosBaseString[4] = string.Concat("=", parametrosSegu[2], "&"); //oauthSignatureMethod
            parametrosBaseString[5] = string.Concat("=", parametrosSegu[4]); //oauthVersion
            parametrosBaseString[3] = ObtenerCadenaToUriEncoding(parametrosBaseString[3]); //oauthStrNounce
            return parametrosBaseString;
        }

        /// <summary>
        /// Devuelve el BaseString necesario para obtener la firma en el servicio de autenticación de Flickr
        /// </summary>
        /// <param name="parametrosBaseStr">Vector string con los parametros de seguridad de Flickr concatenados con caracteres especiales</param>
        /// <param name="utf8EndToken">String con La URL de flickr donde se realizará la petición del token</param>
        /// <returns>String con el BaseString de Flickr para generar la signature</returns>
        public static string ObtenerBaseString(string[] parametrosBaseStr, string utf8EndToken)
        {
            string baseString;
            return baseString = HttpVerb.GET.ToString() + "&" + utf8EndToken + "oauth_callback" + ObtenerCadenaToUriEncoding(parametrosBaseStr[0]) +
               "oauth_consumer_key" + ObtenerCadenaToUriEncoding(parametrosBaseStr[1]) + "oauth_nonce" + parametrosBaseStr[3] + "oauth_signature_method" + ObtenerCadenaToUriEncoding(parametrosBaseStr[4]) +
               "oauth_timestamp" + ObtenerCadenaToUriEncoding(parametrosBaseStr[2]) + "oauth_version" + ObtenerCadenaToUriEncoding(parametrosBaseStr[5]);
        }

        /// <summary>
        /// Convierte la cadena que recibe como argumento a codoficación Uri
        /// </summary>
        /// <param name="cadena">String a convertir a codificación Uri</param>
        /// <returns>String en codoficación Uri</returns>
        public static string ObtenerCadenaToUriEncoding(string cadena)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(cadena);
            string cadenaUTF8 = Encoding.UTF8.GetString(bytes);
            string toUriEncoding = Uri.EscapeDataString(cadenaUTF8);
            return toUriEncoding;
        }

        /// <summary>
        /// Método estático que genera el signature bajo encriptación SHA1 para las firmas de las peticiones que se realicen a Flickr
        /// </summary>
        /// <param name="baseString">El BaseString de la petición</param>
        /// <param name="key">El key de Flickr</param>
        /// <returns>String en codoficación utf8</returns>
        public static string GenerarSignature(string baseString, string key)
        {
            byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(key);
            System.Security.Cryptography.HMACSHA1 sha1 = new System.Security.Cryptography.HMACSHA1(keyBytes);
            byte[] signatureBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(baseString));
            string signature = Convert.ToBase64String(signatureBytes);
            return signature;
        }

        /// <summary>
        /// Método que permite obtener la URL donde se pedirá el Token de solicitud de Flickr
        /// </summary>
        /// <param name="infoSegApp">Vector tipo string con la información de seguridad</param>
        /// <param name="signature">String con la firma de Flickr</param>
        /// <param name="timeStamp">String con la cantidad de segundos trascurridos desde 1970 hasta el momento en que se realizó la petición</param>
        /// <param name="nonce">String con el GUID identificador de la petición o del usuario</param>
        /// <returns>String con la URL a la URL que se debe invocar para recibir la respuesta con el Token de solicitud de FLickr</returns>
        public static string ConstruirUrlTokenSolicitud(string[] infoSegApp, string signature, string timeStamp, string nonce)
        {
            string requestUrlSolicitudToken = infoSegApp[5] + "?" + "oauth_callback=" +
                Uri.EscapeDataString(infoSegApp[3]) + "&" + "oauth_consumer_key=" + infoSegApp[0] +
                "&" + "oauth_nonce=" + nonce + "&" + "oauth_signature_method=" + infoSegApp[2] + "&" +
                "oauth_timestamp=" + timeStamp + "&" + "oauth_version=" + infoSegApp[4] + "&" +
                "oauth_signature=" + signature;
            return requestUrlSolicitudToken;
        }
    }
}