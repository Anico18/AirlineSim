using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace SimAirline.Responses
{
    public class ResponseData
    {
        public ResponseData() { 
            passengers = new List<ResponsePassenger>();
        }

        public int flightId { get; set; }
        public int? takeoffDateTime { get; set; }
        public string takeoffAirport { get; set; }
        public int? landingDateTime { get; set; }
        public string landingAirport { get; set; }
        public int? airplaneId { get; set; }
        public List<ResponsePassenger> passengers { get; set; }
    }

    public class ResponsePassenger
    {
        public int passengerId { get; set; }
        public string dni { get; set; } = null!;
        public string name { get; set; } = null!;
        public int? age { get; set; }
        public string country { get; set; } = null!;
        public int boardingPassId { get; set; }
        public int? purchaseId { get; set; }
        public int? seatTypeId { get; set; }
        public int? seatId { get; set; }
    }

    public class ResponseFly
    {
        public int code { get; set; }
        public ResponseData data { get; set; } = null!;
    }
}
