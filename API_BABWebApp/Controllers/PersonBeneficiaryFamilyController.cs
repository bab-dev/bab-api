using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities.DTOs.PersonBeneficiaryFamily;
using Entities.Models;
using Entities.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_BABWebApp.Controllers
{
    [Route("api/person-beneficiaries")]
    [ApiController]
    public class PersonBeneficiaryFamilyController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryWrapper _repository;
        private readonly IMapper _mapper;
        public PersonBeneficiaryFamilyController(ILoggerManager logger, IRepositoryWrapper repository, IMapper mapper)
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
        /// Gets the list of all PersonBeneficiaryFamilies.
        /// </summary>
        [HttpGet]
        [Authorize]
        public IActionResult GetAllPersonBeneficiaries([FromQuery] BeneficiaryFamilyParameters parameters)
        {
            try
            {
                var beneficiaryFamilies = _repository.BeneficiaryFamily.GetAllBeneficiaryFamilies(parameters);
                if (beneficiaryFamilies is null)
                {
                    _logger.LogError($"There is no BeneficiaryFamily found in db.");
                    return NotFound();
                }
                var personBeneficiaryFamiliesResult = new List<PersonBeneficiaryFamilyDTO>();

                foreach (var beneficiary in beneficiaryFamilies)
                {
                    //BENEFICIARY FAMILY
                    var beneficiaryFamilyResult = _mapper.Map<BeneficiaryFamilyDTO>(beneficiary);
                    beneficiaryFamilyResult.HousingTypeName = beneficiary.HousingType.ToString();

                    //PERSON
                    var person = _repository.Person.GetPersonById(beneficiary.IDPerson);
                    if (person is null)
                    {
                        _logger.LogError($"Person with id: {beneficiary.IDPerson}, hasn't been found in db.");
                        return NotFound();
                    }
                    var personResult = _mapper.Map<PersonDTO>(person);
                    personResult.Age = GetAge(person.DateOfBirth);

                    // MEMBERS
                    var familyMembers = _repository.BeneficiaryFamily.GetMembersByIdBeneficiary(beneficiary.Id);
                    if (familyMembers is null)
                    {
                        _logger.LogError($"Members from BeneficiaryFamily with ID: {beneficiary.Id}, hasn't been found in db.");
                        return NotFound();
                    }
                    var membersResult = _mapper.Map<IEnumerable<BeneficiaryFamilyMemberDTO>>(familyMembers);

                    var result = new PersonBeneficiaryFamilyDTO
                    {
                        Person = personResult,
                        BeneficiaryFamily = beneficiaryFamilyResult,
                        Members = membersResult
                    };

                    personBeneficiaryFamiliesResult.Add(result);
                };

                _logger.LogInfo($"Returned all PersonBeneficiaryFamilies");

                return Ok(personBeneficiaryFamiliesResult);
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAllPersonBeneficiaries action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets a PersonBeneficiaryFamily by IDBeneficiary.
        /// </summary>
        /// <param name="idBeneficiary"></param> 
        [HttpGet("{idBeneficiary}", Name = "PersonBeneficiaryFamilyByIdBeneficiary")]
        [Authorize]
        public IActionResult GetPersonBeneficiaryFamilyById(Guid idBeneficiary)
        {
            try
            {
                var beneficiaryFamily = _repository.BeneficiaryFamily.GetBeneficiaryFamilyById(idBeneficiary);
                if (beneficiaryFamily is null)
                {
                    _logger.LogError($"BeneficiaryFamily with id: {idBeneficiary}, hasn't been found in db.");
                    return NotFound();
                }

                var person = _repository.Person.GetPersonById(beneficiaryFamily.IDPerson);
                if (person is null)
                {
                    _logger.LogError($"Person with id: {beneficiaryFamily.IDPerson}, hasn't been found in db.");
                    return NotFound();
                }

                var familyMembers = _repository.BeneficiaryFamily.GetMembersByIdBeneficiary(beneficiaryFamily.Id);
                if (familyMembers is null)
                {
                    _logger.LogError($"Members from BeneficiaryFamily with ID: {beneficiaryFamily.Id}, hasn't been found in db.");
                    return NotFound();
                }

                else
                {
                    _logger.LogInfo($"Returned PersonBeneficiaryFamily with IDBeneficiary: {idBeneficiary}");

                    //PERSON
                    var personResult = _mapper.Map<PersonDTO>(person);
                    personResult.Age = GetAge(person.DateOfBirth);

                    //BENEFICIARY FAMILY
                    var beneficiaryFamilyResult = _mapper.Map<BeneficiaryFamilyDTO>(beneficiaryFamily);
                    beneficiaryFamilyResult.HousingTypeName = beneficiaryFamily.HousingType.ToString();

                    // MEMBERS
                    var membersResult = _mapper.Map<IEnumerable<BeneficiaryFamilyMemberDTO>>(familyMembers);

                    var personBeneficiaryFamilyResult = new PersonBeneficiaryFamilyDTO
                    {
                        Person = personResult,
                        BeneficiaryFamily = beneficiaryFamilyResult,
                        Members = membersResult
                    };

                    return Ok(personBeneficiaryFamilyResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetPersonBeneficiaryFamilyByIdPerson action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        private bool PersonAlreadyExists(PersonForCreationAndUpdateDTO personDTO)
        {
            var people = _repository.Person.GetAllPeople();
            var personResult = _mapper.Map<Person>(personDTO);
            return people.Any(p => p == personResult);
        }

        /// <summary>
        /// Creates a new PersonBeneficiaryFamily.
        /// </summary>
        /// <param name="newPersonBeneficiaryFamily"></param>  
        [Produces("application/json")]
        [HttpPost]
        [Authorize]
        public IActionResult CreatePersonBeneficiaryFamily([FromBody] PersonBeneficiaryFamilyForCreationDTO newPersonBeneficiaryFamily)
        {
            try
            {
                if (newPersonBeneficiaryFamily is null)
                {
                    _logger.LogError("PersonBeneficiaryFamily object sent from client is null.");
                    return BadRequest("PersonBeneficiaryFamily object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid object sent from client.");
                    return BadRequest("Invalid model object");
                }
                //PERSON
                if (PersonAlreadyExists(newPersonBeneficiaryFamily.Person))
                {
                    _logger.LogError($"Person with same personal data has been already registered in db.");
                    return BadRequest("Person has already been registered");
                }
                var personEntity = _mapper.Map<Person>(newPersonBeneficiaryFamily.Person);
                _repository.Person.CreatePerson(personEntity);
                _repository.Save();
                var createdPerson = _mapper.Map<PersonDTO>(personEntity); //Contains the person Id

                //BENEFICIARY FAMILY
                var beneficiaryFamilyEntity = _mapper.Map<BeneficiaryFamily>(newPersonBeneficiaryFamily.BeneficiaryFamily);
                beneficiaryFamilyEntity.IDPerson = createdPerson.Id; //Setting IDPerson after creating the person object
                _repository.BeneficiaryFamily.CreateBeneficiaryFamily(beneficiaryFamilyEntity);
                _repository.Save();
                var createdBeneficiaryFamily = _repository.BeneficiaryFamily.GetBeneficiaryFamilyByIdPerson(createdPerson.Id); //Contains the BeneficiaryFamily Id

                var mappedBeneficiaryFamily = _mapper.Map<BeneficiaryFamilyDTO>(createdBeneficiaryFamily);

                //MEMBERS
                if (newPersonBeneficiaryFamily.Members?.Any() == true)
                {
                    var membersEntity = _mapper.Map<IEnumerable<BeneficiaryFamilyMember>>(newPersonBeneficiaryFamily.Members);

                    var membersToCreate = membersEntity.Select((element, index) =>
                    {
                        element.IDBeneficiary = mappedBeneficiaryFamily.Id; // Set the ID of each element
                        _repository.BeneficiaryFamilyMember.CreateBeneficiaryFamilyMember(element);
                        _repository.Save();
                        return element;
                    });

                    var mappedMembers = _mapper.Map<IEnumerable<BeneficiaryFamilyMemberDTO>>(membersToCreate);

                    var personBeneficiaryFamilyResult = new PersonBeneficiaryFamilyDTO
                    {
                        Person = createdPerson,
                        BeneficiaryFamily = mappedBeneficiaryFamily,
                        Members = mappedMembers
                    };
                    return CreatedAtRoute("PersonBeneficiaryFamilyByIdBeneficiary", new { idBeneficiary = mappedBeneficiaryFamily.Id }, personBeneficiaryFamilyResult);
                } else
                {
                    var personBeneficiaryFamilyResult = new PersonBeneficiaryFamilyDTO
                    {
                        Person = createdPerson,
                        BeneficiaryFamily = mappedBeneficiaryFamily,
                    };
                    return CreatedAtRoute("PersonBeneficiaryFamilyByIdBeneficiary", new { idBeneficiary = mappedBeneficiaryFamily.Id }, personBeneficiaryFamilyResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreatePersonBeneficiaryFamily action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


        /// <summary>
        /// Updates a PersonBeneficiaryFamily.
        /// </summary>
        /// <param name="idBeneficiary"></param>
        /// <param name="updatedPersonBeneficiaryFamily"></param> 
        [HttpPut("{idBeneficiary}")]
        [Authorize]
        public IActionResult UpdatePersonBeneficiaryFamily(Guid idBeneficiary, [FromBody] PersonBeneficiaryFamilyForUpdateDTO updatedPersonBeneficiaryFamily)
        {
            try
            {
                if (updatedPersonBeneficiaryFamily is null)
                {
                    _logger.LogError("PersonBeneficiaryFamily object sent from client is null.");
                    return BadRequest("PersonBeneficiaryFamily object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid object sent from client.");
                    return BadRequest("Invalid model object");
                }
                

                //BENEFICIARY FAMILY 
                var beneficiaryFamilyEntity = _repository.BeneficiaryFamily.GetBeneficiaryFamilyById(idBeneficiary);
                if (beneficiaryFamilyEntity is null)
                {
                    _logger.LogError($"BeneficiaryFamily with id: {idBeneficiary}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(updatedPersonBeneficiaryFamily.BeneficiaryFamily, beneficiaryFamilyEntity);
                _repository.BeneficiaryFamily.UpdateBeneficiaryFamily(beneficiaryFamilyEntity);
                _repository.Save();
                var updatedBeneficiaryFamily = _mapper.Map<BeneficiaryFamilyDTO>(beneficiaryFamilyEntity);
                
                //PERSON
                var personEntity = _repository.Person.GetPersonById(beneficiaryFamilyEntity.IDPerson);
                if (personEntity is null)
                {
                    _logger.LogError($"Person with id: {beneficiaryFamilyEntity.IDPerson}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(updatedPersonBeneficiaryFamily.Person, personEntity);
                _repository.Person.UpdatePerson(personEntity);
                _repository.Save();
                var updatedPerson = _mapper.Map<PersonDTO>(personEntity);
                updatedPerson.Age = GetAge(personEntity.DateOfBirth);

                //MEMBERS
                var membersEntity = _repository.BeneficiaryFamily.GetMembersByIdBeneficiary(idBeneficiary);
                if (membersEntity is null)
                {
                    _logger.LogError($"Members from BeneficiaryFamily with ID: {idBeneficiary}, hasn't been found in db.");
                    return NotFound();
                }

                if (updatedPersonBeneficiaryFamily.Members.Any())
                {
                    var mappedMembers= _mapper.Map<IEnumerable<BeneficiaryFamilyMember>>(updatedPersonBeneficiaryFamily.Members);

                    var membersToUpdate = mappedMembers.Select(member=>
                    {
                        member.IDBeneficiary = idBeneficiary; // Set the ID of each element

                        if (membersEntity.Any(existingMember => existingMember.Id == member.Id))
                        {
                            _repository.BeneficiaryFamilyMember.UpdateBeneficiaryFamilyMember(member);
                            _repository.Save();
                        } else
                        {
                            _repository.BeneficiaryFamilyMember.CreateBeneficiaryFamilyMember(member);
                            _repository.Save();
                        }
                    
                        return member;
                    }).ToList();

                    // Delete all members that were deleted when the user was updating the object'
                    IEnumerable<BeneficiaryFamilyMember> removedMembers = membersEntity.Where(b => !mappedMembers.Any(a => a.Id == b.Id)).ToList();

                    foreach (var memberToDelete in removedMembers)
                    {
                        _repository.BeneficiaryFamilyMember.DeleteBeneficiaryFamilyMember(memberToDelete);
                        _repository.Save();
                    }

                    var mappedMembersToDisplay = _mapper.Map<IEnumerable<BeneficiaryFamilyMemberDTO>>(membersToUpdate);
                    var updatedPersonBeneficiaryFamilyResult = new PersonBeneficiaryFamilyDTO
                    {
                        Person = updatedPerson,
                        BeneficiaryFamily = updatedBeneficiaryFamily,
                        Members = mappedMembersToDisplay
                    };
                    return Ok(updatedPersonBeneficiaryFamilyResult);

                } else
                {
                    if (membersEntity.Any())
                    {
                        foreach (var member in membersEntity)
                        {
                            _repository.BeneficiaryFamilyMember.DeleteBeneficiaryFamilyMember(member);
                            _repository.Save();
                        }

                    }
                    var updatedPersonBeneficiaryFamilyResult = new PersonBeneficiaryFamilyDTO
                    {
                        Person = updatedPerson,
                        BeneficiaryFamily = updatedBeneficiaryFamily,
                        Members = Array.Empty<BeneficiaryFamilyMemberDTO>()
                    };
                    return Ok(updatedPersonBeneficiaryFamilyResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdatePersonBeneficiaryFamily action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Deletes a PersonBeneficiaryFamily.
        /// </summary>
        /// <param name="idBeneficiary"></param>
        [HttpDelete("{idBeneficiary}")]
        [Authorize]
        public IActionResult DeletePersonBeneficiaryFamily(Guid idBeneficiary)
        {
            try
            { 
                var beneficiaryFamily = _repository.BeneficiaryFamily.GetBeneficiaryFamilyById(idBeneficiary);
                if (beneficiaryFamily is null)
                {
                    _logger.LogError($"BeneficiaryFamily with ID: {idBeneficiary}, hasn't been found in db.");
                    return NotFound();
                }

                // MEMBERS
                var members = _repository.BeneficiaryFamily.GetMembersByIdBeneficiary(beneficiaryFamily.Id);
                if (members is null)
                {
                    _logger.LogError($"Members from BeneficiaryFamily with ID: {beneficiaryFamily.Id}, hasn't been found in db.");
                    return NotFound();
                }
                if (members.Any())
                {
                    foreach (var member in members)
                    {
                        _repository.BeneficiaryFamilyMember.DeleteBeneficiaryFamilyMember(member);
                        _repository.Save();
                    }
                }

                //BENEFICIARY FAMILY
                _repository.BeneficiaryFamily.DeleteBeneficiaryFamily(beneficiaryFamily);
                _repository.Save();

                //PERSON
                var person = _repository.Person.GetPersonById(beneficiaryFamily.IDPerson);
                if (person is null)
                {
                    _logger.LogError($"Person with Id: {beneficiaryFamily.IDPerson}, hasn't been found in db.");
                    return NotFound();
                }
                _repository.Person.DeletePerson(person);
                _repository.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeletePersonBeneficiaryFamily action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
