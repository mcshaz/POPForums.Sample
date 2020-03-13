using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PopForums.Sample.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            // this is a hack because the PopForums library redirects to the wrong place - remove entire controller when fixed
            var queryString = Request.QueryString;
            return LocalRedirectPermanentPreserveMethod("/Forums/Account/Login" + queryString);
        }
    }
}