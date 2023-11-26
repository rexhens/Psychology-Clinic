using DAL.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    [Table("AspNetUsers")]
    public class Client : IdentityUser<int>
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public int? TherapistLicenseNo { get; set; }
        public string? ImgUrlProfilePic { get; set; }
        public Client()
        {
        }
    }

    [Table("UserRoles")]
    public class Role : IdentityRole<int>
    {

    }

}
