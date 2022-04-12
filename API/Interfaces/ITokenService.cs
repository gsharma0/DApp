using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.Interfaces
{
    public interface ITokenService
    {
        //coomented to get roles which uses asycn methos
        //public string CreateToken(AppUser user);
        public Task<string> CreateToken(AppUser user);
    }
}