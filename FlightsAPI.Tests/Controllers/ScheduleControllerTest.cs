using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlightsApi.Controllers;
using System.Net;
using FlightsRepository;
using NSubstitute;
using Newtonsoft.Json;
using System.Web.Http.Results;
using FlightsApi.BusinessLayer;

namespace FlightsApi.Tests.Controllers
{
    [TestClass]
    public class ScheduleControllerTest
    {
        static IScheduleManager mockedSchMgr = Substitute.For<IScheduleManager>();
        ScheduleController controller = new ScheduleController(mockedSchMgr);
        static string jsonContent = Properties.Resources.MockedData_json;
        List<Flight> returnData = JsonConvert.DeserializeObject<List<Flight>>(jsonContent);

        [TestMethod]
        public void Get_Should_Return_NotNullValue_With_OK_Statuscode()
        {
            // Arrange
            mockedSchMgr.Get().Returns(returnData);

            // Act
            var result = (NegotiatedContentResult<List<Flight>>)controller.Get();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void Get_With_Int_Input_Should_Return_NotNullValue_With_OK_Statuscode()
        {
            // Arrange
            var mockInput = 1;
            var expectedOutput = returnData.Where(x => x.Gate == mockInput).ToList();

            mockedSchMgr.Get().Returns(expectedOutput);

            // Act
            var result = (NegotiatedContentResult<List<Flight>>)controller.Get(mockInput);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }


        [TestMethod]
        public void Post_Should_Return_BadRequest_With_Invalid_Message()
        {
            // Arrange
            var mockFlight = new Flight
            {
                FlightNumber = "",
                Arrival = DateTime.Parse("2016-09-20T14:10:00"),
                Departure = DateTime.Parse("2016-09-20T14:39:00"),
                Gate = 1,
                Status = FlightStatus.Arrived
            };

            // Act
            var result = (NegotiatedContentResult<string>)controller.Post(mockFlight);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.AreEqual("Invalid flight input.", result.Content);
        }

        [TestMethod]
        public void Post_Should_Return_BadRequest_With_Already_Exists_Message()
        {
            // Arrange
            var mockFlight = new Flight
            {
                FlightNumber = "QA123",
                Arrival = DateTime.Now,
                Departure = DateTime.Now.AddMinutes(29),
                Gate = 1,
                Status = FlightStatus.Arrived
            };

            mockedSchMgr.Get("QA123").Returns(mockFlight);

            // Act
            var result = (NegotiatedContentResult<string>)controller.Post(mockFlight);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.AreEqual("Flight already exists.", result.Content);
        }

        [TestMethod]
        public void Post_Should_Return_Created_StatusCode()
        {
            // Arrange
            var mockFlight = new List<Flight>
            {
                new Flight {
                FlightNumber = "QA123",
                Arrival = DateTime.Now,
                Departure = DateTime.Now.AddMinutes(29),
                Gate = 1,
                Status = FlightStatus.Arrived
                }
            };
            Flight nullFlight = null;
            mockedSchMgr.Get("QA123").Returns(nullFlight);
            mockedSchMgr.AllocateSchedule(mockFlight.First(), true).Returns(mockFlight);

            // Act
            var result = (NegotiatedContentResult<List<Flight>>)controller.Post(mockFlight.First());

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);
            Assert.AreEqual(mockFlight.First().FlightNumber, result.Content.First().FlightNumber);
            Assert.AreEqual(mockFlight.First().Gate, result.Content.First().Gate);
        }

        [TestMethod]
        public void Put_Should_Return_BadRequest_With_Invalid_Message()
        {
            // Arrange
            var mockFlight = new Flight
            {
                FlightNumber = "",
                Arrival = DateTime.Parse("2016-09-20T14:10:00"),
                Departure = DateTime.Parse("2016-09-20T14:39:00"),
                Gate = 1,
                Status = FlightStatus.Arrived
            };

            // Act
            var result = (NegotiatedContentResult<string>)controller.Put(mockFlight);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.AreEqual("Invalid flight input.", result.Content);
        }

        [TestMethod]
        public void Put_Should_Return_Accepted_StatusCode()
        {
            // Arrange
            var mockFlight = new List<Flight> {
            new Flight {
                FlightNumber = "QA123",
                Arrival = DateTime.Now,
                Departure = DateTime.Now.AddMinutes(29),
                Gate = 1,
                Status = FlightStatus.Arrived }
            };

            mockedSchMgr.AllocateSchedule(mockFlight.First(), false).Returns(mockFlight);

            // Act
            var result = (NegotiatedContentResult<List<Flight>>)controller.Put(mockFlight.First());

            // Assert
            Assert.AreEqual(HttpStatusCode.Accepted, result.StatusCode);
            Assert.AreEqual(mockFlight.First().FlightNumber, result.Content.First().FlightNumber);
            Assert.AreEqual(mockFlight.First().Gate, result.Content.First().Gate);
        }
    }
}
