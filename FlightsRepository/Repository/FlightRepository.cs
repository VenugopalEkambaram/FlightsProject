using System.Collections.Generic;

namespace FlightsRepository
{
    public class FlightRepository : IFlightRepository
    {        
        
        public List<Flight> Get()
        {
            return  DataProvider.Get();
        }

        public List<Flight> GetByGate(int GateId)
        {
            return DataProvider.GetByGate(GateId);
        }

        public bool Add(Flight flight)
        {
            return DataProvider.Add(flight);
        }

        public bool Update(Flight flight)
        {
            return DataProvider.Update(flight);
        }
    }
}