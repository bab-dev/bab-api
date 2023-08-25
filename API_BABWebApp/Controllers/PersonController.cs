using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_BABWebApp.Controllers
{
    [Route("api/people")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repository;
        private IMapper _mapper;
        public PersonController(ILoggerManager logger, IRepositoryWrapper repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        private int GetAge(DateTime bday)
        {
            DateTime now = DateTime.Today;
            int age = now.Year - bday.Year;
            if (bday.AddYears(age) > now)
                age--;
            return age;
        }

        /// <summary>
        /// Gets the list of all People.
        /// </summary>
        /// <returns>The list of People.</returns>
        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            try
            {
                var people = _repository.Person.GetAllPeople();
                _logger.LogInfo($"Returned all people from database.");
                var peopleResult = _mapper.Map<IEnumerable<PersonDTO>>(people);

                foreach (var person in peopleResult)
                {
                    person.Age = GetAge(person.DateOfBirth);
                }

                return Ok(peopleResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAllPeople action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets a Person by Id.
        /// </summary>
        /// <param name="id"></param>  
        [HttpGet("{id}", Name = "PersonById")]
        [Authorize]
        public IActionResult GetPersonById(Guid id)
        {
            try
            {
                var person = _repository.Person.GetPersonById(id);
                if (person is null)
                {
                    _logger.LogError($"Person with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned person with id: {id}");
                    var personResult = _mapper.Map<PersonDTO>(person);
                    personResult.Age = GetAge(person.DateOfBirth);

                    return Ok(personResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetPersonById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        private bool PersonAlreadyExists(PersonForCreationAndUpdateDTO personDTO)
        {
            var people = _repository.Person.GetAllPeople();
            var personResult = _mapper.Map<Person>(personDTO);
            return people.Any(p => p == personResult);
        }

        /// <summary>
        /// Creates a Person.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/Person
        ///     {        
        ///       "IDUser": "15",
        ///       "FirstName": "Juan",
        ///       "MiddleName": "David",
        ///       "FirstSurname": "Osorio",
        ///       "SecondSurname": "Rojas",
        ///       "DateOfBirth": "2004-03-08T00:00:00",
        ///       "Address": "Calle Juan Capriles y Av. Libertador Bolivar",
        ///       "City": "Cochabamba",
        ///       "Email": "juan.david.or.47@gmail.com",
        ///       "PhoneNumber": 67427045,
        ///       "Occupation": "Estudiante",
        ///       "CI": 7439540     
        ///     }
        /// </remarks>
        /// <param name="person"></param>  
        [Produces("application/json")]
        [HttpPost]
        [Authorize]
        public IActionResult CreatePerson([FromBody] PersonForCreationAndUpdateDTO person)
        {
            try
            {
                if (person is null)
                {
                    _logger.LogError("Person object sent from client is null.");
                    return BadRequest("Person object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid person object sent from client.");
                    return BadRequest("Invalid model object");
                }
                if (PersonAlreadyExists(person))
                {
                    _logger.LogError($"Person with same personal data has been already registered in db.");
                    return BadRequest("Person has already been registered");
                }
                var personEntity = _mapper.Map<Person>(person);
                _repository.Person.CreatePerson(personEntity);
                _repository.Save();
                var createdPerson = _mapper.Map<PersonDTO>(personEntity);
                return CreatedAtRoute("PersonById", new { id = createdPerson.Id }, createdPerson);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreatePerson action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Updates a Person.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT api/Person/8a1919a7-9202-41ae-9eac-978ff992a589
        ///     {  
        ///       "IDUser": "15",
        ///       "FirstName": "Juan",
        ///       "MiddleName": "David",
        ///       "FirstSurname": "Osorio",
        ///       "SecondSurname": "Lopez",
        ///       "DateOfBirth": "2004-03-08T00:00:00",
        ///       "Address": "Av. America y Calle Potosi",
        ///       "City": "Cochabamba",
        ///       "Email": "juan.david.or.47@gmail.com",
        ///       "PhoneNumber": 67421235,
        ///       "Occupation": "Estudiante",
        ///       "CI": 7447540     
        ///     }
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="person"></param> 
        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdatePerson(Guid id, [FromBody] PersonForCreationAndUpdateDTO person)
        {
            try
            {
                if (person is null)
                {
                    _logger.LogError("Person object sent from client is null.");
                    return BadRequest("Person object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid person object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var personEntity = _repository.Person.GetPersonById(id);
                if (personEntity is null)
                {
                    _logger.LogError($"Person with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(person, personEntity);
                _repository.Person.UpdatePerson(personEntity);
                _repository.Save();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdatePerson action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Deletes a Person.
        /// </summary>
        /// <param name="id"></param>  
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeletePerson(Guid id)
        {
            try
            {
                var person = _repository.Person.GetPersonById(id);
                if (person == null)
                {
                    _logger.LogError($"Person with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _repository.Person.DeletePerson(person);
                _repository.Save();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeletePerson action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
