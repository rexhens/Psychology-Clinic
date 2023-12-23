using Microsoft.AspNetCore.Identity;
using MVC_BLL.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_BLL.Models
{
    public class Client 
    {
        public int Id { get; set; }
     
        public string Name { get; set; }
		public string Surname { get; set; }
		public string? PhoneNum { get; set; }
		public string Email { get; set; }

        public int? TherapistLicenseNo = 1;
        private string? _imgProfilePhoto;
        public string? ImgUrlProfilePhoto
        {
            get
            {
                if (_imgProfilePhoto == null)
                {
                    _imgProfilePhoto = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRSvRRdb4RulPsloRl5u8j8V9JrlrOoxZe1K0XOAGc&s";
                }
                return _imgProfilePhoto;
            }
            set
            {
                _imgProfilePhoto = value;
            }
        }
        [Required(ErrorMessage = "You must enter the UserName!")]
        [MaxLength(100), MinLength(3)]
        public string UserName { get; set; }
        
     


    }
}
