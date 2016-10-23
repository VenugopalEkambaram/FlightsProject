using FlightsApi.BusinessLayer;
using FlightsRepository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlightsApi.Tests.BusinessLayer
{
    [TestClass]
    public class ScheduleManagerTests
    {
        static IFlightRepository mockedRepo = Substitute.For<IFlightRepository>();
        ScheduleManager manager = new ScheduleManager(mockedRepo);
        static string jsonContent = Properties.Resources.MockedData_json;
        List<Flight> returnData = JsonConvert.DeserializeObject<List<Flight>>(jsonContent);

        [TestMethod]
        public void AllocateSchedule_Should_Push_Two_Flights_In_Gate_1_To_Address_Overlap()
        {
            // Arrange            
            var mockFlight = new Flight
            {
                FlightNumber = "XYZ123",
                Arrival = DateTime.Parse("2016-09-20T14:10:00"),
                Departure = DateTime.Parse("2016-09-20T14:39:00"),
                Gate = 1,
                Status = FlightStatus.Arrived
            };
            
            mockedRepo.Get().Returns(returnData);

            // Act
            var result = manager.AllocateSchedule(mockFlight, true);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Where(x => x.FlightNumber == "VA145").Count());
            Assert.AreEqual(1, result.Where(x => x.FlightNumber == "IA234").Count());            
        }

        [TestMethod]
        public void AllocateSchedule_Should_Add_To_Available_Gate_If_Intended_Gate_Has_NoSlots()
        {
            // Arrange            
            var mockFlight = new Flight
            {
                FlightNumber = "XYZ125",
                Arrival = DateTime.Parse("2016-09-20T10:00:00"),
                Departure = DateTime.Parse("2016-09-20T10:30:00"),
                Gate = 2,
                Status = FlightStatus.Arrived
            };
            
            mockedRepo.Get().Returns(returnData);

            // Act
            var result = manager.AllocateSchedule(mockFlight, true);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreNotEqual(2, result.First().Gate);
        }

        [TestMethod]
        public void AllocateSchedule_Should_Update_To_Given_Gate_Only()
        {
            // Arrange            
            var mockFlight = new Flight
            {
                FlightNumber = "VA149",
                Arrival = DateTime.Parse("2016-09-20T10:00:00"),
                Departure = DateTime.Parse("2016-09-20T10:29:00"),
                Gate = 1,
                Status = FlightStatus.Arrived
            };

            mockedRepo.Get().Returns(returnData);

            // Act
            var result = manager.AllocateSchedule(mockFlight, false);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.First().Gate);
        }

        [TestMethod]
        public void Get_With_String_Input_Should_Return_Valid_Flight()
        {
            // Arrange            
            var mockInput = "VA234";

            mockedRepo.Get().Returns(returnData);

            // Act
            var result = manager.Get(mockInput);
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(mockInput, result.FlightNumber);
        }

        [TestMethod]
        public void Get_With_String_Input_Should_Return_Null_For_Invalid_Input()
        {
            // Arrange            
            var mockInput = "YYYYY";

            mockedRepo.Get().Returns(returnData);

            // Act
            var result = manager.Get(mockInput);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Get_With_Int_Input_Should_Return_Flights_List_For_Given_Gate_Input()
        {
            // Arrange            
            var mockInput = 1;

            mockedRepo.GetByGate(mockInput).Returns(returnData.Where(x => x.Gate == mockInput).ToList());
            var expectedOutput = returnData.Where(x => x.Gate == mockInput).Count();

            // Act
            var result = manager.Get(mockInput);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedOutput, result.Count());
        }
    }
}
