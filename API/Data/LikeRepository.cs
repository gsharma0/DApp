using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikeRepository : ILikesRepository
    {
        private readonly DataContext _context;
        public LikeRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
           return await _context.Likes.FindAsync(sourceUserId,likedUserId);

        }
        //Commente for new method of pagiation
        /* public async Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, int userId)
        {
            var likes = _context.Likes.AsQueryable();
            var users = _context.Users.OrderBy(x=>x.UserName).AsQueryable();

            if(predicate == "liked"){
                likes = likes.Where(like=>like.SourceUserId == userId);
                users = likes.Select(x=>x.LikedUser);
            }

             if(predicate == "likedBy"){
                likes = likes.Where(like=>like.LikedUserId == userId);
                users = likes.Select(x=>x.SourceUser);
            }

            return await users.Select(user => new LikeDto{
                Username = user.UserName,
                knownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(x=>x.isMain).Url,
                City = user.City,
                Id = user.Id
            }).ToListAsync();
        }
         */
        public async Task<PagedList<LikeDto>> GetUserLikes(LikeParams likeParams)
        {
            var likes = _context.Likes.AsQueryable();
            var users = _context.Users.OrderBy(x=>x.UserName).AsQueryable();

            if(likeParams.Predicate == "liked"){
                likes = likes.Where(like=>like.SourceUserId == likeParams.UserId);
                users = likes.Select(x=>x.LikedUser);
            }

             if(likeParams.Predicate == "likedBy"){
                likes = likes.Where(like=>like.LikedUserId == likeParams.UserId);
                users = likes.Select(x=>x.SourceUser);
            }

            var likedUsers = users.Select(user => new LikeDto{
                Username = user.UserName,
                knownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(x=>x.isMain).Url,
                City = user.City,
                Id = user.Id
            });

            return await PagedList<LikeDto>.CreateAsync(likedUsers,likeParams.PageNumber, likeParams.PageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users
            .Include(x=>x.LikedUsers)
            .FirstOrDefaultAsync(x=>x.Id == userId);
        }
    }
}