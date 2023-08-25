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
    [Route("api/events")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repository;
        private IMapper _mapper;
        public EventController(ILoggerManager logger, IRepositoryWrapper repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the list of all Events.
        /// </summary>
        /// <returns>The list of Events.</returns>
        [HttpGet]
        [Authorize]
        public IActionResult Get([FromQuery] EventParameters eventParameters)
        {
            try
            {
                var events = _repository.Event.GetAllEvents(eventParameters);
                _logger.LogInfo($"Returned all events from database.");
                var eventsResult = _mapper.Map<IEnumerable<EventDTO>>(events);

                foreach(var e in events)
                {
                    foreach (var eR in eventsResult)
                    {
                        if(e.Id == eR.Id)
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
                _logger.LogInfo($"Returned all volunteers and people from database.");

                return Ok(eventsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetEvents action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets a Event by Id.
        /// </summary>
        /// <param name="id"></param>  
        [HttpGet("{id}", Name = "EventById")]
        [Authorize]
        public IActionResult GetEventById(Guid id)
        {
            try
            {
                var eventRetrieved = _repository.Event.GetEventById(id);
                if (eventRetrieved is null)
                {
                    _logger.LogError($"Event with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    var eventResult = _mapper.Map<EventDTO>(eventRetrieved);
                    eventResult.StartDateTime = eventRetrieved.Start;
                    eventResult.EndDateTime = eventRetrieved.End;
                    eventResult.EventTypeValue = (int)eventRetrieved.EventType;
                    eventResult.EventTypeName = eventRetrieved.EventType.ToString();

                    var departmentName = _repository.Department.GetDepartmentById(eventRetrieved.IDDepartment).DepartmentName;
                    if (departmentName is not null)
                    {
                        eventResult.DepartmentName = departmentName;
                    }
                    _logger.LogInfo($"Returned Event with id: {id}");

                    return Ok(eventResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetEventById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Creates a Event.
        /// </summary>
        /// <param name="eventToCreate"></param>  
        [Produces("application/json")]
        [HttpPost]
        [Authorize]
        public IActionResult CreateEvent([FromBody] EventForCreationAndUpdateDTO eventToCreate)
        {
            try
            {
                if (eventToCreate is null)
                {
                    _logger.LogError("Event object sent from client is null.");
                    return BadRequest("Event object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid event object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var eventEntity = _mapper.Map<Event>(eventToCreate);
                _repository.Event.CreateEvent(eventEntity);
                _repository.Save();
                return CreatedAtRoute("eventById", new { id = eventEntity.Id }, eventEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateEvent action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Updates a Event.
        /// </summary>
        /// <param name="id"></param> 
        /// <param name="eventToUpdate"></param>  
        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateEvent(Guid id, [FromBody] EventForCreationAndUpdateDTO eventToUpdate)
        {
            try
            {
                if (eventToUpdate is null)
                {
                    _logger.LogError("Event object sent from client is null.");
                    return BadRequest("Event object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid event object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var eventEntity = _repository.Event.GetEventById(id);
                if (eventEntity is null)
                {
                    _logger.LogError($"Event with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(eventToUpdate, eventEntity);
                _repository.Event.UpdateEvent(eventEntity);
                _repository.Save();

                var eventResult = _mapper.Map<EventDTO>(eventEntity);
                return Ok(eventResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateEvent action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Deletes a Event.
        /// </summary>
        /// <param name="id"></param>  
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteEvent(Guid id)
        {
            try
            {
                var eventToDelete = _repository.Event.GetEventById(id);
                if (eventToDelete == null)
                {
                    _logger.LogError($"Event with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _repository.Event.DeleteEvent(eventToDelete);
                _repository.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteEvent action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
