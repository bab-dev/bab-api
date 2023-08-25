using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class EmergencyContactRepository : RepositoryBase<EmergencyContact>, IEmergencyContactRepository
    {

        public EmergencyContactRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public IEnumerable<EmergencyContact> GetAllEmergencyContacts()
        {
            return FindAll()
                .OrderBy(emergencyContact => emergencyContact.Id)
                .ToList();
        }

        public EmergencyContact GetEmergencyContactByVolunteerId(int idVolunteer)
        {
            return FindByCondition(volunteer => volunteer.IDVolunteer.Equals(idVolunteer))
                    .FirstOrDefault();
        }

        public void CreateEmergencyContact(EmergencyContact emergencyContact)
        {
            emergencyContact.Id = Guid.NewGuid();
            Create(emergencyContact);
            Save();
        }

        public void UpdateEmergencyContact(EmergencyContact emergencyContact)
        {
            Update(emergencyContact);
            Save();
        }

        public void DeleteEmergencyContact(EmergencyContact emergencyContact)
        {
            Delete(emergencyContact);
            Save();
        }
    }
}
