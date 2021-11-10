using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class CalcController : BaseApiController
    {
        private readonly ICalculator _calc;

        public CalcController(ICalculator calc)
        {
            _calc = calc;
        }
        
        [HttpGet("{x}/{y}/{operation}")]
        [AllowAnonymous]
        public ActionResult<int> Calculate(int x, int y, string operation)
        {
            var result=0;
            if(operation == "ADD"){
               result = _calc.Add(x,y);
            }

            return result;
        }
    }
}