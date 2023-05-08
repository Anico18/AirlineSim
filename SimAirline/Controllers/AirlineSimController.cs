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
            var iterationFc = 0;
            var iterationPe = 0;
            var iterationE = 0;
            var emergencyiterationFc = 0;
            var emergencyiterationPe = 0;
            var emergencyiterationE = 0;

            var queryboarding = await _context.BoardingPasses
                .Include(m => m.Passenger)
                .Include(m => m.Purchase)
                .Include(m => m.Flight)
                .ThenInclude(m => m.Airplanes)
                .ThenInclude(m => m.Seats)
                .Where(m => m.FlightId == flyRequest.id)
                .ToListAsync();

            var flightData = _context.Flights.FirstOrDefault(m => m.FlightId == flyRequest.id);
            var economyClass = new List<Seat>();
            if (flightData != null)
            {
                var firstClass = await _context.Seats.Where(m => m.AirplaneId == flightData.AirplaneId && m.SeatTypeId == 1).ToListAsync();
                var emergencyFc = firstClass.OrderByDescending(m => m.SeatId).ToList();

                var premiumEconomy = await _context.Seats.Where(m => m.AirplaneId == flightData.AirplaneId && m.SeatTypeId == 2).ToListAsync();
                var emergencyPe = premiumEconomy.OrderByDescending(m => m.SeatId).ToList();

                var firstEconomySeat = await _context.Seats.FirstOrDefaultAsync(m => m.SeatTypeId == 3 && m.AirplaneId == flightData.AirplaneId);

                if(firstEconomySeat != null)
                {
                    var seatcolumns = await _context.Seats.Where(m => m.AirplaneId == flightData.AirplaneId).Select(m => m.SeatColumn).Distinct().ToListAsync();
                    
                    foreach(var seatColumn in seatcolumns)
                    {
                        var economyCol = await _context.Seats.Where(m => m.AirplaneId == flightData.AirplaneId && m.SeatColumn == seatColumn && m.SeatRow >= firstEconomySeat.SeatRow).ToListAsync();
                        economyClass.AddRange(economyCol);
                    }

                    //var economyG = await _context.Seats.Where(m => m.AirplaneId == flightData.AirplaneId && m.SeatColumn == "G" && m.SeatRow >= firstEconomySeat.SeatRow).ToListAsync();
                    //var economyF = await _context.Seats.Where(m => m.AirplaneId == flightData.AirplaneId && m.SeatColumn == "F" && m.SeatRow >= firstEconomySeat.SeatRow).ToListAsync();
                    //var economyE = await _context.Seats.Where(m => m.AirplaneId == flightData.AirplaneId && m.SeatColumn == "E" && m.SeatRow >= firstEconomySeat.SeatRow).ToListAsync();
                    //var economyC = await _context.Seats.Where(m => m.AirplaneId == flightData.AirplaneId && m.SeatColumn == "C" && m.SeatRow >= firstEconomySeat.SeatRow).ToListAsync();
                    //var economyB = await _context.Seats.Where(m => m.AirplaneId == flightData.AirplaneId && m.SeatColumn == "B" && m.SeatRow >= firstEconomySeat.SeatRow).ToListAsync();
                    //var economyA = await _context.Seats.Where(m => m.AirplaneId == flightData.AirplaneId && m.SeatColumn == "A" && m.SeatRow >= firstEconomySeat.SeatRow).ToListAsync();

                    //economyClass.AddRange(economyG);
                    //economyClass.AddRange(economyF);
                    //economyClass.AddRange(economyE);
                    //economyClass.AddRange(economyC);
                    //economyClass.AddRange(economyB);
                    //economyClass.AddRange(economyA);

                    var emergencyE = economyClass.OrderByDescending(m => m.SeatId).ToList();
                    

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
                                    if(item.SeatId != null)
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
                                                        if (item.SeatTypeId == 1)
                                                        {
                                                            item.SeatId = item.SeatId;

                                                            if (item.SeatId == null)
                                                            {
                                                                item.SeatId = emergencyFc[emergencyiterationFc].SeatId;
                                                                emergencyiterationFc++;
                                                            }

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
                                                            Seat seatToRemoveFc = firstClass.FirstOrDefault(m => m.SeatId == item.SeatId);
                                                            if (seatToRemoveFc != null)
                                                            {
                                                                firstClass.Remove(seatToRemoveFc);
                                                            }
                                                        }
                                                        else if (item.SeatTypeId == 2)
                                                        {
                                                            item.SeatId = item.SeatId;

                                                            if (item.SeatId == null)
                                                            {
                                                                item.SeatId = emergencyPe[emergencyiterationPe].SeatId;
                                                                emergencyiterationPe++;
                                                            }

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
                                                            Seat seatToRemovePe = premiumEconomy.FirstOrDefault(m => m.SeatId == item.SeatId);
                                                            if (seatToRemovePe != null)
                                                            {
                                                                premiumEconomy.Remove(seatToRemovePe);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            item.SeatId = item.SeatId;

                                                            if (item.SeatId == null)
                                                            {
                                                                item.SeatId = emergencyE[emergencyiterationE].SeatId;
                                                                emergencyiterationE++;
                                                            }

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
                                                            Seat seatToRemoveE = economyClass.FirstOrDefault(m => m.SeatId == item.SeatId);
                                                            if (seatToRemoveE != null)
                                                            {
                                                                economyClass.Remove(seatToRemoveE);
                                                            }
                                                        }
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
                                                            if (obj.SeatTypeId == 1)
                                                            {
                                                                obj.SeatId = obj.SeatId;

                                                                if (obj.SeatId == null)
                                                                {
                                                                    obj.SeatId = emergencyFc[emergencyiterationFc].SeatId;
                                                                    emergencyiterationFc++;
                                                                }

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
                                                                Seat seatToRemoveFc = firstClass.FirstOrDefault(m => m.SeatId == obj.SeatId);
                                                                if (seatToRemoveFc != null)
                                                                {
                                                                    firstClass.Remove(seatToRemoveFc);
                                                                }
                                                            }
                                                            else if (obj.SeatTypeId == 2)
                                                            {
                                                                obj.SeatId = obj.SeatId;

                                                                if (obj.SeatId == null)
                                                                {
                                                                    obj.SeatId = emergencyPe[emergencyiterationPe].SeatId;
                                                                    emergencyiterationPe++;
                                                                }

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
                                                                Seat seatToRemovePe = premiumEconomy.FirstOrDefault(m => m.SeatId == obj.SeatId);
                                                                if (seatToRemovePe != null)
                                                                {
                                                                    premiumEconomy.Remove(seatToRemovePe);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                obj.SeatId = obj.SeatId;

                                                                if (obj.SeatId == null)
                                                                {
                                                                    obj.SeatId = emergencyE[emergencyiterationE].SeatId;
                                                                    emergencyiterationE++;
                                                                }

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
                                                                Seat seatToRemoveE = economyClass.FirstOrDefault(m => m.SeatId == obj.SeatId);
                                                                if (seatToRemoveE != null)
                                                                {
                                                                    economyClass.Remove(seatToRemoveE);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                    }
                                    else
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
                                                        if (item.SeatTypeId == 1)
                                                        {
                                                            item.SeatId = firstClass[iterationFc].SeatId;

                                                            if(item.SeatId == null)
                                                            {
                                                                item.SeatId = emergencyFc[emergencyiterationFc].SeatId;
                                                                emergencyiterationFc++;
                                                            }

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
                                                            iterationFc++;
                                                        }
                                                        else if (item.SeatTypeId == 2)
                                                        {
                                                            item.SeatId = premiumEconomy[iterationPe].SeatId;

                                                            if (item.SeatId == null)
                                                            {
                                                                item.SeatId = emergencyPe[emergencyiterationPe].SeatId;
                                                                emergencyiterationPe++;
                                                            }

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
                                                            iterationPe++;
                                                        }
                                                        else
                                                        {
                                                            item.SeatId = economyClass[iterationE].SeatId;

                                                            if (item.SeatId == null)
                                                            {
                                                                item.SeatId = emergencyE[emergencyiterationE].SeatId;
                                                                emergencyiterationE++;
                                                            }

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
                                                            iterationE++;
                                                        }
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
                                                            if (obj.SeatTypeId == 1)
                                                            {
                                                                obj.SeatId = firstClass[iterationFc].SeatId;
                                                                
                                                                if (obj.SeatId == null)
                                                                {
                                                                    obj.SeatId = emergencyFc[emergencyiterationFc].SeatId;
                                                                    emergencyiterationFc++;
                                                                }

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
                                                                iterationFc++;
                                                            }
                                                            else if (obj.SeatTypeId == 2)
                                                            {
                                                                obj.SeatId = premiumEconomy[iterationPe].SeatId;

                                                                if (obj.SeatId == null)
                                                                {
                                                                    obj.SeatId = emergencyPe[emergencyiterationPe].SeatId;
                                                                    emergencyiterationPe++;
                                                                }

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
                                                                iterationPe++;
                                                            }
                                                            else
                                                            {
                                                                obj.SeatId = economyClass[iterationE].SeatId;

                                                                if (obj.SeatId == null)
                                                                {
                                                                    obj.SeatId = emergencyE[emergencyiterationE].SeatId;
                                                                    emergencyiterationE++;
                                                                }

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
                                                                iterationE++;
                                                            }
                                                        }
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
                                return BadRequest(new
                                {
                                    code = "400",
                                    errors = "could not connect to the db"
                                });
                            }

                            responseFly.code = 200;
                            responseFly.data = responseData;

                            return Ok(responseFly);
                        }

                    }
                }

                return Ok(new
                {
                    code = "404",
                    data = "{}"
                });
            }

            return Ok(new
            {
                code = "404",
                data = "{}"
            });
        }
    }
}
