using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVC5.Common;

namespace MVC5.Tests.Common
{
    [TestClass]
    public class CommonTest
    {
        [TestMethod]
        public void ObtenerEndPointTest()
        {
            string endPoint = CommonCode.ObtenerEndPoint(CommonCode.ApiAdress.FlickrApi.ToString());
            Assert.AreEqual("http://api.flickr.com/services/rest/?", endPoint);
        }

        [TestMethod]
        public void ObtenerNonceTest()
        {
            string segTimeStamp = CommonCode.ObtenerNonce(out string nonce);
            DateTime fecha = new DateTime(1970, 1, 1, 0, 0, 0);
            fecha = fecha.AddSeconds(double.Parse(segTimeStamp)).ToUniversalTime();
            string otraFecha = DateTime.Now.ToUniversalTime().ToShortDateString();
            Assert.AreEqual(fecha.ToShortDateString(), otraFecha);
        }

        [TestMethod]
        public void ObtenerFirmaTest()
        {
            string[] infoSeguridad = new string[5];
            infoSeguridad = CommonCode.ObtenerInfoSeguridadApp();
            string firma = CommonCode.ObtenerFirma(infoSeguridad, out string nonce, out string timeStamp);
        }
    }
}
