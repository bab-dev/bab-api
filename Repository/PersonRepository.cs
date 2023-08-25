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
    public class PersonRepository : RepositoryBase<Person>, IPersonRepository
    {
        public PersonRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public IEnumerable<Person> GetAllPeople()
        {
            return FindAll()
                .OrderBy(person => person.FirstName)
                .ToList();
        }

        public Person GetPersonById(Guid idPerson)
        {
            return FindByCondition(person => person.Id.Equals(idPerson))
                    .FirstOrDefault();
        }

        public Person GetPersonByEmail(string email)
        {
            return FindByCondition(person => person.Email.Equals(email))
                    .FirstOrDefault();
        }

        public void CreatePerson(Person person)
        {
            person.Id = Guid.NewGuid();
            Create(person);
            Save();
        }

        public void UpdatePerson(Person person)
        {
            Update(person);
            Save();
        }

        public void DeletePerson(Person person)
        {
            Delete(person);
            Save();
        }
    }
}
