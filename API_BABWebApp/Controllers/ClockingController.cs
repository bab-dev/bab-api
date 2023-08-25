using AutoMapper;
using Contracts;
using Entities.DTOs.Clocking;
using Entities.Models;
using Entities.Models.Enums;
using Entities.Parameters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_BABWebApp.Controllers
{
    [Route("api/clocking")]
    [ApiController]
    public class ClockingController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryWrapper _repository;
        private readonly IMapper _mapper;
        public ClockingController(ILoggerManager logger, IRepositoryWrapper repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the list of all Clockings.
        /// </summary>
        /// <returns>The list of Clockings.</returns>
        [HttpGet]
        public IActionResult Get([FromQuery] ClockingParameters parameters)
        {
            try
            {
                var clockings = _repository.Clocking.GetAllClockings(parameters);

                var clockingsResult = new List<ClockingDTO>();

                foreach (var clocking in clockings)
                {
                    var model = _mapper.Map<ClockingDTO>(clocking);

                    model.ClockInTime = clocking.ClockInTime.ToString(@"hh\:mm");
                    model.ClockOutTime = clocking.ClockOutTime.ToString(@"hh\:mm");

                    clockingsResult.Add(model);
                }
                var metadata = new
                {
                    clockings.TotalCount,
                    clockings.PageSize,
                    clockings.CurrentPage,
                    clockings.TotalPages,
                    clockings.HasNext,
                    clockings.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo($"Returned all Clockings from database.");
                return Ok(clockingsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetClockings action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets a Clocking info by Id.
        /// </summary>
        /// <param name="id"></param>  
        [HttpGet("{id}", Name = "ClockingById")]
        public IActionResult GetClockingById(Guid id)
        {
            try
            {
                var clocking = _repository.Clocking.GetClockingById(id);
                if (clocking is null)
                {
                    _logger.LogError($"Clocking with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    var clockingResult = _mapper.Map<ClockingDTO>(clocking);
                    clockingResult.ClockInTime = clocking.ClockInTime.ToString(@"hh\:mm");
                    clockingResult.ClockOutTime = clocking.ClockOutTime.ToString(@"hh\:mm");

                    _logger.LogInfo($"Returned Clocking with id: {id}");
                    return Ok(clockingResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetClockingById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        private bool VolunteerExists(int idVolunteer)
        {
            Volunteer volunteerFound = _repository.Volunteer.GetVolunteerById(idVolunteer);
            return !(volunteerFound is null);
        }

        /// <summary>
        /// Creates a Clocking.
        /// </summary>
        /// <param name="clocking"></param>  
        [Produces("application/json")]
        [HttpPost]
        public IActionResult CreateClocking([FromBody] ClockingForCreationDTO clocking)
        {
            try
            {
                if (clocking is null)
                {
                    _logger.LogError("Clocking object sent from client is null.");
                    return BadRequest("Clocking object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Clocking object sent from client.");
                    return BadRequest("Invalid model object");
                }
                if (!VolunteerExists(clocking.IDVolunteer))
                {
                    _logger.LogError($"Volunteer with id: {clocking.IDVolunteer}, hasn't been found in db.");
                    return NotFound("Volunteer not found");
                }

                var clockingEntity = _mapper.Map<Clocking>(clocking);
                clockingEntity.ClockInTime = TimeSpan.Parse(clocking.ClockInTime);
                clockingEntity.ClockOutTime = TimeSpan.Parse(clocking.ClockInTime);

                _repository.Clocking.CreateClocking(clockingEntity);
                _repository.Save();

                var createdClocking = _mapper.Map<ClockingDTO>(clockingEntity);
                return CreatedAtRoute("ClockingById", new { id = createdClocking.Id }, createdClocking);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateClocking action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        private void SaveEventTypesWorkedOn(Clocking clocking, IEnumerable<int> eventTypesWorkedOn)
        {
            if (eventTypesWorkedOn.Any())
            {
                foreach(var evenType in eventTypesWorkedOn)
                {
                    var eventWorkedOnEntity = new VolunteerEventWorkedOn() {
                        IDVolunteerClocking = clocking.Id,
                        IDVolunteer = clocking.IDVolunteer,
                        Date = clocking.Date,
                        EventType = evenType
                    };
                   
                    _repository.VolunteerEventWorkedOn.CreateVolunteerEventWorkedOn(eventWorkedOnEntity);
                    _repository.Save();
                }
            }
        }

        /// <summary>
        /// Clocking out.
        /// </summary>
        /// <param name="idVolunteer"></param> 
        /// <param name="clockout"></param>  
        [HttpPut("{idVolunteer}/clockout")]
        public IActionResult Clockout(int idVolunteer, [FromBody] ClockoutDTO clockout)
        {
            try
            {
                if (clockout is null)
                {
                    _logger.LogError("Clockout object sent from client is null.");
                    return BadRequest("Clockout object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Clocking object sent from client.");
                    return BadRequest("Invalid model object");
                }
                
                var clockingEntity = _repository.Clocking.GetClockingByIdVolunteerAndDate(idVolunteer, clockout.Date);
                if (clockingEntity is null)
                {
                    _logger.LogError($"Clocking with IDVolunteer: {idVolunteer}, hasn't been found in db.");
                    return NotFound("Clocking registry with that IDVolunteer not found");
                }

                clockingEntity.ClockOutTime = TimeSpan.Parse(clockout.ClockOutTime);

                _repository.Clocking.UpdateClocking(clockingEntity);
                _repository.Save();

                //Store event types that the volunteer worked on
                SaveEventTypesWorkedOn(clockingEntity, clockout.EventTypesWorkedOn);

                return Ok(clockingEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside Clockout action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Updates a Clocking registry.
        /// </summary>
        /// <param name="id"></param> 
        /// <param name="clocking"></param>  
        [HttpPut("{id}")]
        public IActionResult UpdateClocking(Guid id, [FromBody] ClockingForUpdateDTO clocking)
        {
            try
            {
                if (clocking is null)
                {
                    _logger.LogError("Clocking object sent from client is null.");
                    return BadRequest("Clocking object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Clocking object sent from client.");
                    return BadRequest("Invalid model object");
                }

                var clockingEntity = _repository.Clocking.GetClockingById(id);
                if (clockingEntity is null)
                {
                    _logger.LogError($"Clocking with id: {id}, hasn't been found in db.");
                    return NotFound("Clocking registry not found");
                }

                clockingEntity.ClockInTime = TimeSpan.Parse(clocking.ClockInTime);
                clockingEntity.ClockOutTime = TimeSpan.Parse(clocking.ClockOutTime);

                _repository.Clocking.UpdateClocking(clockingEntity);
                _repository.Save();

                return Ok(clockingEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateClocking action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Deletes a Clocking.
        /// </summary>
        /// <param name="id"></param>  
        [HttpDelete("{id}")]
        public IActionResult DeleteClocking(Guid id)
        {
            try
            {
                var clocking = _repository.Clocking.GetClockingById(id);
                if (clocking == null)
                {
                    _logger.LogError($"Clocking with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _repository.Clocking.DeleteClocking(clocking);
                _repository.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteClocking action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
