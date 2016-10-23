using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FlightsRepository
{
    internal static class DataProvider
    {
        private static List<Flight> _dataSet;

        static DataProvider()
        {
            InitializeData(GetJsonData());
        }

        public static List<Flight> Get()
        {
            return _dataSet.OrderBy(x => x.Gate).ThenBy(x => x.Arrival).ToList();
        }

        public static List<Flight> GetByGate(int Id)
        {
            return _dataSet.Where(x => x.Gate.Equals(Id)).ToList();
        }

        public static bool Add(Flight flight)
        {
            _dataSet.Add(flight);
            return true;
        }

        public static bool Update(Flight flight)
        {
            if (!_dataSet.Remove(_dataSet.Where(x => x.FlightNumber.Equals(flight.FlightNumber)).FirstOrDefault()))
                return false;
            _dataSet.Add(flight);
            return true;
        }

        private static Stream GetJsonData()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "FlightsRepository.App_Data.DataStub.json";
            return assembly.GetManifestResourceStream(resourceName);
        }

        private static void InitializeData(Stream fileStream)
        {
            using (var stream = new StreamReader(fileStream))
            {
                string json = stream.ReadToEnd();
                _dataSet = JsonConvert.DeserializeObject<List<Flight>>(json);
                
                //To maintain the data for current date
                int diff = DateTime.Now.Day - DateTime.Parse("2016-09-20T00:00:00").Day;
                _dataSet.ForEach(x => x.Arrival = x.Arrival.Date.AddDays(diff).AddTicks(x.Arrival.TimeOfDay.Ticks));
                _dataSet.ForEach(x => x.Departure = x.Departure.Date.AddDays(diff).AddTicks(x.Departure.TimeOfDay.Ticks));
            }
        }


    }
}