using Entities.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    [Table("trip")]
    public class Trip
    {
        [Key]
        [Column("IDTrip")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "IDCoordinator is required")]
        [ForeignKey(nameof( Volunteer))]
        public int IDCoordinator { get; set; }

        [Required(ErrorMessage = "IDDepartment is required")]
        [ForeignKey(nameof(Department))]
        public Guid IDDepartment { get; set; }

        [Required(ErrorMessage = "Vehicule is required")]
        public string Vehicule { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "NumberOfPassengers is required")]
        public int NumOfPassengers { get; set; }

        [Required(ErrorMessage = "TransportType is required")]
        public TransportType TransportType { get; set; }

        [Required(ErrorMessage = "DepartureType is required")]
        public PlaceType DepartureType { get; set; }

        public Guid DepartureIDPlace { get; set; }

        [Required(ErrorMessage = "DeparturePlace is required")]
        public string DeparturePlace { get; set; }

        [Column(TypeName = "time")]
        [Required(ErrorMessage = "DepartureTime is required")]
        public TimeSpan DepartureTime { get; set; }

        [Required(ErrorMessage = "InitialKm is required")]
        public decimal InitialKm { get; set; }

        [Required(ErrorMessage = "ArrivalType is required")]
        public PlaceType ArrivalType { get; set; }

        public Guid ArrivalIDPlace { get; set; }

        [Required(ErrorMessage = "ArrivalPlace is required")]
        public string ArrivalPlace { get; set; }

        [Column(TypeName = "time")]
        [Required(ErrorMessage = "ArrivaTime is required")]
        public TimeSpan ArrivalTime { get; set; }

        [Required(ErrorMessage = "FinalKm is required")]
        public decimal FinalKm { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal TotalKm { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string TotalTime { get; set; }
    }
}
