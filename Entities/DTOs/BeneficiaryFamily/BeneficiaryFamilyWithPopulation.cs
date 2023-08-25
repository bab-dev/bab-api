using Entities.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.BeneficiaryFamily
{
    public class BeneficiaryFamilyWithPopulation
    {
        public Guid Id { get; set; }
        public Guid IDPerson { get; set; }

        //Person
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string FirstSurname { get; set; }
        public string SecondSurname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public int PhoneNumber { get; set; }
        public string Address { get; set; }
        public int CI { get; set; }

        //Beneficiary
        public int BeneficiaryType { get; set; }
        public HousingType HousingType { get; set; }
        public string HousingTypeName { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Zone { get; set; }
        public string Observations { get; set; }

        //House members
        public int TotalPopulation { get; set; }
        public int MemberAgesBetween0And2Years { get; set; }
        public int MemberAgesBetween3And5Years { get; set; }
        public int MemberAgesBetween6And18Years { get; set; }
        public int MemberAgesBetween19And49Years { get; set; }
        public int MemberAgesOver50Years { get; set; }
        public int TotalMembersWithDisabilities { get; set; }
    }
}
