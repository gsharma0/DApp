using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        //private readonly IUserRepository _userRepository;commented due to UOF
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _photo;
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(IUnitOfWork unitOfWork,IMapper mapper, ICloudinaryService photo )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _photo = photo;
            
            //_userRepository = userRepository;commented due to UOF
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet]
        //[AllowAnonymous]
        //public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
        {
            //return await _context.Users.ToListAsync();
           // return Ok(await _userRepository.GetUsersAsync());
        //    var users=await _userRepository.GetUsersAsync();
        //    var userstoReturn = _mapper.Map<IEnumerable<MemberDto>>(users);
        //    return Ok(userstoReturn);
        //Comented below for pagination
        //var users= await _userRepository.GetMembersAsync();
        var username = User.GetUserName();
       // var user= await  _unitOfWork.userRepository.GetUserByNameAsync(username); optimization
        var gender= await  _unitOfWork.userRepository.GetUserGender(username);

        userParams.CurrentUsername = username;

        if(string.IsNullOrEmpty(userParams.Gender)){
            userParams.Gender = gender == "male" ? "female" :"male";
        }


        var users= await _unitOfWork.userRepository.GetMembersAsync(userParams);
        Response.AddPaginationHeaders(users.CurrentPage, users.TotalPages, users.PageSize,users.TotalCount);
        return Ok(users);

        }

        /*  [HttpGet("{id}")]
        // [Authorize]
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
           // var user = _context.Users.Find(id);
            return await _userRepository.GetUserByIdAsync(id);

        } */
        [Authorize(Roles = "Member")]
        [HttpGet("{username}", Name ="GetUser")]
        // [Authorize]
       // public async Task<ActionResult<AppUser>> GetUser(string username)
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
           // var user = _context.Users.Find(id);
           // return await _userRepository.GetUserByNameAsync(username);
        //    var user =await _userRepository.GetUserByNameAsync(username);
        //     return _mapper.Map<MemberDto>(user);

        return await _unitOfWork.userRepository.GetMemberByNameAsync(username);

        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(UpdateMemberDto member){
            var username = User.GetUserName();
            var user= await _unitOfWork.userRepository.GetUserByNameAsync(username);

            _mapper.Map(member,user);
           _unitOfWork.userRepository.Update(user);
          if(await  _unitOfWork.Complete()) return NoContent();
          return BadRequest("Failed");
        }
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
             var username = User.GetUserName();
            var user= await _unitOfWork.userRepository.GetUserByNameAsync(username);
            var result = await _photo.UploadPhotoAsync(file);
            if(result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo{
                Url = result.SecureUrl.AbsoluteUri,
                PublicID = result.PublicId
            };
            // if(user.Photos.Count==0){
            //     photo.isMain = true;
            // }

            user.Photos.Add(photo);
            if(await _unitOfWork.Complete()){
                 //return _mapper.Map<PhotoDto>(photo); in order to give location in response used other method 
                 return CreatedAtRoute("GetUser", new {username = user.UserName},_mapper.Map<PhotoDto>(photo));
            }
           

            return BadRequest("Error");
        }

         [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
             var username = User.GetUserName();
            var user= await _unitOfWork.userRepository.GetUserByNameAsync(username);

            var photo = user.Photos.FirstOrDefault(x=>x.ID == photoId);

            if(photo.isMain) return BadRequest("Already Main");
            if(!photo.isApproved) return BadRequest("Not Approved Yet");

            var mainPhoto = user.Photos.FirstOrDefault(x=>x.isMain);
            if(mainPhoto !=null) mainPhoto.isMain=false;
            

            photo.isMain=true;
            if(await _unitOfWork.Complete()) return NoContent();

            return BadRequest("Error");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto (int photoId){
             var username = User.GetUserName();
            var user= await _unitOfWork.userRepository.GetUserByNameAsync(username);
            var photo = user.Photos.FirstOrDefault(x=>x.ID == photoId);
            if(photo == null) return NotFound("Invalid Details");
            if(photo.isMain) return BadRequest("Can't delete Main");
            if(photo.PublicID != null){
              var result = await _photo.DeletePhotoAsync(photo.PublicID);
              if (result.Error !=null) return BadRequest(result.Error.Message);
            }
            user.Photos.Remove(photo);
            if(await _unitOfWork.Complete()) return Ok();

            return BadRequest("Error");


        }

    }

    
}