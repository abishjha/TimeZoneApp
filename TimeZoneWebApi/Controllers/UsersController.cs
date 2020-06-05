using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using TimeZoneWebApi.Helpers;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TimeZoneWebApi.Services;
using TimeZoneWebApi.Entities;
using TimeZoneWebApi.Models.Users;
using TimeZoneWebApi.Models;

namespace TimeZoneWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UsersController(
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]UserAuthenticateModel model)
        {
            var user = _userService.Authenticate(model.Username, model.Password);

            if (user == null)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // return basic user info and authentication token
            return Ok(new
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                Token = tokenString
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]UserRegisterModel model)
        {
            // map model to entity
            var user = _mapper.Map<User>(model);

            if (User != null && User.Identity != null && User.Identity.Name != null)
            {
                var currUser = _userService.GetById(Int32.Parse(User.Identity.Name));
                if (currUser == null || _userService.CheckIf(currUser, new string[] { Roles.ROLE_MANAGER, Roles.ROLE_USER }))
                {
                    user.Role = Roles.ROLE_USER;
                }
            }
            else
            {
                user.Role = Roles.ROLE_USER;
            }

            try
            {
                // create user
                _userService.Create(user, model.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var currUser = _userService.GetById(Int32.Parse(User.Identity.Name));
            if (_userService.CheckIf(currUser, new string[] { Roles.ROLE_MANAGER, Roles.ROLE_ADMIN }))
            {
                var users = _userService.GetAll();
                var model = _mapper.Map<IList<UserModel>>(users);
                return Ok(model);
            }
            else
            {
                return StatusCode(403, "Unauthorized! Only 'Admin' or 'User Manager' can access this resource");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var currUser = _userService.GetById(Int32.Parse(User.Identity.Name));
            if (id == currUser.Id || _userService.CheckIf(currUser, new string[] { Roles.ROLE_MANAGER, Roles.ROLE_ADMIN }))
            {
                var user = _userService.GetById(id);
                var model = _mapper.Map<UserModel>(user);
                return Ok(model);
            }
            else
            {
                return StatusCode(403, "Unauthorized! Only 'Admin' or 'User Manager' or the owner can access this resource");
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]UserUpdateModel model)
        {
            var currUser = _userService.GetById(Int32.Parse(User.Identity.Name));
            if (id == currUser.Id || _userService.CheckIf(currUser, new string[] { Roles.ROLE_MANAGER, Roles.ROLE_ADMIN }))
            {
                // map model to entity and set id
                var user = _mapper.Map<User>(model);
                user.Id = id;

                // so that a user cannot upgrade their privileages themselves
                if(_userService.CheckIf(currUser, Roles.ROLE_USER))
                {
                    user.Role = Roles.ROLE_USER;
                }

                try
                {
                    // update user 
                    _userService.Update(user, model.Password);
                    return Ok();
                }
                catch (AppException ex)
                {
                    // return error message if there was an exception
                    return BadRequest(new { message = ex.Message });
                }
            }
            else
            {
                return StatusCode(403, "Unauthorized! Only 'Admin' or 'User Manager' or the owner can update this resource");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var currUser = _userService.GetById(Int32.Parse(User.Identity.Name));
            if (id == currUser.Id || _userService.CheckIf(currUser, new string[] { Roles.ROLE_MANAGER, Roles.ROLE_ADMIN }))
            {
                _userService.Delete(id);
                return Ok();
            }
            else
            {
                return StatusCode(403, "Unauthorized! Only 'Admin' or 'User Manager' or the owner can delete this resource");
            }
        }
    }
}
