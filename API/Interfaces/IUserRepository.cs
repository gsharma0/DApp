using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        public void Update(AppUser user);

        public Task<bool> SaveAllAsync();

        public Task<AppUser> GetUserByIdAsync(int id);

        public Task<IEnumerable<AppUser>> GetUsersAsync();

        public Task<MemberDto> GetMemberByNameAsync(string username);

        //Commented to use pagination feature
       // public Task<IEnumerable<MemberDto>> GetMembersAsync();
        public Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);

        public Task<AppUser> GetUserByNameAsync(string username);
    }
}