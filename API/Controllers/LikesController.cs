using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LikesController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        public LikesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //commented due to UOF
        // private readonly ILikesRepository _likesRepository;
        //private readonly IUserRepository _userRepository;
        // public LikesController(ILikesRepository likesRepository, IUserRepository userRepository)
        // {
        //     _userRepository = userRepository;
        //     _likesRepository = likesRepository;

        // }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await _unitOfWork.userRepository.GetUserByNameAsync(username);
            var sourceUser = await _unitOfWork.likesRepository.GetUserWithLikes(sourceUserId);

            if(likedUser == null) return NotFound();
            if(sourceUser.UserName == username) return BadRequest("You can't like yourself");

           var userLike = await _unitOfWork.likesRepository.GetUserLike(sourceUserId, likedUser.Id);
           if(userLike != null) return BadRequest ("You already liked this user");

           userLike = new UserLike
           {
               SourceUserId = sourceUserId,
               LikedUserId = likedUser.Id
           };

           sourceUser.LikedUsers.Add(userLike);

           if( await _unitOfWork.Complete()) return Ok();
            return BadRequest ("Failed");

        }
        
        //commented for pagination
        /* public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes(string predicate)
        {
            var userId= User.GetUserId();
            var users = await _likesRepository.GetUserLikes(predicate, userId );
            return Ok(users);
        } */

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery]LikeParams likeparams)
        {
            likeparams.UserId= User.GetUserId();
            var users = await _unitOfWork.likesRepository.GetUserLikes(likeparams);
            Response.AddPaginationHeaders(users.CurrentPage, users.TotalPages, users.PageSize,users.TotalCount);
            return Ok(users);
        }
    }
}