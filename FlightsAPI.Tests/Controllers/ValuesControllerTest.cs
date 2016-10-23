using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlightsAPI;
using FlightsAPI.Controllers;
using System.Net;

namespace FlightsAPI.Tests.Controllers
{
    [TestClass]
    public class FlightsControllerTest
    {
        [TestMethod]
        public void Get()
        {
            // Arrange
            FlightsController controller = new FlightsController();

            // Act
            IHttpActionResult result = controller.Get();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, result);
            //Assert.AreEqual("value1", result.ElementAt(0));
            //Assert.AreEqual("value2", result.ElementAt(1));
        }

        [TestMethod]
        public void GetById()
        {
            // Arrange
            FlightsController controller = new FlightsController();

            // Act
            string result = controller.Get(5);

            // Assert
            Assert.AreEqual("value", result);
        }

        [TestMethod]
        public void Post()
        {
            // Arrange
            FlightsController controller = new FlightsController();

            // Act
            controller.Post("value");

            // Assert
        }

        [TestMethod]
        public void Put()
        {
            // Arrange
            FlightsController controller = new FlightsController();

            // Act
            controller.Put(5, "value");

            // Assert
        }

        [TestMethod]
        public void Delete()
        {
            // Arrange
            FlightsController controller = new FlightsController();

            // Act
            controller.Delete(5);

            // Assert
        }
    }
}
