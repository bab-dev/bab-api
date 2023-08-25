using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.Trip
{
    public class TripForCreationAndUpdateDTO
    {
        public int IDCoordinator { get; set; }
        public Guid IDDepartment { get; set; }
        public string Vehicule { get; set; }
        public DateTime Date { get; set; }
        public int NumOfPassengers { get; set; }
        public int TransportType { get; set; }
        public int DepartureType { get; set; }
        public Guid DepartureIDPlace { get; set; }

        [RegularExpression(@"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$",
        ErrorMessage = "DepartureTime must be in the format HH:MM")]
        public string DepartureTime { get; set; }
        public decimal InitialKm { get; set; }
        public int ArrivalType { get; set; }
        public Guid ArrivalIDPlace { get; set; }

        [RegularExpression(@"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$",
        ErrorMessage = "ArrivalTime must be in the format HH:MM")]
        public string ArrivalTime { get; set; }
        public decimal FinalKm { get; set; }
    }
}
