using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities.Models;
using Entities.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_BABWebApp.Controllers
{
    [Route("api/volunteers")]
    [ApiController]
    public class VolunteerController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repository;
        private IMapper _mapper;
        public VolunteerController(ILoggerManager logger, IRepositoryWrapper repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets all Volunteers.
        /// </summary>
        /// <returns>The list of Volunteers.</returns>
        [HttpGet]
        [Authorize]
        public IActionResult Get([FromQuery] VolunteerParameters volunteerParameters)
        {
            try
            {   var people = _repository.Person.GetAllPeople();
                var volunteers = _repository.Volunteer.GetAllVolunteers(volunteerParameters);
                var volunteerPersonList = new List<VolunteerPersonDTO>();

                foreach (var volunteer in volunteers)
                {
                    var person = _repository.Person.GetPersonById(volunteer.IDPerson);
                    if (person is not null)
                    {
                        var model = _mapper.Map<VolunteerPersonDTO>(person);
                        model.IdVolunteer = volunteer.Id;
                        model.FullName = person.GetFullName();
                        var department = _repository.Department.GetDepartmentById(volunteer.IDDepartment);
                        model.IDDepartment = department.Id;
                        model.RoleName = volunteer.Role.ToString();
                        model.RoleValue = (int)volunteer.Role;
                        model.CategoryValue = (int)volunteer.Category;

                        _mapper.Map(volunteer, model); 
                            
                        volunteerPersonList.Add(model);
                    }
                }

                var metadata = new
                {
                    volunteers.TotalCount,
                    volunteers.PageSize,
                    volunteers.CurrentPage,
                    volunteers.TotalPages,
                    volunteers.HasNext,
                    volunteers.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata)); 
                
                _logger.LogInfo($"Returned all volunteers and people from database.");


                return Ok(volunteerPersonList);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAllVolunteers action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets an Volunteer by Id.
        /// </summary>
        /// <param name="id"></param> 
        [HttpGet("{id}", Name = "VolunteerById")]
        [Authorize]
        public IActionResult GetVolunteerById(int id)
        {
            try
            {
                var volunteer = _repository.Volunteer.GetVolunteerById(id);
                if (volunteer is null)
                {
                    _logger.LogError($"Volunteer with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned volunteer with id: {id}");
                    var volunteerResult = _mapper.Map<VolunteerDTO>(volunteer);
                    return Ok(volunteerResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetVolunteerById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets an Volunteer by Id.
        /// </summary>
        /// <param name="idPerson"></param> 
        [HttpGet("person/{idPerson}", Name = "VolunteerByPersonId")]
        [Authorize]
        public IActionResult GetVolunteerByPersonId(Guid idPerson)
        {
            try
            {
                var volunteer = _repository.Volunteer.GetVolunteerByPersonId(idPerson);
                if (volunteer is null)
                {
                    _logger.LogError($"Volunteer with idPerson: {idPerson}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned volunteer with idPerson: {idPerson}");
                    var volunteerResult = _mapper.Map<VolunteerDTO>(volunteer);
                    return Ok(volunteerResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetVolunteerById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets an Volunteer by Id.
        /// </summary>
        /// <param name="id"></param> 
        [HttpGet("volunteerPerson/{i}", Name = "VolunteerPersonById")]
        [Authorize]
        public IActionResult GetVolunteerPersonById(int id)
        {
            try
            {
                var volunteer = _repository.Volunteer.GetVolunteerById(id);
                if (volunteer is null)
                {
                    _logger.LogError($"VolunteerPerson with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned VolunteerPerson with id: {id}");
                    var volunteerResult = _mapper.Map<VolunteerPersonDTO>(volunteer);

                    var person = _repository.Person.GetPersonById(volunteer.IDPerson);
                    if (person is not null)
                    {
                        volunteerResult.IdVolunteer = volunteer.Id;
                        volunteerResult.FullName = person.GetFullName();
                        var department = _repository.Department.GetDepartmentById(volunteer.IDDepartment);
                        volunteerResult.IDDepartment = department.Id;
                        volunteerResult.RoleName = volunteer.Role.ToString();
                        volunteerResult.RoleValue = (int)volunteer.Role;
                        volunteerResult.CategoryValue = (int)volunteer.Category;
                    }

                    return Ok(volunteerResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetVolunteerById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        private bool PersonExists(Guid idPerson)
        {
            Person personFound = _repository.Person.GetPersonById(idPerson);
            return !(personFound is null);
        }

        private bool VolunteerWithSamePersonIdExists(Guid idPerson)
        {
            var volunteers = _repository.Volunteer.GetAllVolunteers();
            return volunteers.Any(v => v.IDPerson == idPerson);
        }

        private bool DepartmentExists(Guid idDepartment)
        {
            Department departmentFound = _repository.Department.GetDepartmentById(idDepartment);
            return !(departmentFound is null);
        }

        /// <summary>
        /// Creates a Volunteer.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/Volunteer
        ///     {        
        ///       "IDPerson": "8a1919a7-9202-41ae-9eac-978ff992a589",
        ///       "IDDepartment": "17b7fc87-27da-471c-81a6-31c07bf2c80c",
        ///       "Category": 2,
        ///       "Group": "Scouts",
        ///       "IsVaccinated": true
        ///     }
        /// </remarks>
        /// <param name="volunteer"></param> 
        [Produces("application/json")]
        [HttpPost]
        [Authorize]
        public IActionResult CreateVolunteer([FromBody] VolunteerForCreationAndUpdateDTO volunteer)
        {
            try
            {
                if (volunteer is null)
                {
                    _logger.LogError("Volunteer object sent from client is null.");
                    return BadRequest("Volunteer object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid volunteer object sent from client.");
                    return BadRequest("Invalid model object");
                }
                if (!PersonExists(volunteer.IDPerson))
                {
                    _logger.LogError($"Person with id: {volunteer.IDPerson}, hasn't been found in db.");
                    return NotFound("Person not found");
                }
                if (VolunteerWithSamePersonIdExists(volunteer.IDPerson))
                {
                    _logger.LogError("Another volunteer with the same IDPerson has already been registered.");
                    return BadRequest("The IDPerson has already been registered");
                }
                if (!DepartmentExists(volunteer.IDDepartment))
                {
                    _logger.LogError($"Department with id: {volunteer.IDDepartment}, hasn't been found in db.");
                    return NotFound("Department not found");
                }

                var volunteerEntity = _mapper.Map<Volunteer>(volunteer);
                _repository.Volunteer.CreateVolunteer(volunteerEntity);
                _repository.Save();

                var createdVolunteer = _mapper.Map<VolunteerDTO>(volunteerEntity);
                return CreatedAtRoute("VolunteerById", new { id = createdVolunteer.Id }, createdVolunteer);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateVolunteer action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Creates a Volunteer.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT api/Volunteer/439700
        ///     {        
        ///       "IDDepartment": "17b7fc87-27da-471c-81a6-31c07bf2c80c",
        ///       "Role": 2,
        ///       "Category": 1,
        ///       "Group": "Scouts",
        ///       "IsVaccinated": false
        ///     }
        /// </remarks>
        /// <param name="id"></param> 
        /// <param name="volunteer"></param> 
        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateVolunteer(int id, [FromBody] VolunteerForCreationAndUpdateDTO volunteer)
        {
            try
            {
                if (volunteer is null)
                {
                    _logger.LogError("Volunteer object sent from client is null.");
                    return BadRequest("Volunteer object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid volunteer object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var volunteerEntity = _repository.Volunteer.GetVolunteerById(id);
                if (volunteerEntity is null)
                {
                    _logger.LogError($"Volunteer with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                if (!DepartmentExists(volunteer.IDDepartment))
                {
                    _logger.LogError($"Department with id: {volunteer.IDDepartment}, hasn't been found in db.");
                    return NotFound("Department not found in db");
                }
                _mapper.Map(volunteer, volunteerEntity);
                _repository.Volunteer.UpdateVolunteer(volunteerEntity);
                _repository.Save();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateVolunteer action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Deletes a Volunteer.
        /// </summary>
        /// <param name="id"></param> 
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteVolunteer(int id)
        {
            try
            {
                var volunteer = _repository.Volunteer.GetVolunteerById(id);
                if (volunteer is null)
                {
                    _logger.LogError($"Volunteer with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _repository.Volunteer.DeleteVolunteer(volunteer);
                _repository.Save();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteVolunteer action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
