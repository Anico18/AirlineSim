using SimAirline.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimAirline.Responses;
using SimAirline.Request;
using System.Collections.Generic;
using System.Collections;

namespace SimAirline.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AirlineSimController : ControllerBase
    {
        private readonly DBEFContext _context;

        public AirlineSimController(DBEFContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Index(FlyRequest flyRequest)
        {
            var boarding = await _context.BoardingPasses.ToListAsync();
            var purchase = await _context.Purchases.ToListAsync();
            var seat = await _context.Seats.ToListAsync();


            var queryboarding = await _context.BoardingPasses
                .Include(m => m.Passenger)
                .Include(m => m.Purchase)
                .Include(m => m.Flight)
                .ThenInclude(m => m.Airplanes)
                .ThenInclude(m => m.Seats)
                .Where(m => m.FlightId == flyRequest.id)
                .ToListAsync();

            var flightData = _context.Flights.FirstOrDefault(m => m.FlightId == flyRequest.id);

            List<int> processedBoardingPasses = new List<int>();

            ResponseFly responseFly = new ResponseFly();
            ResponseData responseData = new ResponseData();

            if (flightData != null)
            {
                var airplaneInUse = flightData.AirplaneId;
                var firstSeatId = _context.Seats.FirstOrDefault(m => m.AirplaneId == airplaneInUse);

                if (firstSeatId != null)
                {
                    int seatIncremental = firstSeatId.SeatId;
                    if (queryboarding != null)
                    {
                        foreach (var item in queryboarding)
                        {
                            if (item.Purchase != null)
                            {
                                if (item.Purchase.BoardingPasses.Count == 1)
                                {
                                    if (processedBoardingPasses.Contains(item.BoardingPassId))
                                    {
                                    }
                                    else
                                    {
                                        if (item.Passenger != null)
                                        {
                                            item.SeatId = seatIncremental;
                                            seatIncremental++;

                                            ResponsePassenger responsePassenger = new ResponsePassenger();
                                            responsePassenger.passengerId = item.Passenger.PassengerId;
                                            responsePassenger.dni = item.Passenger.Dni ?? "";
                                            responsePassenger.name = item.Passenger.Name ?? "";
                                            responsePassenger.age = item.Passenger.Age;
                                            responsePassenger.country = item.Passenger.Country ?? "";
                                            responsePassenger.boardingPassId = item.BoardingPassId;
                                            responsePassenger.purchaseId = item.PurchaseId;
                                            responsePassenger.seatTypeId = item.SeatTypeId;
                                            responsePassenger.seatId = item.SeatId;

                                            responseData.passengers.Add(responsePassenger);
                                            processedBoardingPasses.Add(item.BoardingPassId);
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var obj in item.Purchase.BoardingPasses)
                                    {
                                        if (processedBoardingPasses.Contains(obj.BoardingPassId))
                                        {
                                        }
                                        else
                                        {
                                            if (item.Passenger != null)
                                            {
                                                obj.SeatId = seatIncremental;
                                                seatIncremental++;

                                                ResponsePassenger responsePassenger = new ResponsePassenger();
                                                responsePassenger.passengerId = obj.Passenger.PassengerId;
                                                responsePassenger.dni = obj.Passenger.Dni ?? "";
                                                responsePassenger.name = obj.Passenger.Name ?? "";
                                                responsePassenger.age = obj.Passenger.Age;
                                                responsePassenger.country = obj.Passenger.Country ?? "";
                                                responsePassenger.boardingPassId = obj.BoardingPassId;
                                                responsePassenger.purchaseId = obj.PurchaseId;
                                                responsePassenger.seatTypeId = obj.SeatTypeId;
                                                responsePassenger.seatId = obj.SeatId;

                                                responseData.passengers.Add(responsePassenger);
                                                processedBoardingPasses.Add(obj.BoardingPassId);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        responseData.flightId = flyRequest.id;
                        responseData.takeoffDateTime = flightData.TakeoffDateTime;
                        responseData.takeoffAirport = flightData.TakeoffAirport ?? "";
                        responseData.landingDateTime = flightData.LandingDateTime;
                        responseData.landingAirport = flightData.LandingAirport ?? "";
                        responseData.airplaneId = flightData.AirplaneId;
                    }
                    else
                    {
                        return BadRequest(new {
                            code = "400",
                            errors = "could not connect to the db"
                        });
                    }

                    responseFly.code = 200;
                    responseFly.data = responseData;

                    return Ok(responseFly);
                }
            }

            return Ok(new {
                code = "404",
                data = "{}"
            });
        }
    }
}
