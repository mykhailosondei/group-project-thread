﻿using ApplicationBLL.Services;
using ApplicationCommon.Interfaces;
using ApplicationDAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace group_project_thread.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {

        private readonly UserService _userService;
        private readonly IUserIdGetter _userIdGetter;

        public UserController(UserService userService, IUserIdGetter userIdGetter)
        {
            _userService = userService;
            _userIdGetter = userIdGetter;
        }

        // GET: api/<UserController>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _userService.GetAllUsers();
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<User> GetUserById(int id)
        {
            return await _userService.GetUserById(id);
        }
        
        [HttpGet("currentUser")]
        [Authorize]
        public async Task<User> GetCurrentUser()
        {
            return await _userService.GetUserById(_userIdGetter.CurrentId);
        }
        
        [HttpPost("{id}/follow")]
        public async Task Follow(int id)
        {
            int currentUserId = _userIdGetter.CurrentId; 
            await _userService.Follow(id, currentUserId);
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public async Task PutUser(int id, [FromBody] User value)
        {
            await _userService.PutUser(id, value);
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public async Task DeleteUser(int id)
        {
            await _userService.DeleteUser(id);
        }
        [HttpPost("{id}/unfollow")]
        public async Task Unfollow(int id)
        {
            int currentUserId = _userIdGetter.CurrentId;
            await _userService.Follow(id, currentUserId);
        }
    }
}
