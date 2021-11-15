using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        //Used repository,context is used inside it, so move the context out
        //private readonly DataContext _context;
        // public UsersController(DataContext context)
        // {
        //     _context = context;
        // }
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository,IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        [HttpGet]
        //[AllowAnonymous]
        //public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            //return await _context.Users.ToListAsync();
           // return Ok(await _userRepository.GetUsersAsync());
        //    var users=await _userRepository.GetUsersAsync();
        //    var userstoReturn = _mapper.Map<IEnumerable<MemberDto>>(users);
        //    return Ok(userstoReturn);
        var users= await _userRepository.GetMembersAsync();
        return Ok(users);

        }

        /*  [HttpGet("{id}")]
        // [Authorize]
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
           // var user = _context.Users.Find(id);
            return await _userRepository.GetUserByIdAsync(id);

        } */

        [HttpGet("{username}")]
        // [Authorize]
       // public async Task<ActionResult<AppUser>> GetUser(string username)
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
           // var user = _context.Users.Find(id);
           // return await _userRepository.GetUserByNameAsync(username);
        //    var user =await _userRepository.GetUserByNameAsync(username);
        //     return _mapper.Map<MemberDto>(user);

        return await _userRepository.GetMemberByNameAsync(username);

        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(UpdateMemberDto member){
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user= await _userRepository.GetUserByNameAsync(username);

            _mapper.Map(member,user);
           _userRepository.Update(user);
          if(await  _userRepository.SaveAllAsync()) return NoContent();
          return BadRequest("Failed");
        }
    }

    
}