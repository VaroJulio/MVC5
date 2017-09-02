using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVC5.Common;

namespace MVC5.Tests.Common
{
    [TestClass]
    public class AlvaroClientTest
    {
        [TestMethod]
        public void MakeRequestTokenSolicitudTest()
        {
            AlvaroRestClient clienteRest = new AlvaroRestClient(CommonCode.ApiAdress.FlickrApi.ToString());
            string respuesta = clienteRest.MakeRequestTokenSolicitudAsync().Result; 
        }
    }
}
