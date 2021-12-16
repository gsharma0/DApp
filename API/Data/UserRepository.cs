using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DataContext context,IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<MemberDto> GetMemberByNameAsync(string username)
        {
            return await _context.Users
            .Where(x=>x.UserName==username)
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {

            //changes done for adding filtering
           /* var query = _context.Users
                        .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                        .AsNoTracking(); */

            var query= _context.Users.AsQueryable();
            query = query.Where(user => user.UserName != userParams.CurrentUsername);
            query = query.Where(user => user.Gender == userParams.Gender);

            var minAge = DateTime.Now.AddYears(-userParams.MaxAge-1);
            var maxAge = DateTime.Now.AddYears(-userParams.MinAge);

            query = query.Where(user => user.DateOfBirth >= minAge && user.DateOfBirth <= maxAge);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u => u.LastActive)
            };

             var finalquery =query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking();

            return await PagedList<MemberDto>.CreateAsync(finalquery,userParams.PageNumber,userParams.PageSize);
        }

        //Commented to use pagination feature
        // public async Task<IEnumerable<MemberDto>> GetMembersAsync()
        // {
        //      return await _context.Users
        //     .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
        //     .ToListAsync();
        // }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByNameAsync(string username)
        {
            return await _context.Users.Include(p=>p.Photos).SingleOrDefaultAsync(x=>x.UserName==username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
            .Include(p=>p.Photos)
            .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
           return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State= EntityState.Modified;
        }
    }
}