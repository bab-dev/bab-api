using API_BABWebApp.Extensions;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_BABWebApp.Controllers
{
    [Route("api/person-volunteers")]
    [ApiController]
    public class PersonVolunteerController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repository;
        private IMapper _mapper;
        public PersonVolunteerController(ILoggerManager logger, IRepositoryWrapper repository, IMapper mapper)
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
        /// Gets a PersonVolunteer by IdPerson.
        /// </summary>
        /// <param name="idPerson"></param> 
        [HttpGet("{idPerson}", Name = "PersonVolunteerByIdPerson")]
        public IActionResult GetPersonVolunteerById(Guid idPerson)
        {
            try
            {
                var person = _repository.Person.GetPersonById(idPerson);
                if (person is null)
                {
                    _logger.LogError($"Person with id: {idPerson}, hasn't been found in db.");
                    return NotFound();
                }

                var volunteer = _repository.Volunteer.GetVolunteerByPersonId(idPerson);
                if (volunteer is null)
                {
                    _logger.LogError($"Volunteer with IDPerson: {idPerson}, hasn't been found in db.");
                    return NotFound();
                }

                var volunteerAvailability = _repository.VolunteerAvailability.GetAvailabilityByVolunteerId(volunteer.Id);
                if (volunteerAvailability is null)
                {
                    _logger.LogError($"VolunteerAvailability with IDVolunteer: {volunteer.Id}, hasn't been found in db.");
                    return NotFound();
                }
                var emergencyContact = _repository.EmergencyContact.GetEmergencyContactByVolunteerId(volunteer.Id);
                if (emergencyContact is null)
                {
                    _logger.LogError($"EmergencyContact with IDVolunteer: {volunteer.Id}, hasn't been found in db.");
                    return NotFound();
                }

                else
                {
                    _logger.LogInfo($"Returned personVolunteer with idPerson: {idPerson}");

                    //PERSON
                    var personResult = _mapper.Map<PersonDTO>(person);
                    personResult.Age = GetAge(person.DateOfBirth);
                    //VOLUNTEER
                    var volunteerResult = _mapper.Map<VolunteerDTO>(volunteer);
                    volunteerResult.RoleName = volunteer.Role.ToString();
                    volunteerResult.CategoryName = volunteer.Category.ToString();
                    //VOLUNTEER AVAILABILITY
                    var volunteerAvailabilityResult = _mapper.Map<VolunteerAvailabilityDTO>(volunteerAvailability);
                    //EMERGENCY CONTACT
                    var emergencyContactResult = _mapper.Map<EmergencyContactDTO>(emergencyContact);
                    emergencyContactResult.Relationship = (int)emergencyContact.Relationship;

                    var personVolunteerResult = new PersonVolunteerDTO
                    {
                        Person = personResult,
                        Volunteer = volunteerResult,
                        VolunteerAvailability = volunteerAvailabilityResult,
                        EmergencyContact = emergencyContactResult
                    };
                    return Ok(personVolunteerResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetPersonVolunteerByIdPerson action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        private bool PersonAlreadyExists(PersonForCreationAndUpdateDTO personDTO)
        {
            var people = _repository.Person.GetAllPeople();
            var personResult = _mapper.Map<Person>(personDTO);
            return people.Any(p => p == personResult);
        }

        private bool DepartmentExists(Guid idDepartment)
        {
            Department departmentFound = _repository.Department.GetDepartmentById(idDepartment);
            return !(departmentFound is null);
        }

        private bool UserAlreadyExists(string email)
        {
            return _repository.User.GetUserByEmail(email) != null;
        }

        private string GenerateUserPassword(int idVolunteer)
        {
            return $"bab{idVolunteer}";
        }

        /// <summary>
        /// Creates a new PersonVolunteer.
        /// </summary>
        /// <param name="newPersonVolunteer"></param>  
        [Produces("application/json")]
        [HttpPost]
        [Authorize]
        public IActionResult CreatePersonVolunteer([FromBody] PersonVolunteerForCreationDTO newPersonVolunteer)
        {
            try
            {
                if (newPersonVolunteer is null)
                {
                    _logger.LogError("PersonVolunteer object sent from client is null.");
                    return BadRequest("PersonVolunteer object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid object sent from client.");
                    return BadRequest("Invalid model object");
                }
                //PERSON
                if (PersonAlreadyExists(newPersonVolunteer.Person))
                {
                    _logger.LogError($"Person with same personal data has been already registered in db.");
                    return BadRequest("Person has already been registered");
                }
                var personEntity = _mapper.Map<Person>(newPersonVolunteer.Person);
                _repository.Person.CreatePerson(personEntity);
                _repository.Save();
                var createdPerson = _mapper.Map<PersonDTO>(personEntity); //Contains the person Id

                //VOLUNTEER
                if (!DepartmentExists(newPersonVolunteer.Volunteer.IDDepartment))
                {
                    _logger.LogError($"Department with id: {newPersonVolunteer.Volunteer.IDDepartment}, hasn't been found in db.");
                    return NotFound("Department not found");
                }

                var volunteerEntity = _mapper.Map<Volunteer>(newPersonVolunteer.Volunteer);
                volunteerEntity.IDPerson = createdPerson.Id; //Setting IDPerson after creating the person object
                _repository.Volunteer.CreateVolunteer(volunteerEntity);
                _repository.Save();
                var createdVolunteer = _repository.Volunteer.GetVolunteerByPersonId(createdPerson.Id); //Contains the volunteer Id

                var mappedVolunteer = _mapper.Map<VolunteerDTO>(createdVolunteer);

                //VOLUNTEER AVAILABILITY
                var volunteerAvailabilityEntity = _mapper.Map<VolunteerAvailability>(newPersonVolunteer.VolunteerAvailability);
                volunteerAvailabilityEntity.IDVolunteer = createdVolunteer.Id; //Setting IDVolunteer after creating the volunteer object
                _repository.VolunteerAvailability.CreateVolunteerAvailability(volunteerAvailabilityEntity);
                _repository.Save();
                var createdVolunteerAvailability = _mapper.Map<VolunteerAvailabilityDTO>(volunteerAvailabilityEntity);

                //EMERGENCY CONTACT
                var emergencyContactEntity = _mapper.Map<EmergencyContact>(newPersonVolunteer.EmergencyContact);
                emergencyContactEntity.IDVolunteer = createdVolunteer.Id; //Setting IDVolunteer after creating the volunteer object
                _repository.EmergencyContact.CreateEmergencyContact(emergencyContactEntity);
                _repository.Save();
                var createdEmergencyContact = _mapper.Map<EmergencyContactDTO>(emergencyContactEntity);

                //USER 
                if (UserAlreadyExists(createdPerson.Email))
                {
                    _logger.LogError($"Email {createdPerson.Email} is already in use. User can't be created.");
                    return BadRequest("Email is already in use");
                }
                var generatedPassword = GenerateUserPassword(createdVolunteer.Id);
                var userEntity = _mapper.Map<User>(new UserForCreationAndLoginDTO{
                    Email = createdPerson.Email,
                    Password = generatedPassword
                });

                var salt = CryptoUtil.GenerateSalt();
                userEntity.IDPerson = createdPerson.Id;
                userEntity.Salt = salt;
                userEntity.Password = CryptoUtil.HashMultiple(generatedPassword, salt);

                _repository.User.CreateUser(userEntity);
                _repository.Save();

                var personVolunteerResult = new PersonVolunteerDTO
                {
                    Person = createdPerson,
                    Volunteer = mappedVolunteer,
                    VolunteerAvailability = createdVolunteerAvailability,
                    EmergencyContact = createdEmergencyContact
                };

                return CreatedAtRoute("PersonVolunteerByIdPerson", new { idPerson = createdPerson.Id }, personVolunteerResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreatePersonVolunteer action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


        /// <summary>
        /// Updates a PersonVolunteer.
        /// </summary>
        /// <param name="idPerson"></param>
        /// <param name="updatedPersonVolunteer"></param> 
        [HttpPut("{idPerson}")]
        [Authorize]
        public IActionResult UpdatePersonVolunteer(Guid idPerson, [FromBody] PersonVolunteerForUpdateDTO updatedPersonVolunteer)
        {
            try
            {
                if (updatedPersonVolunteer is null)
                {
                    _logger.LogError("PersonVolunteer object sent from client is null.");
                    return BadRequest("PersonVolunteer object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid object sent from client.");
                    return BadRequest("Invalid model object");
                }
                //PERSON
                var personEntity = _repository.Person.GetPersonById(idPerson);
                if (personEntity is null)
                {
                    _logger.LogError($"Person with id: {idPerson}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(updatedPersonVolunteer.Person, personEntity);
                _repository.Person.UpdatePerson(personEntity);
                _repository.Save();
                var updatedPerson = _mapper.Map<PersonDTO>(personEntity);
                updatedPerson.Age = GetAge(personEntity.DateOfBirth);

                //VOLUNTEER
                var volunteerEntity = _repository.Volunteer.GetVolunteerByPersonId(idPerson);
                if (volunteerEntity is null)
                {
                    _logger.LogError($"Volunteer with id: {idPerson}, hasn't been found in db.");
                    return NotFound();
                }
                if (!DepartmentExists(updatedPersonVolunteer.Volunteer.IDDepartment))
                {
                    _logger.LogError($"Department with id: {updatedPersonVolunteer.Volunteer.IDDepartment}, hasn't been found in db.");
                    return NotFound("Department not found in db");
                }
                _mapper.Map(updatedPersonVolunteer.Volunteer, volunteerEntity);
                _repository.Volunteer.UpdateVolunteer(volunteerEntity);
                _repository.Save();
                var updatedVolunteer= _mapper.Map<VolunteerDTO>(volunteerEntity);
                updatedVolunteer.RoleName = volunteerEntity.Role.ToString();
                updatedVolunteer.CategoryName = volunteerEntity.Category.ToString();

                //VOLUNTEER AVAILABILITY
                var volunteerAvailabilityEntity = _repository.VolunteerAvailability.GetAvailabilityByVolunteerId(updatedPersonVolunteer.VolunteerAvailability.IDVolunteer);
                if (volunteerAvailabilityEntity is null)
                {
                    _logger.LogError($"VolunteerAvailability with IDVolunteer: {updatedPersonVolunteer.VolunteerAvailability.IDVolunteer}, hasn't been found in db.");
                    return NotFound();
                }

                _mapper.Map(updatedPersonVolunteer.VolunteerAvailability, volunteerAvailabilityEntity);
                _repository.VolunteerAvailability.UpdateVolunteerAvailability(volunteerAvailabilityEntity);
                _repository.Save();
                var updatedVolunteerAvailability = _mapper.Map<VolunteerAvailabilityDTO>(volunteerAvailabilityEntity);

                //EMERGENCY CONTACT
                var emergencyContactEntity = _repository.EmergencyContact.GetEmergencyContactByVolunteerId(updatedPersonVolunteer.EmergencyContact.IDVolunteer);
                if (emergencyContactEntity is null)
                {
                    _logger.LogError($"EmergencyContact with idVolunteer: {updatedPersonVolunteer.EmergencyContact.IDVolunteer}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(updatedPersonVolunteer.EmergencyContact, emergencyContactEntity);
                _repository.EmergencyContact.UpdateEmergencyContact(emergencyContactEntity);
                _repository.Save();
                var updatedEmergencyContact = _mapper.Map<EmergencyContactDTO>(emergencyContactEntity);

                var updatedPersonVolunteerResult = new PersonVolunteerDTO
                {
                    Person = updatedPerson,
                    Volunteer = updatedVolunteer,
                    VolunteerAvailability = updatedVolunteerAvailability,
                    EmergencyContact = updatedEmergencyContact
                };

                return Ok(updatedPersonVolunteerResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdatePersonVolunteer action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Deletes a PersonVolunteer.
        /// </summary>
        /// <param name="idPerson"></param>
        [HttpDelete("{idPerson}")]
        [Authorize]
        public IActionResult DeletePersonVolunteer(Guid idPerson)
        {
            try
            {
                var volunteer = _repository.Volunteer.GetVolunteerByPersonId(idPerson);
                if (volunteer is null)
                {
                    _logger.LogError($"Volunteer with idPerson: {idPerson}, hasn't been found in db.");
                    return NotFound();
                }
                //EMERGENCY CONTACT
                var emergencyContact = _repository.EmergencyContact.GetEmergencyContactByVolunteerId(volunteer.Id);
                if (emergencyContact is null)
                {
                    _logger.LogError($"EmergencyContact with idVolunteer: {volunteer.Id}, hasn't been found in db.");
                    return NotFound();
                }
                _repository.EmergencyContact.DeleteEmergencyContact(emergencyContact);
                _repository.Save();
                //VOLUNTEER AVAILABILITY
                var volunteerAvailability = _repository.VolunteerAvailability.GetAvailabilityByVolunteerId(volunteer.Id);
                if (volunteerAvailability is null)
                {
                    _logger.LogError($"VolunteerAvailability with idVolunteer: {volunteer.Id}, hasn't been found in db.");
                    return NotFound();
                }
                _repository.VolunteerAvailability.DeleteVolunteerAvailability(volunteerAvailability);
                _repository.Save();
                //VOLUNTEER
                _repository.Volunteer.DeleteVolunteer(volunteer);
                _repository.Save();
                //PERSON
                var person = _repository.Person.GetPersonById(idPerson);
                if (person is null)
                {
                    _logger.LogError($"Person with Id: {idPerson}, hasn't been found in db.");
                    return NotFound();
                }
                _repository.Person.DeletePerson(person);
                _repository.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdatePersonVolunteer action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
