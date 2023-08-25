using AutoMapper;
using Contracts;
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
    [Route("api/emergency-contacts")]
    [ApiController]
    public class EmergencyContactController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repository;
        private IMapper _mapper;
        public EmergencyContactController(ILoggerManager logger, IRepositoryWrapper repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets all EmergencyContacts.
        /// </summary>
        /// <returns>The list of EmergencyContacts.</returns>
        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            try
            {
                var emergencyContacts = _repository.EmergencyContact.GetAllEmergencyContacts();
                _logger.LogInfo($"Returned all emergency contacts from database.");
                var emergencyContactsResult = _mapper.Map<IEnumerable<EmergencyContactDTO>>(emergencyContacts);
                foreach (var contact in emergencyContactsResult)
                {
                    contact.Relationship = (int) contact.Relationship;
                }
                    return Ok(emergencyContactsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAllEmergencyContacts action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets an EmergencyContact by Id.
        /// </summary>
        /// <param name="idVolunteer"></param> 
        [HttpGet("{idVolunteer}", Name = "EmergencyContactByVolunteerId")]
        [Authorize]
        public IActionResult GetEmergencyContactByVolunteerId(int idVolunteer)
        {
            try
            {
                var emergencyContact = _repository.EmergencyContact.GetEmergencyContactByVolunteerId(idVolunteer);
                if (emergencyContact is null)
                {
                    _logger.LogError($"EmergencyContact with idVolunteer: {idVolunteer}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned emergency contact with idVolunteer: {idVolunteer}");
                    var emergencyContactResult = _mapper.Map<EmergencyContactDTO>(emergencyContact);
                    emergencyContactResult.Relationship = (int)emergencyContact.Relationship;
                    return Ok(emergencyContactResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetEmergencyContactById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        private bool VolunteerExists(int idVolunteer)
        {
            Volunteer volunteerFound = _repository.Volunteer.GetVolunteerById(idVolunteer);
            return !(volunteerFound is null);
        }

        /// <summary>
        /// Creates an EmergencyContact.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/EmergencyContact
        ///     {        
        ///       "IDVolunteer": 439110,
        ///       "Name": "Andres",
        ///       "LastName": "Ferrufino",
        ///       "PhoneNumber": 65574593,
        ///       "Relationship": 3
        ///     }
        /// </remarks>
        /// <param name="emergencyContact"></param> 
        [Produces("application/json")]
        [HttpPost]
        [Authorize]
        public IActionResult CreateEmergencyContact([FromBody] EmergencyContactForCreationDTO emergencyContact)
        {
            try
            {
                if (emergencyContact is null)
                {
                    _logger.LogError("EmergencyContact object sent from client is null.");
                    return BadRequest("EmergencyContact object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid emergencyContact object sent from client.");
                    return BadRequest("Invalid model object");
                }
                if (!VolunteerExists(emergencyContact.IDVolunteer))
                {
                    _logger.LogError($"Volunteer with id: {emergencyContact.IDVolunteer}, hasn't been found in db.");
                    return NotFound("Volunteer not found");
                }

                var emergencyContactEntity = _mapper.Map<EmergencyContact>(emergencyContact);
                _repository.EmergencyContact.CreateEmergencyContact(emergencyContactEntity);
                _repository.Save();

                var createdEmergencyContact = _mapper.Map<EmergencyContactDTO>(emergencyContactEntity);
                return CreatedAtRoute("EmergencyContactByVolunteerId", new { idVolunteer = createdEmergencyContact.IDVolunteer }, createdEmergencyContact);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateEmergencyContact action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Updates an EmergencyContact.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT api/EmergencyContact/8a1919a7-9202-41ae-9eac-978ff992a589
        ///     {        
        ///       "IDVolunteer": 439700,
        ///       "Name": "Andres",
        ///       "LastName": "Ferrufino",
        ///       "PhoneNumber": 67474593,
        ///       "Relationship": 3
        ///     }
        /// </remarks>
        /// <param name="idVolunteer"></param>
        /// <param name="emergencyContact"></param> 
        [HttpPut("{idVolunteer}")]
        [Authorize]
        public IActionResult UpdateEmergencyContact(int idVolunteer, [FromBody] EmergencyContactForUpdateDTO emergencyContact)
        {
            try
            {
                if (emergencyContact is null)
                {
                    _logger.LogError("EmergencyContact object sent from client is null.");
                    return BadRequest("EmergencyContact object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid EmergencyContact object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var emergencyContactEntity = _repository.EmergencyContact.GetEmergencyContactByVolunteerId(idVolunteer);
                if (emergencyContactEntity is null)
                {
                    _logger.LogError($"EmergencyContact with idVolunteer: {idVolunteer}, hasn't been found in db.");
                    return NotFound();
                }
                if (!VolunteerExists(emergencyContact.IDVolunteer))
                {
                    _logger.LogError($"Volunteer with id: {emergencyContact.IDVolunteer}, hasn't been found in db.");
                    return NotFound("Volunteer not found in db");
                }
                _mapper.Map(emergencyContact, emergencyContactEntity);
                _repository.EmergencyContact.UpdateEmergencyContact(emergencyContactEntity);
                _repository.Save();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateEmergencyContact action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Deletes an EmergencyContact.
        /// </summary>
        /// <param name="idVolunteer"></param>  
        [HttpDelete("{idVolunteer}")]
        [Authorize]
        public IActionResult DeleteEmergencyContact(int idVolunteer)
        {
            try
            {
                var emergencyContact = _repository.EmergencyContact.GetEmergencyContactByVolunteerId(idVolunteer);
                if (emergencyContact is null)
                {
                    _logger.LogError($"EmergencyContact with idVolunteer: {idVolunteer}, hasn't been found in db.");
                    return NotFound();
                }
                _repository.EmergencyContact.DeleteEmergencyContact(emergencyContact);
                _repository.Save();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteEmergencyContact action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
