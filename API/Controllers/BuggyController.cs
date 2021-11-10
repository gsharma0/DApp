using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    public class BuggyController : BaseApiController
    {
        private readonly DataContext _context;

        public BuggyController(DataContext context)
        {
            this._context = context;
        }

      [HttpGet("server-error")]
        public ActionResult<string> GetServerError(){

            var test= _context.Users.Find(-1);

            return test.ToString();
        }

        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound(){

            var test= _context.Users.Find(-1);
            if(test==null) return NotFound("Not Found");
            return Ok(test);
        }

        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest(){

           return BadRequest("Bad Request found");
        }

    }
}