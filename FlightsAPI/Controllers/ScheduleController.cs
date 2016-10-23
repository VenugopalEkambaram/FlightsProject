using System;
using System.Net;
using System.Web.Http;
using FlightsRepository;
using System.Web.Http.Cors;
using FlightsApi.BusinessLayer;

namespace FlightsApi.Controllers
{
    [RoutePrefix("api/schedule")]
    [EnableCors(origins: "http://localhost:51384", headers: "*", methods: "*")]
    public class ScheduleController : ApiController
    {
        private readonly IScheduleManager _scheduleManager;

        public ScheduleController(IScheduleManager ScheduleManager)
        {            
            _scheduleManager = ScheduleManager;
        }

        public IHttpActionResult Get()
        {
            return Content(HttpStatusCode.OK, _scheduleManager.Get());
        }

        public IHttpActionResult Get(int Gate)
        {
            return Content(HttpStatusCode.OK, _scheduleManager.Get(Gate));
        }

        public IHttpActionResult Post([FromBody]Flight flight)
        {
            if (string.IsNullOrEmpty(flight.FlightNumber) || flight.Gate <= 0 || flight.Arrival.Day != DateTime.Now.Day || flight.Departure.Day != DateTime.Now.Day)
                return Content(HttpStatusCode.BadRequest, "Invalid flight input.");
            
            if (_scheduleManager.Get(flight.FlightNumber) != null)
                return Content(HttpStatusCode.BadRequest, "Flight already exists.");

            var flightAdded = _scheduleManager.AllocateSchedule(flight, true);
            return Content(HttpStatusCode.Created, flightAdded);
        }

        public IHttpActionResult Put([FromBody]Flight flight)
        {
            if (string.IsNullOrEmpty(flight.FlightNumber) || flight.Gate <= 0 || flight.Arrival.Day != DateTime.Now.Day || flight.Departure.Day != DateTime.Now.Day)
                return Content(HttpStatusCode.BadRequest, "Invalid flight input.");

            var flightUpdated = _scheduleManager.AllocateSchedule(flight, false);
            return Content(HttpStatusCode.Accepted, flightUpdated);

        }
    }
}

