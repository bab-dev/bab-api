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
    [Route("api/beneficiary-family-members")]
    [ApiController]
    public class BeneficiaryFamilyMemberController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryWrapper _repository;
        private readonly IMapper _mapper;
        public BeneficiaryFamilyMemberController(ILoggerManager logger, IRepositoryWrapper repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the list of all BeneficiaryFamilyMembers.
        /// </summary>
        /// <returns>The list of BeneficiaryFamilyMembers.</returns>
        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            try
            {
                var members = _repository.BeneficiaryFamilyMember.GetAllBeneficiaryFamilyMembers();
                _logger.LogInfo($"Returned all beneficiaryFamilyMembers from database.");
                var peopleResult = _mapper.Map<IEnumerable<BeneficiaryFamilyMemberDTO>>(members);
                return Ok(peopleResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetBeneficiaryFamilyMembers action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets a BeneficiaryFamilyMember by Id.
        /// </summary>
        /// <param name="id"></param>  
        [HttpGet("{id}", Name = "BeneficiaryFamilyMemberById")]
        [Authorize]
        public IActionResult GetBeneficiaryFamilyMemberById(Guid id)
        {
            try
            {
                var member = _repository.BeneficiaryFamilyMember.GetBeneficiaryFamilyMemberById(id);
                if (member is null)
                {
                    _logger.LogError($"BeneficiaryFamilyMember with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned BeneficiaryFamilyMember with id: {id}");
                    var memberResult = _mapper.Map<BeneficiaryFamilyMemberDTO>(member);
                    return Ok(memberResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetBeneficiaryFamilyMemberById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Creates a BeneficiaryFamilyMember.
        /// </summary>
        /// <param name="member"></param>  
        [Produces("application/json")]
        [HttpPost]
        [Authorize]
        public IActionResult CreateBeneficiaryFamilyMember([FromBody] BeneficiaryFamilyMemberForCreationDTO member)
        {
            try
            {
                if (member is null)
                {
                    _logger.LogError("BeneficiaryFamilyMember object sent from client is null.");
                    return BadRequest("BeneficiaryFamilyMember object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid BeneficiaryFamilyMember object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var memberEntity = _mapper.Map<BeneficiaryFamilyMember>(member);
                _repository.BeneficiaryFamilyMember.CreateBeneficiaryFamilyMember(memberEntity);
                _repository.Save();
                var createdBeneficiaryFamilyMember = _mapper.Map<BeneficiaryFamilyMemberDTO>(memberEntity);
                return CreatedAtRoute("BeneficiaryFamilyMemberById", new { id = createdBeneficiaryFamilyMember.Id }, createdBeneficiaryFamilyMember);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateBeneficiaryFamilyMember action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Updates a BeneficiaryFamilyMember.
        /// </summary>
        /// <param name="id"></param> 
        /// <param name="member"></param>  
        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateBeneficiaryFamilyMember(Guid id, [FromBody] BeneficiaryFamilyMemberForCreationDTO member)
        {
            try
            {
                if (member is null)
                {
                    _logger.LogError("BeneficiaryFamilyMember object sent from client is null.");
                    return BadRequest("BeneficiaryFamilyMember object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid BeneficiaryFamilyMember object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var memberEntity = _repository.BeneficiaryFamilyMember.GetBeneficiaryFamilyMemberById(id);
                if (memberEntity is null)
                {
                    _logger.LogError($"BeneficiaryFamilyMember with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(member, memberEntity);
                _repository.BeneficiaryFamilyMember.UpdateBeneficiaryFamilyMember(memberEntity);
                _repository.Save();
                var updatedBeneficiaryFamilyMember = _mapper.Map<BeneficiaryFamilyMemberDTO>(memberEntity);
                return Ok(updatedBeneficiaryFamilyMember);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateBeneficiaryFamilyMember action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Deletes a BeneficiaryFamilyMember.
        /// </summary>
        /// <param name="id"></param>  
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteBeneficiaryFamilyMember(Guid id)
        {
            try
            {
                var member = _repository.BeneficiaryFamilyMember.GetBeneficiaryFamilyMemberById(id);
                if (member == null)
                {
                    _logger.LogError($"BeneficiaryFamilyMember with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _repository.BeneficiaryFamilyMember.DeleteBeneficiaryFamilyMember(member);
                _repository.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteBeneficiaryFamilyMember action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
