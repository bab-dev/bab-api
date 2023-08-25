using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities.Models;
using Entities.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_BABWebApp.Controllers
{
    [Route("api/event-volunteers")]
    [ApiController]
    public class EventVolunteerController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repository;
        private IMapper _mapper;
        public EventVolunteerController(ILoggerManager logger, IRepositoryWrapper repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the list of all EventVolunteers.
        /// </summary>
        /// <returns>The list of EventVolunteers.</returns>
        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            try
            {
                var eventVolunteers = _repository.EventVolunteer.GetAllEventVolunteers();
                _logger.LogInfo($"Returned all eventVolunteers from database.");

                if (eventVolunteers is null)
                {
                    _logger.LogError($"There are no EventVolunteers in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned EventVolunteers list");

                    return Ok(eventVolunteers);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetEventVolunteers action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets an EventVolunteer by Id.
        /// </summary>
        /// <param name="id"></param>  
        [HttpGet("{id}", Name = "EventVolunteerById")]
        [Authorize]
        public IActionResult GetEventVolunteerById(Guid id)
        {
            try
            {
                var eventVolunteerRetrieved = _repository.EventVolunteer.GetEventVolunteerById(id);
                if (eventVolunteerRetrieved is null)
                {
                    _logger.LogError($"EventVolunteer with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned EventVolunteer with id: {id}");

                    return Ok(eventVolunteerRetrieved);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetEventVolunteerById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets an EventVolunteer by IdEvent and IdVolunteer.
        /// </summary>
        /// <param name="idVolunteer"></param>  
        /// <param name="idEvent"></param>   
        [HttpGet("{idVolunteer}/events/{idEvent}", Name = "EventVolunteerByIDEventAndIDVolunteer")]
        [Authorize]
        public IActionResult GetEventVolunteerByIDEventAndIDVolunteer(Guid idEvent, int idVolunteer)
        {
            try
            {
                var eventVolunteerRetrieved = _repository.EventVolunteer.GetEventVolunteerByIDEventAndIDVolunteer(idEvent, idVolunteer);
                if (eventVolunteerRetrieved is null)
                {
                    _logger.LogError($"EventVolunteer with IDEvent: {idEvent} and IDVolunteer: {idVolunteer}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned EventVolunteerwith IDEvent: {idEvent} and IDVolunteer: {idVolunteer}");

                    return Ok(eventVolunteerRetrieved);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetEventVolunteerById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets the list of all Events that a Volunteer is assigned to.
        /// </summary>
        /// <param name="idVolunteer"></param>  
        /// <param name="eventParameters"></param>
        [HttpGet("volunteers/{idVolunteer}/events", Name = "EventsByIdVolunteer")]
        [Authorize]
        public IActionResult GetEventsByIdVolunteer(int idVolunteer, [FromQuery] EventParameters eventParameters)
        {
            try
            {
                var events = _repository.EventVolunteer.GetEventsByIdVolunteer(idVolunteer, eventParameters);
                if (events is null)
                {
                    _logger.LogError($"There are no events assigned to volunteer with id: {idVolunteer}.");
                    return NotFound();
                }
                else
                {
                    var eventsResult = _mapper.Map<IEnumerable<EventDTO>>(events);

                    foreach (var e in events)
                    {
                        foreach (var eR in eventsResult)
                        {
                            if (e.Id == eR.Id)
                            {
                                eR.StartDateTime = e.Start;
                                eR.EndDateTime = e.End;
                                eR.EventTypeValue = (int)e.EventType;
                                eR.EventTypeName = e.EventType.ToString();
                                var departmentName = _repository.Department.GetDepartmentById(e.IDDepartment).DepartmentName;
                                if (departmentName is not null)
                                {
                                    eR.DepartmentName = departmentName;
                                }
                                break;
                            }

                        }
                    }

                    var metadata = new
                    {
                        events.TotalCount,
                        events.PageSize,
                        events.CurrentPage,
                        events.TotalPages,
                        events.HasNext,
                        events.HasPrevious
                    };

                    Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                    _logger.LogInfo($"Returned Events assigned to a Volunteer with id: {idVolunteer}");
                    return Ok(eventsResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetEventsByIdVolunteer action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets the list of all Events that a Volunteer is assigned to.
        /// </summary>
        /// <param name="idVolunteer"></param>  
        /// <param name="date"></param>
        [HttpGet("volunteers/{idVolunteer}/events/date/{date}", Name = "EventsByIdVolunteerAndDate")]
        [Authorize]
        public IActionResult GetEventsByIdVolunteer(int idVolunteer, DateTime date)
        {
            try
            {
                var events = _repository.EventVolunteer.GetEventsByIdVolunteerAndDate(idVolunteer, date);
                if (events is null)
                {
                    _logger.LogError($"There are no events assigned to volunteer with id: {idVolunteer}.");
                    return NotFound();
                }
                else
                {
                    var eventsResult = _mapper.Map<IEnumerable<EventDTO>>(events);

                    foreach (var e in events)
                    {
                        foreach (var eR in eventsResult)
                        {
                            if (e.Id == eR.Id)
                            {
                                eR.StartDateTime = e.Start;
                                eR.EndDateTime = e.End;
                                eR.EventTypeValue = (int)e.EventType;
                                eR.EventTypeName = e.EventType.ToString();
                                var departmentName = _repository.Department.GetDepartmentById(e.IDDepartment).DepartmentName;
                                if (departmentName is not null)
                                {
                                    eR.DepartmentName = departmentName;
                                }
                                break;
                            }

                        }
                    }

                    _logger.LogInfo($"Returned Events assigned to a Volunteer with id: {idVolunteer} and date: {date}");
                    return Ok(eventsResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside EventsByIdVolunteerAndDate action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


        /// <summary>
        /// Gets the list of all Volunteers assigned to an Event.
        /// </summary>
        /// <param name="idEvent"></param>  
        /// <param name="volunteerParameters"></param>  
        [HttpGet("events/{idEvent}/volunteers", Name = "VolunteersByIdEvent")]
        [Authorize]
        public IActionResult GetVolunteersByIdEvent(Guid idEvent, [FromQuery] VolunteerParameters volunteerParameters)
        {
            try
            {
                var volunteers = _repository.EventVolunteer.GetVolunteersByIdEvent(idEvent, volunteerParameters);
                if (volunteers is null)
                {
                    _logger.LogError($"There are no volunteers assigned to an event with id: {idEvent}.");
                    return NotFound();
                }
                else
                {
                    var volunteersList = new List<VolunteerFromEventDTO>();

                    foreach (var volunteer in volunteers)
                    {
                        var person = _repository.Person.GetPersonById(volunteer.IDPerson);
                        if (person is not null)
                        {
                            var model = _mapper.Map<VolunteerFromEventDTO>(volunteer);
                            model.IDVolunteer = volunteer.Id;
                            model.IDEvent = idEvent;
                            model.FullName = $"{person.FirstName} {person.FirstSurname}";

                            volunteersList.Add(model);
                        }
                    }

                    _logger.LogInfo($"Returned Volunteers assigned to an Event with id: {idEvent}");
                    return Ok(volunteersList);
                }
            }

            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetVolunteersByIdEvent action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


        /// <summary>
        /// Assign a Volunteer to an Event.
        /// </summary>
        /// <param name="eventVolunteerToCreate"></param>  
        [Produces("application/json")]
        [HttpPost]
        [Authorize]
        public IActionResult CreateEventVolunteer([FromBody] EventVolunteerForCreationAndUpdateDTO eventVolunteerToCreate)
        {
            try
            {
                if (eventVolunteerToCreate is null)
                {
                    _logger.LogError("EventVolunteer object sent from client is null.");
                    return BadRequest("EventVolunteer object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid eventVolunteer object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var eventVolunteerEntity = _mapper.Map<EventVolunteer>(eventVolunteerToCreate);
                _repository.EventVolunteer.CreateEventVolunteer(eventVolunteerEntity);
                _repository.Save();
                return CreatedAtRoute("eventVolunteerById", new { id = eventVolunteerEntity.Id }, eventVolunteerEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateEventVolunteer action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Unassign a Volunteer from an Event by Id.
        /// </summary>
        /// <param name="id"></param>  
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteEventVolunteer(Guid id)
        {
            try
            {
                var eventVolunteerToDelete = _repository.EventVolunteer.GetEventVolunteerById(id);
                if (eventVolunteerToDelete == null)
                {
                    _logger.LogError($"EventVolunteer with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                _repository.EventVolunteer.DeleteEventVolunteer(eventVolunteerToDelete);
                _repository.Save();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteEventVolunteer action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Unassign a Volunteer from an Event by IDEvent and IDVolunteer.
        /// </summary>
        /// <param name="idVolunteer"></param>  
        /// <param name="idEvent"></param>  
        [HttpDelete("{idVolunteer}/events/{idEvent}")]
        [Authorize]
        public IActionResult DeleteEventVolunteerByIdEventAndIdVoolunteer(Guid idEvent, int idVolunteer)
        {
            try
            {
                var eventVolunteerToDelete = _repository.EventVolunteer.GetEventVolunteerByIDEventAndIDVolunteer(idEvent, idVolunteer);
                if (eventVolunteerToDelete == null)
                {
                    _logger.LogError($"EventVolunteer with IDEvent: {idEvent} and IDVolunteer: {idVolunteer}, hasn't been found in db.");
                    return NotFound();
                }

                _repository.EventVolunteer.DeleteEventVolunteer(eventVolunteerToDelete);
                _repository.Save();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteEventVolunteer action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
