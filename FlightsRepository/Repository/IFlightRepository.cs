using System.Collections.Generic;

namespace FlightsRepository
{
    public interface IFlightRepository
    {
        List<Flight> Get();
        List<Flight> GetByGate(int Id);
        bool Add(Flight flight);
        bool Update(Flight flight);
    }
}