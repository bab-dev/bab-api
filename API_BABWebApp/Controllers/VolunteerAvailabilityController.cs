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
    [Route("api/volunteers-availability")]
    [ApiController]
    public class VolunteerAvailabilityController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repository;
        private IMapper _mapper;
        public VolunteerAvailabilityController(ILoggerManager logger, IRepositoryWrapper repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets all VolunteerAvailabilities.
        /// </summary>
        /// <returns>The list of VolunteerAvailabilities.</returns>
        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            try
            {
                var volunteerAvailabilities = _repository.VolunteerAvailability.GetAllVolunteerAvailabilities();
                _logger.LogInfo($"Returned all volunteer availability entries from database.");
                var volunteerAvailabilityResult = _mapper.Map<IEnumerable<VolunteerAvailabilityDTO>>(volunteerAvailabilities);
                return Ok(volunteerAvailabilityResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAllVolunteerAvailabilities action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets a VolunteerAvailability by Id.
        /// </summary>
        /// <param name="idVolunteer"></param> 
        [HttpGet("{idVolunteer}", Name = "AvailabilityByVolunteerId")]
        [Authorize]
        public IActionResult GetAvailabilityByVolunteerId(int idVolunteer)
        {
            try
            {
                var volunteerAvailability = _repository.VolunteerAvailability.GetAvailabilityByVolunteerId(idVolunteer);
                if (volunteerAvailability is null)
                {
                    _logger.LogError($"VolunteerAvailability with IDVolunteer: {idVolunteer}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    
                    var volunteerAvailabilityResult = _mapper.Map<VolunteerAvailabilityDTO>(volunteerAvailability);
                    _logger.LogInfo($"Returned volunteer availability with id: {volunteerAvailabilityResult.Id}");
                    return Ok(volunteerAvailabilityResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetVolunteerAvailabilityById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
        private bool VolunteerExists(int idVolunteer)
        {
            Volunteer volunteerFound = _repository.Volunteer.GetVolunteerById(idVolunteer);
            return !(volunteerFound is null);
        }

        /// <summary>
        /// Creates a VolunteerAvailability.
        /// </summary>
        /// <param name="volunteerAvailability"></param> 
        [Produces("application/json")]
        [HttpPost]
        [Authorize]
        public IActionResult CreateVolunteerAvailability([FromBody] VolunteerAvailabilityForCreationDTO volunteerAvailability)
        {
            try
            {
                if (volunteerAvailability is null)
                {
                    _logger.LogError("VolunteerAvailability object sent from client is null.");
                    return BadRequest("VolunteerAvailability object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid volunteerAvailability object sent from client.");
                    return BadRequest("Invalid model object");
                }
                if (!VolunteerExists(volunteerAvailability.IDVolunteer))
                {
                    _logger.LogError($"Volunteer with id: {volunteerAvailability.IDVolunteer}, hasn't been found in db.");
                    return NotFound("Volunteer not found");
                }

                var volunteerAvailabilityEntity = _mapper.Map<VolunteerAvailability>(volunteerAvailability);
                _repository.VolunteerAvailability.CreateVolunteerAvailability(volunteerAvailabilityEntity);
                _repository.Save();

                var createdVolunteerAvailability = _mapper.Map<VolunteerAvailabilityDTO>(volunteerAvailabilityEntity);
                return CreatedAtRoute("AvailabilityByVolunteerId", new { id = createdVolunteerAvailability.Id }, createdVolunteerAvailability);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateVolunteerAvailability action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Updates a VolunteerAvailability.
        /// </summary>
        /// <param name="idVolunteer"></param> 
        /// <param name="volunteerAvailability"></param> 
        [HttpPut("{idVolunteer}")]
        [Authorize]
        public IActionResult UpdateVolunteerAvailability(int idVolunteer, [FromBody] VolunteerAvailabilityForUpdateDTO volunteerAvailability)
        {
            try
            {
                if (volunteerAvailability is null)
                {
                    _logger.LogError("VolunteerAvailability object sent from client is null.");
                    return BadRequest("VolunteerAvailability object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid volunteerAvailability object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var volunteerAvailabilityEntity = _repository.VolunteerAvailability.GetAvailabilityByVolunteerId(idVolunteer);
                if (volunteerAvailabilityEntity is null)
                {
                    _logger.LogError($"VolunteerAvailability with IDVolunteer: {idVolunteer}, hasn't been found in db.");
                    return NotFound();
                }
                if (!VolunteerExists(volunteerAvailability.IDVolunteer))
                {
                    _logger.LogError($"Volunteer with id: {volunteerAvailability.IDVolunteer}, hasn't been found in db.");
                    return NotFound("Volunteer not found in db");
                }

                _mapper.Map(volunteerAvailability, volunteerAvailabilityEntity);
                _repository.VolunteerAvailability.UpdateVolunteerAvailability(volunteerAvailabilityEntity);
                _repository.Save();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateVolunteerAvailability action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Deletes a VolunteerAvailability.
        /// </summary>
        /// <param name="idVolunteer"></param> 
        [HttpDelete("{idVolunteer}")]
        [Authorize]
        public IActionResult DeleteVolunteerAvailability(int idVolunteer)
        {
            try
            {
                var volunteerAvailability = _repository.VolunteerAvailability.GetAvailabilityByVolunteerId(idVolunteer);
                if (volunteerAvailability is null)
                {
                    _logger.LogError($"VolunteerAvailability with IDVolunteer: {idVolunteer}, hasn't been found in db.");
                    return NotFound();
                }
                _repository.VolunteerAvailability.DeleteVolunteerAvailability(volunteerAvailability);
                _repository.Save();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteVolunteerAvailability action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
