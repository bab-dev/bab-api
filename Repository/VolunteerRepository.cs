using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Helpers;
using Entities.Models;
using Entities.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class VolunteerRepository : RepositoryBase<Volunteer>, IVolunteerRepository
    {
        private readonly PersonRepository _personRepository;
        private static readonly Random getRandom = new Random();
        private readonly ISortHelper<Volunteer> _sortHelper;

        public VolunteerRepository(RepositoryContext repositoryContext, ISortHelper<Volunteer> sortHelper)
            : base(repositoryContext)
        {
            _personRepository = new PersonRepository(repositoryContext);
            _sortHelper = sortHelper;
        }

        private void SearchByName(ref IQueryable<Volunteer> volunteers, string name)
        {
            if (!volunteers.Any() || string.IsNullOrWhiteSpace(name))
                return;

            IEnumerable<Guid> peopleIds = _personRepository.GetAllPeople().Where(person =>
                person.FirstName.ToLower().Contains(name.Trim().ToLower()) ||
                person.MiddleName.ToLower().Contains(name.Trim().ToLower()) ||
                person.FirstSurname.ToLower().Contains(name.Trim().ToLower()) ||
                person.SecondSurname.ToLower().Contains(name.Trim().ToLower()) ||
                (person.FirstName + " " + person.FirstSurname).ToLower().Contains(name.ToLower()) ||
                (person.MiddleName == null
                    ? (person.FirstName + " " + person.FirstSurname)
                    : (person.SecondSurname == null
                        ? (person.FirstName + " " + person.MiddleName + " " + person.FirstSurname)
                        : (person.FirstName + " " + person.MiddleName + " " + person.FirstSurname + " " + person.SecondSurname)
                       )
                ).ToLower().Contains(name.ToLower())
            ).Select(person => person.Id);

            volunteers = volunteers.Where(volunteer => peopleIds.Contains(volunteer.IDPerson));
        }


        public PagedList<Volunteer> GetAllVolunteers([Optional] VolunteerParameters parameters)
        {
            IQueryable<Volunteer> volunteers = FindByCondition(volunteer =>
                (volunteer.IDDepartment == parameters.IDDepartment || parameters.IDDepartment == null) &&
                ((int)volunteer.Role == parameters.Role || parameters.Role == null) &&
                (volunteer.IsActive == parameters.IsActive || parameters.IsActive == true)
            );

            if (parameters is not null)
            {
                SearchByName(ref volunteers, parameters.Name);
                var sortedVolunteers = _sortHelper.ApplySort(volunteers, parameters.OrderBy);

                if (parameters.PageSize.HasValue)
                {
                    return PagedList<Volunteer>.ToPagedList(sortedVolunteers, parameters.PageNumber, parameters.PageSize.Value);
                }
                else
                {
                    return PagedList<Volunteer>.ToPagedList(sortedVolunteers, 1, sortedVolunteers.Count());
                }
            } else
            {
                var sortedVolunteers = _sortHelper.ApplySort(volunteers, "Id");
                return PagedList<Volunteer>.ToPagedList(sortedVolunteers, 1, sortedVolunteers.Count());
            }
        }

        public Volunteer GetVolunteerById(int idVolunteer)
        {
            return FindByCondition(volunteer => volunteer.Id.Equals(idVolunteer))
                    .FirstOrDefault();
        }

        public Volunteer GetVolunteerByPersonId(Guid idPerson)
        {
            return FindByCondition(volunteer => volunteer.IDPerson.Equals(idPerson))
                    .FirstOrDefault();
        }

        public string GetVolunteerFullName(int idVolunteer)
        {
            var volunteer = GetVolunteerById(idVolunteer);
            if(volunteer is not null)
            {
                var person = _personRepository.GetPersonById(volunteer.IDPerson);
                if(person is not null)
                {
                    return person.GetFullName();
                }
            }
            return null;
        }

        private int GenerateRandomInt(int min, int max)
        {
            lock (getRandom)
            {
                return getRandom.Next(min, max);
            }
        }

        private int GenerateVolunteerId(Guid idPerson)
        {
            int volunteerCI = _personRepository.GetPersonById(idPerson).CI;
            string parsedCI = volunteerCI.ToString();
            int newId = volunteerCI;
            if (parsedCI.Length >= 6)
            {
                newId = Int32.Parse(parsedCI.Substring(parsedCI.Length - 6));
            }
            if (GetVolunteerById(newId) != null) // If exists another volunter with that ID
            {
                newId = GenerateRandomInt(100000, 999999);
            }
            return newId;
        }

        private int GetAge(DateTime bday)
        {
            DateTime now = DateTime.Today;
            int age = now.Year - bday.Year;
            if (bday.AddYears(age) > now)
                age--;
            return age;
        }

        private bool GetIsOlder(Guid idPerson)
        {
            DateTime bday =  _personRepository.GetPersonById(idPerson).DateOfBirth;
            int age = GetAge(bday);
            return age >= 18; 
        }

        public void CreateVolunteer(Volunteer volunteer)
        {
            volunteer.Id = GenerateVolunteerId(volunteer.IDPerson);
            volunteer.IsOlder = GetIsOlder(volunteer.IDPerson);
            volunteer.IsActive = true;
            Create(volunteer);
            Save();
        }

        public void UpdateVolunteer(Volunteer volunteer)
        {
            volunteer.IsOlder = GetIsOlder(volunteer.IDPerson);
            volunteer.IsActive = true;
            Update(volunteer);
            Save();
        }

        public void DeleteVolunteer(Volunteer volunteer)
        {
            Delete(volunteer);
            Save();
        }
    }
}
