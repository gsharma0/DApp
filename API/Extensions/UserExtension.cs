using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Core;


namespace API.Extensions
{
    public static class UserExtension
    {
        public static string GetUserName(this ClaimsPrincipal user)
        {
            var username = user.FindFirst(ClaimTypes.Name)?.Value;
            return username;
        }

        public static int GetUserId(this ClaimsPrincipal user)
        {
            return  Convert.ToInt32(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
        }
    }
}