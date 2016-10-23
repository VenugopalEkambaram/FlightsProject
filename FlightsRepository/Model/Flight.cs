using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace FlightsRepository
{
    public class Flight
    {
        public string FlightNumber { get; set; }
        public DateTime Arrival { get; set; }
        public DateTime Departure { get; set; }
        public int Gate { get; set; }
        public FlightStatus Status { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FlightStatus
    { Scheduled, Arrived, Departed, Delayed, Cancelled }
}