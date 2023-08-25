using API_BABWebApp.Extensions;
using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace API_BABWebApp.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IConfiguration _configuration;
        private ILoggerManager _logger;
        private IRepositoryWrapper _repository;
        private IMapper _mapper;
        private SymmetricSecurityKey _key;
        private const string ADMIN_ROLE = "ADMIN";
        private const string USER_ROLE = "USER";

        public UserController(IConfiguration configuration, ILoggerManager logger, IRepositoryWrapper repository, IMapper mapper)
        {
            _configuration = configuration;
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
        }

        /// <summary>
        /// Gets a User by Email.
        /// </summary>
        /// <param name="email"></param> 
        [HttpGet("{email}", Name = "UserByEmail"), Authorize]
        public IActionResult GetUserByEmail(string email)
        {
            try
            {
                var user = _repository.User.GetUserByEmail(email);
                if (user is null)
                {
                    _logger.LogError($"User with email: {email}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned user with id: {email}");
                    var userResult = _mapper.Map<UserDTO>(user);
                    return Ok(userResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetUserByEmail action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        private bool UserAlreadyExists(string email)
        {
            return _repository.User.GetUserByEmail(email) != null;
        }

        private Guid GetPersonID(string email)
        {
            var person = _repository.Person.GetPersonByEmail(email);
            if (person != null) return person.Id;
            return Guid.Empty;
            
        }

        /// <summary>
        /// Creates a User.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/User
        ///     {        
        ///       "email": "juanosorio1@gmail.com",
        ///       "password": "test123"    
        ///     }
        /// </remarks>
        /// <param name="user"></param>  
        [Produces("application/json")]
        [ActionName("CreateUser")]
        [HttpPost]
        public IActionResult CreateUser([FromBody] UserForCreationAndLoginDTO user)
        {
            try
            {
                if (user is null)
                {
                    _logger.LogError("User object sent from client is null.");
                    return BadRequest("User object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid user object sent from client.");
                    return BadRequest("Invalid model object");
                }
                if (UserAlreadyExists(user.Email))
                {
                    _logger.LogError($"Email {user.Email} is already in use");
                    return BadRequest("Email is already in use");
                }
                var personId = GetPersonID(user.Email);
                if (personId == Guid.Empty)
                {
                    _logger.LogError($"Person not found in DB");
                    return BadRequest("Person not found in DB");
                }

                var userEntity = _mapper.Map<User>(user);

                var salt = CryptoUtil.GenerateSalt();
                userEntity.IDPerson = personId;
                userEntity.Salt = salt;
                userEntity.Password = CryptoUtil.HashMultiple(user.Password, salt);

                _repository.User.CreateUser(userEntity);
                _repository.Save();
                var createdUser = _mapper.Map<UserDTO>(userEntity);
                return StatusCode(201, "User created");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateUser action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <param name="user"></param>  
        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] UserForCreationAndLoginDTO user)
        {
            try
            {
                
                var existingUser = _repository.User.GetUserByEmail(user.Email);
                if (existingUser != null)
                {
                    var isPasswordVerified = CryptoUtil.VerifyPassword(user.Password, existingUser.Salt, existingUser.Password);
                    if (isPasswordVerified)
                    {
                        
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, existingUser.Email),
                            new Claim(ClaimTypes.Role, existingUser.Role)
                        };
                        var accesstoken = _repository.JWTManager.GenerateAccessToken(claims, _key);
                        var refreshToken = _repository.JWTManager.GenerateRefreshToken();

                        var expireDate = DateTime.Now.AddMinutes(5);

                        _repository.User.UpdateUserRefreshToken(existingUser, refreshToken, expireDate); //Set RefreshToken in DB

                        var responseLogin = new AuthenticatedResponseDTO()
                        {
                            Success = true,
                            AccessToken = accesstoken,
                            RefreshToken = refreshToken,
                            ExpireDate = expireDate,
                            IDUser = existingUser.Id,
                            IDPerson = existingUser.IDPerson,
                            UserRole = existingUser.Role,
                            Roles = existingUser.Roles,
                            Permissions = existingUser.Permissions
                        };
                    Console.WriteLine(User.Identity.Name);
                        return Ok(responseLogin);
                    }
                    else
                    {
                        _logger.LogInfo("Password is wrong");
                        return BadRequest("Invalid login request");
                    }
                }
                else
                {
                    _logger.LogError("Incorrect email or password!");
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside Login action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Changes the user role.
        /// </summary>
        /// <param name="email"></param> 
        [HttpPut("{email}"), Authorize]
        public IActionResult ChangeUserRole(string email)
        {
            try
            {
                var userEntity = _repository.User.GetUserByEmail(email);
                if (userEntity is null)
                {
                    _logger.LogError($"User with email: {email}, hasn't been found in db.");
                    return NotFound();
                }
                if (userEntity.Role == ADMIN_ROLE)
                {
                    userEntity.Role = USER_ROLE;
                }
                else if (userEntity.Role == USER_ROLE)
                {
                    userEntity.Role = ADMIN_ROLE;
                }

                _repository.User.UpdateUserRole(userEntity);
                _repository.Save();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdatePerson action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
