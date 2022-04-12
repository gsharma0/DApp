using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
       // private readonly DataContext _context;
        public ITokenService _token { get; set; }

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMapper _mapper;
        //public AccountController(DataContext context, ITokenService token, IMapper mapper)
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
             ITokenService token, IMapper mapper)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _token = token;
           // _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerdto)
        {
            if (await IsUserNameExists(registerdto.username)) return BadRequest("user name exists");
            var user = _mapper.Map<AppUser>(registerdto);
            user.UserName = registerdto.username.ToLower();

            //Commented to use Microsoft Identity Feature
           /*  using var hmac = new HMACSHA512();       

            // var user = new AppUser()
            // {
                
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerdto.password));
                user.PasswordSalt = hmac.Key; */
            //};

           // _context.Users.Add(user);
           // await _context.SaveChangesAsync();

          var result = await _userManager.CreateAsync(user, registerdto.password);

          if(!result.Succeeded) return BadRequest(result.Errors);

         result= await _userManager.AddToRoleAsync(user,"Member");

         if(!result.Succeeded) return BadRequest(result.Errors);

           return new UserDto
            {
                username = user.UserName,
                Token = await _token.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender                
            };

        }

        private async Task<bool> IsUserNameExists(string username)
        {
           // return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
           return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto logindto)
        {
            //var user = await _context.Users
            var user = await _userManager.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == logindto.UserName.ToLower());

            if (user == null) return Unauthorized("Invalid User");

            //Commented to use Microsoft Identity Feature
            /* using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedhash = hmac.ComputeHash(Encoding.UTF8.GetBytes(logindto.Password));

            for (int i = 0; i < computedhash.Length; i++)
            {
                if (computedhash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            } */

            var result = await _signInManager.CheckPasswordSignInAsync(user,logindto.Password,false);
            if(!result.Succeeded) return Unauthorized();

            return new UserDto
            {
                username = user.UserName,
                Token = await _token.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.isMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };

    }


    }
}