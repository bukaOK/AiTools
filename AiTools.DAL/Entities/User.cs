using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AiTools.DAL.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string SirName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string InviteKey { get; set; }
    }
}
