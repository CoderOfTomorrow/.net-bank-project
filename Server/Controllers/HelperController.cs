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
        public List<string> GetUsers()
        {
            var users_list = new List<string>();
            foreach(var data in context.Users)
            {
                string info = data.UserName;
                users_list.Add(info);
            }
            return users_list;
        }
    }
}
