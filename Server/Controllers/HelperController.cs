using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Endava_Project.Server.Data;
using Endava_Project.Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Endava_Project.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelperController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public HelperController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [Route("{username}")]
        public bool CheckUser(string username)
        {
            var user = context.Users.FirstOrDefault(u => u.UserName == username);
            return user != null;
        }
    }
}
