using FlightsRepository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlightsApi.BusinessLayer
{
    public class ScheduleManager : IScheduleManager
    {
        private readonly IFlightRepository _flightRepo;
        private readonly List<Flight> ModifiedFlights = new List<Flight>();

        public ScheduleManager(IFlightRepository FlightRepo)
        {
            _flightRepo = FlightRepo;
        }

        public List<Flight> Get()
        {
            return _flightRepo.Get();
        }

        public Flight Get(string FlightNumber)
        {
            return _flightRepo.Get().Where(x => x.FlightNumber.Equals(FlightNumber)).FirstOrDefault();
        }

        public List<Flight> Get(int Gate)
        {
            return _flightRepo.GetByGate(Gate);
        }

        public List<Flight> AllocateSchedule(Flight Flight, bool IsNew)
        {
            if (!IsNew && CheckIfGateAloneChanged(Flight))
            {
                PushSchedule(Flight, IsNew);
                return ModifiedFlights;
            }
            
            var AvailableGates = GetAvailableGates(Flight);
            if (AvailableGates.Count != 0)
            {
                Flight.Gate = AvailableGates.First();                
                SaveChangesToRepo(Flight, IsNew);
                return ModifiedFlights;
            }

            PushSchedule(Flight, IsNew);
            return ModifiedFlights;
        }

        private void PushSchedule(Flight Flight, bool IsNew)
        {
            var AllFlights = _flightRepo.Get();
            DateTime StartTime = Flight.Arrival;
            DateTime EndTime = Flight.Arrival.AddMinutes(30);

            SaveChangesToRepo(Flight, IsNew);

            //Get the flights specific to given gate, whose arrival and departure time need to be pushed due to overlap
            var AffectedFlights = AllFlights.Where(x => x.Gate == Flight.Gate && ((x.Arrival >= StartTime && x.Arrival <= EndTime) ||
                (x.Departure >= StartTime && x.Departure <= EndTime)) && x.FlightNumber != Flight.FlightNumber).OrderBy(x => x.Arrival).ToList();
            
            //To avoid repeatation of re-scheduling for the flights that are already done in this instance
            AffectedFlights = AffectedFlights.Except(ModifiedFlights).ToList();

            if (AffectedFlights.Count > 0)
            {
                var ImmediateFlight = AffectedFlights.First();
                ImmediateFlight.Arrival = StartTime.AddMinutes(30);
                ImmediateFlight.Departure = ImmediateFlight.Arrival.AddMinutes(29);

                //Recursive call is made to look for any next immediate flight that may also needs to be pushed. Its repeated until impacted flights is 0.
                PushSchedule(ImmediateFlight, false);
            }
        }

        private List<int> GetAvailableGates(Flight Flight)
        {
            var AllFlights = _flightRepo.Get();

            DateTime StartTime = Flight.Arrival;
            DateTime EndTime = Flight.Arrival.AddMinutes(30);

            var ArrivalBlockers = AllFlights.Where(x => x.FlightNumber != Flight.FlightNumber && x.Arrival >= StartTime && x.Arrival <= EndTime).ToList();
            var DepartureBlockers = AllFlights.Where(x => x.FlightNumber != Flight.FlightNumber && x.Departure >= StartTime && x.Departure <= EndTime).ToList();
            var TotalBlockers = ArrivalBlockers.Union(DepartureBlockers).ToList();

            var AllGates = AllFlights.Select(x => x.Gate).Distinct().ToList();
            var BlockedGates = TotalBlockers.Select(x => x.Gate).Distinct().ToList();
            var AvailableGates = AllGates.Except(BlockedGates).ToList();

            return AvailableGates;
        }

        private void SaveChangesToRepo(Flight Flight, bool IsNew)
        {
            if (IsNew)
                _flightRepo.Add(Flight);
            else
                _flightRepo.Update(Flight);
            ModifiedFlights.Add(Flight);
        }

        private bool CheckIfGateAloneChanged(Flight Flight)
        {
           var AllFlights = Get();
           int MatchCount = AllFlights.Where(x => x.FlightNumber == Flight.FlightNumber && x.Gate != Flight.Gate && x.Arrival == Flight.Arrival 
               && x.Departure == Flight.Departure && x.Status == Flight.Status).Count();

           if (MatchCount <= 0) 
               return false;
            
           return true;
        }
    }
}