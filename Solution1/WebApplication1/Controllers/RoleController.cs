using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class RoleController : Controller
    {

        private readonly UserManager<IdentityUser> _userManager;
        public RoleController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }


        public IActionResult AllocateRole()
        { return View(); }

        [HttpPost]
        public async  Task<IActionResult> AllocateRole(string role)
        {

            var username = HttpContext.User.Identity.Name;

            var user = await _userManager.FindByNameAsync(username);

            await _userManager.AddToRoleAsync(user, role);


            return View();
        }



        [HttpPost]
        public async Task<IActionResult> DeallocateRole(string role)
        {

            var username = HttpContext.User.Identity.Name;

            var user = await _userManager.FindByNameAsync(username);

            await _userManager.RemoveFromRoleAsync(user, role);


            return View();
        }
    }
}
