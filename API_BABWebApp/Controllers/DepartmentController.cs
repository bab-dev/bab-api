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
using System.Threading.Tasks;

namespace API_BABWebApp.Controllers
{
    [Route("api/departments")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repository;
        private IMapper _mapper;
        public DepartmentController(ILoggerManager logger, IRepositoryWrapper repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the list of all Departments.
        /// </summary>
        /// <returns>The list of Departments.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var departments = _repository.Department.GetAllDepartments();
                _logger.LogInfo($"Returned all people from database.");

                var departmentResult = _mapper.Map<IEnumerable<DepartmentDTO>>(departments);
                return Ok(departmentResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetDepartments action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets a Department by Id.
        /// </summary>
        /// <param name="id"></param>  
        [HttpGet("{id}", Name = "DepartmentById")]
        [Authorize]
        public IActionResult GetDepartmentById(Guid id)
        {
            try
            {
                var department = _repository.Department.GetDepartmentById(id);
                if (department is null)
                {
                    _logger.LogError($"Department with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned department with id: {id}");
                    var departmentResult = _mapper.Map<DepartmentDTO>(department);
                    return Ok(departmentResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetDepartmentById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Creates a Department.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/Department
        ///     {        
        ///       "DepartmentName": "Logistica"    
        ///     }
        /// </remarks>
        /// <param name="department"></param>  
        [Produces("application/json")]
        [HttpPost]
        [Authorize]
        public IActionResult CreateDepartment([FromBody] DepartmentForCreationDTO department)
        {
            try
            {
                if (department is null)
                {
                    _logger.LogError("Department object sent from client is null.");
                    return BadRequest("Department object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid department object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var departmentEntity = _mapper.Map<Department>(department);
                _repository.Department.CreateDepartment(departmentEntity);
                _repository.Save();
                var createdDepartment = _mapper.Map<DepartmentDTO>(departmentEntity);
                return CreatedAtRoute("DepartmentById", new { id = createdDepartment.Id }, createdDepartment);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateDepartment action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Updates a Department.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT api/Department/17b7fc87-27da-471c-81a6-31c07bf2c80c
        ///     {        
        ///       "DepartmentName": "Logistica"    
        ///     }
        /// </remarks>
        /// <param name="id"></param> 
        /// <param name="department"></param>  
        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateDepartment(Guid id, [FromBody] DepartmentForUpdateDTO department)
        {
            try
            {
                if (department is null)
                {
                    _logger.LogError("Department object sent from client is null.");
                    return BadRequest("Department object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid department object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var departmentEntity = _repository.Department.GetDepartmentById(id);
                if (departmentEntity is null)
                {
                    _logger.LogError($"Department with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(department, departmentEntity);
                _repository.Department.UpdateDepartment(departmentEntity);
                _repository.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateDepartment action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Deletes a Department.
        /// </summary>
        /// <param name="id"></param>  
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteDepartment(Guid id)
        {
            try
            {
                var department = _repository.Department.GetDepartmentById(id);
                if (department == null)
                {
                    _logger.LogError($"Department with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _repository.Department.DeleteDepartment(department);
                _repository.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteDepartment action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
