using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Entities.Models
{
    [Table("person")]
    public class Person
    {
        [Key]
        [Column("IDPerson")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        public  string MiddleName { get; set; }

        [Required(ErrorMessage = "First surname is required")]
        public  string FirstSurname { get; set; }

        public string SecondSurname { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        public int PhoneNumber { get; set; }

        [Required(ErrorMessage = "Occupation is required")]
        public string Occupation { get; set; }

        [Required(ErrorMessage = "CI is required")]
        public int CI { get; set; }

        public string GetFullName()
        {
            if(MiddleName != "")
            {
                return $"{FirstName} {MiddleName.Substring(0, 1)}. {FirstSurname} {SecondSurname}";
            }
            return $"{FirstName} {FirstSurname} {SecondSurname}";
        }

        /*public string GetParsedDateOfBirth()
        {
            return $"{DateOfBirth.Date.ToShortDateString()}";
        }*/
    }
}
