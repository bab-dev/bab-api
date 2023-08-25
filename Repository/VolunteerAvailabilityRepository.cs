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
    public class VolunteerAvailabilityRepository : RepositoryBase<VolunteerAvailability>, IVolunteerAvailabilityRepository
    {
        private readonly PersonRepository _personRepository;
        private static readonly Random getRandom = new Random();

        public VolunteerAvailabilityRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
            _personRepository = new PersonRepository(repositoryContext);
        }

        public IEnumerable<VolunteerAvailability> GetAllVolunteerAvailabilities()
        {
            return FindAll()
                .OrderBy(volunteerAvailability => volunteerAvailability.Id)
                .ToList();
        }

        public VolunteerAvailability GetAvailabilityByVolunteerId(int idVolunteer)
        {
            return FindByCondition(volunteerAvailability => volunteerAvailability.IDVolunteer.Equals(idVolunteer))
                    .FirstOrDefault();
        }

        public void CreateVolunteerAvailability(VolunteerAvailability volunteerAvailability)
        {
            volunteerAvailability.Id = Guid.NewGuid();
            Create(volunteerAvailability);
            Save();
        }

        public void UpdateVolunteerAvailability(VolunteerAvailability volunteerAvailability)
        {
            Update(volunteerAvailability);
            Save();
        }

        public void DeleteVolunteerAvailability(VolunteerAvailability volunteerAvailability)
        {
            Delete(volunteerAvailability);
            Save();
        }
    }
}

