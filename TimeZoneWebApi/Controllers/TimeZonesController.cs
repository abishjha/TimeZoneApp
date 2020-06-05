using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeZoneWebApi.Entities;
using TimeZoneWebApi.Helpers;
using TimeZoneWebApi.Services;
using TimeZoneWebApi.Models.TimeZones;
using Microsoft.AspNetCore.Authorization;
using TimeZoneWebApi.Models;

namespace TimeZoneWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TimeZonesController : ControllerBase
    {
        private ITimeZoneService _timeZoneService;
        private IUserService _userService;
        private IMapper _mapper;

        public TimeZonesController(
            IUserService userService,
            ITimeZoneService timeZoneService,
            IMapper mapper)
        {
            _timeZoneService = timeZoneService;
            _userService = userService;
            _mapper = mapper;
        }

        // GET: api/TimeZones
        [HttpGet]
        public IActionResult GetAll()
        {
            var currUser = _userService.GetById(Int32.Parse(User.Identity.Name));

            var timeZones = _timeZoneService.GetAll();
            // if the current user is not an admin, only return the time zones that he is the owner of
            if (!_userService.CheckIf(currUser, Roles.ROLE_ADMIN))
            {
                timeZones = timeZones.Where(x => x.UserId == currUser.Id);
            }
            var model = _mapper.Map<IList<TimeZoneModel>>(timeZones);
            return Ok(model);
        }

        // GET: api/TimeZones/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var timeZone = _timeZoneService.GetById(id);

            var currUser = _userService.GetById(Int32.Parse(User.Identity.Name));
            if (timeZone.UserId == currUser.Id || _userService.CheckIf(currUser, Roles.ROLE_ADMIN))
            {
                var model = _mapper.Map<TimeZoneModel>(timeZone);
                return Ok(model);
            }
            else
            {
                return StatusCode(403, "Unauthorized! Only 'Admin' or the owner can access this resource");
            }
        }

        // PUT: api/TimeZones/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]TimeZoneUpdateModel model)
        {
            // map model to entity and set id
            var timeZone = _mapper.Map<Entities.TimeZone>(model);
            timeZone.Id = id;

            var currUser = _userService.GetById(Int32.Parse(User.Identity.Name));
            if (timeZone.UserId == currUser.Id || _userService.CheckIf(currUser, Roles.ROLE_ADMIN))
            {
                try
                {
                    _timeZoneService.Update(timeZone);
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
                return StatusCode(403, "Unauthorized! Only 'Admin' or the owner can update this resource");
            }
        }

        // POST: api/TimeZones
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public IActionResult Create([FromBody]TimeZoneCreateModel model)
        {
            var currUser = _userService.GetById(Int32.Parse(User.Identity.Name));
            // if an admin is inserting a row, it might be for any user so preserve the user id
            if(! _userService.CheckIf(currUser, Roles.ROLE_ADMIN))
            {
                model.UserId = currUser.Id;
            }

            // map model to entity
            var timeZone = _mapper.Map<Entities.TimeZone>(model);

            try
            {
                _timeZoneService.Create(timeZone);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/TimeZones/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var timeZone = _timeZoneService.GetById(id);
            var currUser = _userService.GetById(Int32.Parse(User.Identity.Name));
            if (_userService.CheckIf(currUser, Roles.ROLE_ADMIN) || timeZone.UserId == currUser.Id)
            {
                _timeZoneService.Delete(id);
                return Ok();
            }
            else
            {
                return StatusCode(403, "Unauthorized! Only 'Admin' or the owner can delete this resource");
            }
        }
    }
}
