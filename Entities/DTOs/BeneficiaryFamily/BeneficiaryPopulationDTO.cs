using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.BeneficiaryFamily
{
    public class BeneficiaryPopulationDTO
    {
        public Guid IDBeneficiary { get; set; }
        public int TotalPopulation { get; set; }
        public int MemberAgesBetween0And2Years { get; set; }
        public int MemberAgesBetween3And5Years { get; set; }
        public int MemberAgesBetween6And18Years { get; set; }
        public int MemberAgesBetween19And49Years { get; set; }
        public int MemberAgesOver50Years { get; set; }
        public int TotalMembersWithDisabilities { get; set; }
    }
}
