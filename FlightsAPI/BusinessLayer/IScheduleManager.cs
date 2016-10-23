using FlightsRepository;
using System.Collections.Generic;

namespace FlightsApi.BusinessLayer
{
    public interface IScheduleManager
    {
        List<Flight> Get();
        List<Flight> Get(int Gate);
        Flight Get(string FlightNumber);
        List<Flight> AllocateSchedule(Flight flight, bool IsNew);
    }
}