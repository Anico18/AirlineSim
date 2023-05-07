using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimAirplane.Models;
using System.Diagnostics;

namespace SimAirplane.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        private readonly DBEFContext _context;

        public HomeController(DBEFContext context)
        {
            _context = context;
        }


        public IActionResult Index(int id)
        {
            var boarding = _context.BoardingPasses.ToList();
            var airplane = _context.Airplanes.ToList();
            var flight = _context.Flights.ToList();
            var passenger = _context.Passengers.ToList();
            var purchase = _context.Purchases.ToList();
            var seat = _context.Seats.ToList();
            var seattype = _context.SeatTypes.ToList();

            var detail8 = _context.BoardingPasses.Include(m => m.Flight).Where(m => m.SeatTypeId == 1 && m.FlightId == 1).ToList();
            var detail8a = _context.BoardingPasses.Include(m => m.Flight).Where(m => m.SeatTypeId == 2 && m.FlightId == 1).ToList();
            var detail8b = _context.BoardingPasses.Include(m => m.Flight).Where(m => m.SeatTypeId == 3 && m.FlightId == 1).ToList();

            var detail9 = _context.BoardingPasses.Include(m => m.Flight).Where(m => m.SeatTypeId == 1 && m.FlightId == 2).ToList();
            var detail9a = _context.BoardingPasses.Include(m => m.Flight).Where(m => m.SeatTypeId == 2 && m.FlightId == 2).ToList();
            var detail9b = _context.BoardingPasses.Include(m => m.Flight).Where(m => m.SeatTypeId == 3 && m.FlightId == 2).ToList();

            var detail10 = _context.BoardingPasses.Include(m => m.Flight).Where(m => m.SeatTypeId == 1 && m.FlightId == 3).ToList();
            var detail10a = _context.BoardingPasses.Include(m => m.Flight).Where(m => m.SeatTypeId == 2 && m.FlightId == 3).ToList();
            var detail1b= _context.BoardingPasses.Include(m => m.Flight).Where(m => m.SeatTypeId == 3 && m.FlightId == 3).ToList();

            var detail11 = _context.BoardingPasses.Include(m => m.Flight).Where(m => m.SeatTypeId == 1 && m.FlightId == 4).ToList();
            var detail11a = _context.BoardingPasses.Include(m => m.Flight).Where(m => m.SeatTypeId == 2 && m.FlightId == 4).ToList();
            var detail11b= _context.BoardingPasses.Include(m => m.Flight).Where(m => m.SeatTypeId == 3 && m.FlightId == 4).ToList();



            //--------------------------------------------------------------------------------------------------------------------------

            //var queryboarding = _context.BoardingPasses.Where(m => m.FlightId == id);

            //foreach (var item in queryboarding)
            //{
            //    foreach(var buy in flight)




            //    if(item.Passenger.Age < 18)
            //    {

            //    }
            //}

            


            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}