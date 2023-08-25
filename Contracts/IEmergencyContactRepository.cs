using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IEmergencyContactRepository : IRepositoryBase<EmergencyContact>
    {
        IEnumerable<EmergencyContact> GetAllEmergencyContacts();
        EmergencyContact GetEmergencyContactByVolunteerId(int idVolunteer);
        void CreateEmergencyContact(EmergencyContact emergencyContact);
        void UpdateEmergencyContact(EmergencyContact emergencyContact);
        void DeleteEmergencyContact(EmergencyContact emergencyContact);
    }
}
