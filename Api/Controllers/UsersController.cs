using api.DTO;
using Api.DTO;
using Api.Entities;
using Api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var user = await _userRepository.GetUsersAsync();
            return Ok(_mapper.Map<IEnumerable<MemberDto>>(user));
        }
        [Authorize]
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUserByUserNameAsync(string username)
        {
            var user = await _userRepository.GetUserByUserNameAsync(username);
            return Ok(_mapper.Map<MemberDto>(user));
        }
        [HttpDelete("delete-user/{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            await _userRepository.DeleteUserAsync(id);
            return Ok();
        }
        [Authorize(Policy = "EditUser")]
        [HttpPut("update-user")]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userRepository.GetUserByUserNameAsync(username);
            _mapper.Map(memberUpdateDto, user);
            _userRepository.UpdateUser(user);
            if (await _userRepository.SaveAllAsync())
                return NoContent();
            return BadRequest("Fail");
        }
    }
}
