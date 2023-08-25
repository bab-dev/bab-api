using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities.DTOs.BeneficiaryFamily;
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
    [Route("api/beneficiary-families")]
    [ApiController]
    public class BeneficiaryFamilyController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryWrapper _repository;
        private readonly IMapper _mapper;
        public BeneficiaryFamilyController(ILoggerManager logger, IRepositoryWrapper repository, IMapper mapper)
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
        /// Gets the list of all BeneficiaryFamilies.
        /// </summary>
        /// <returns>The list of BeneficiaryFamilies.</returns>
        [HttpGet]
        [Authorize]
        public IActionResult Get([FromQuery] BeneficiaryFamilyParameters parameters)
        {
            try
            {
                /*var isFirstSurnameDesc = false;

                if (parameters.OrderBy == "FirstSurname desc")
                {
                    isFirstSurnameDesc = true;
                    parameters.OrderBy = null;
                }*/

                var beneficiaryFamilies = _repository.BeneficiaryFamily.GetAllBeneficiaryFamilies(parameters);
                
                var beneficiaryFamiliesResult = _mapper.Map<IEnumerable<BeneficiaryFamilyWithPopulation>>(beneficiaryFamilies);

                foreach (var beneficiary in beneficiaryFamiliesResult)
                {
                    var person = _repository.Person.GetPersonById(beneficiary.IDPerson);
                    if (person is null)
                    {
                        _logger.LogError($"Person with id: {beneficiary.IDPerson}, hasn't been found in db.");
                        return NotFound();
                    }

                    _mapper.Map(person, beneficiary);
                    beneficiary.Age = GetAge(person.DateOfBirth);
                    beneficiary.HousingTypeName = beneficiary.HousingType.ToString();

                    var populationData = _repository.BeneficiaryFamily.GetPopulationByIdBeneficiary(beneficiary.Id);
                    if (populationData is null)
                    {
                        _logger.LogError($"Population data from a Beneficiary with id: {beneficiary.Id}, hasn't been found in db.");
                        return NotFound();
                    }

                    _mapper.Map(populationData, beneficiary);
                }

                var metadata = new
                {
                    beneficiaryFamilies.TotalCount,
                    beneficiaryFamilies.PageSize,
                    beneficiaryFamilies.CurrentPage,
                    beneficiaryFamilies.TotalPages,
                    beneficiaryFamilies.HasNext,
                    beneficiaryFamilies.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo($"Returned all BeneficiaryFamilies from database.");

                if(parameters.OrderBy is null)
                {
                    var sortedData = beneficiaryFamiliesResult.OrderBy(beneficiary => beneficiary.FirstSurname).ToList();
                    return Ok(sortedData);
                }
                return Ok(beneficiaryFamiliesResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetBeneficiaryFamilies action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets a BeneficiaryFamily by Id.
        /// </summary>
        /// <param name="id"></param>  
        [HttpGet("{id}", Name = "BeneficiaryFamilyById")]
        [Authorize]
        public IActionResult GetBeneficiaryFamilyById(Guid id)
        {
            try
            {
                var beneficiary = _repository.BeneficiaryFamily.GetBeneficiaryFamilyById(id);
                if (beneficiary is null)
                {
                    _logger.LogError($"BeneficiaryFamily with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    var beneficiaryFamilyResult = _mapper.Map<BeneficiaryFamilyWithPopulation>(beneficiary);

                    var person = _repository.Person.GetPersonById(beneficiary.IDPerson);
                    if (person is null)
                    {
                        _logger.LogError($"Person with id: {beneficiary.IDPerson}, hasn't been found in db.");
                        return NotFound();
                    }

                    _mapper.Map(person, beneficiaryFamilyResult);

                    var populationData = _repository.BeneficiaryFamily.GetPopulationByIdBeneficiary(id);
                    if (populationData is null)
                    {
                        _logger.LogError($"Population data from a Beneficiary with id: {id}, hasn't been found in db.");
                        return NotFound();
                    }

                    _mapper.Map(populationData, beneficiaryFamilyResult);

                    _logger.LogInfo($"Returned BeneficiaryFamily with id: {id}");
                    return Ok(beneficiaryFamilyResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetBeneficiaryFamilyById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


        /// <summary>
        /// Gets the detailed information of the population that is part of the beneficiary's family by IdBeneficiaryFamily.
        /// </summary>
        /// <param name="id"></param>  
        [HttpGet("{id}/population", Name = "PopulationByIdBeneficiaryFamily")]
        public IActionResult GetPopulationByIdBeneficiaryFamily(Guid id)
        {
            try
            {
                var members = _repository.BeneficiaryFamily.GetPopulationByIdBeneficiary(id);
                if (members is null)
                {
                    _logger.LogError($"BeneficiaryFamilyMember with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned BeneficiaryFamilyMember with id: {id}");
                    return Ok(members);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetBeneficiaryFamilyMemberById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets the list of the members of the beneficiary's family by IdBeneficiaryFamily.
        /// </summary>
        /// <param name="id"></param>  
        [HttpGet("{id}/members", Name = "MembersByIdBeneficiaryFamily")]
        public IActionResult GetMembersByIdBeneficiaryFamily(Guid id)
        {
            try
            {
                var members = _repository.BeneficiaryFamily.GetMembersByIdBeneficiary(id);
                if (members is null)
                {
                    _logger.LogError($"BeneficiaryFamilyMember with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned BeneficiaryFamilyMember with id: {id}");
                    return Ok(members);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetBeneficiaryFamilyMemberById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Creates a BeneficiaryFamily.
        /// </summary>
        /// <param name="beneficiaryFamily"></param>  
        [Produces("application/json")]
        [HttpPost]
        [Authorize]
        public IActionResult CreateBeneficiaryFamily([FromBody] BeneficiaryFamilyForCreationAndUpdateDTO beneficiaryFamily)
        {
            try
            {
                if (beneficiaryFamily is null)
                {
                    _logger.LogError("BeneficiaryFamily object sent from client is null.");
                    return BadRequest("BeneficiaryFamily object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid BeneficiaryFamily object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var beneficiaryFamilyEntity = _mapper.Map<BeneficiaryFamily>(beneficiaryFamily);
                _repository.BeneficiaryFamily.CreateBeneficiaryFamily(beneficiaryFamilyEntity);
                _repository.Save();
                var createdBeneficiaryFamily = _mapper.Map<BeneficiaryFamilyDTO>(beneficiaryFamilyEntity);
                return CreatedAtRoute("BeneficiaryFamilyById", new { id = createdBeneficiaryFamily.Id }, createdBeneficiaryFamily);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateBeneficiaryFamily action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Updates a BeneficiaryFamily.
        /// </summary>
        /// <param name="id"></param> 
        /// <param name="beneficiaryFamily"></param>  
        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateBeneficiaryFamily(Guid id, [FromBody] BeneficiaryFamilyForCreationAndUpdateDTO beneficiaryFamily)
        {
            try
            {
                if (beneficiaryFamily is null)
                {
                    _logger.LogError("BeneficiaryFamily object sent from client is null.");
                    return BadRequest("BeneficiaryFamily object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid BeneficiaryFamily object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var beneficiaryFamilyEntity = _repository.BeneficiaryFamily.GetBeneficiaryFamilyById(id);
                if (beneficiaryFamilyEntity is null)
                {
                    _logger.LogError($"BeneficiaryFamily with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(beneficiaryFamily, beneficiaryFamilyEntity);
                _repository.BeneficiaryFamily.UpdateBeneficiaryFamily(beneficiaryFamilyEntity);
                _repository.Save();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateBeneficiaryFamily action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Deletes a BeneficiaryFamily.
        /// </summary>
        /// <param name="id"></param>  
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteBeneficiaryFamily(Guid id)
        {
            try
            {
                var beneficiaryFamily = _repository.BeneficiaryFamily.GetBeneficiaryFamilyById(id);
                if (beneficiaryFamily == null)
                {
                    _logger.LogError($"BeneficiaryFamily with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _repository.BeneficiaryFamily.DeleteBeneficiaryFamily(beneficiaryFamily);
                _repository.Save();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteBeneficiaryFamily action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
