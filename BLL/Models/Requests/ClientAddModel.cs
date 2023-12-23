using MVC_BLL.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_BLL.Models.Requests
{
	public class ClientAddModel : IValidatableObject
	{

		[Required(ErrorMessage = "You must enter the client Name!")]
		[MaxLength(100), MinLength(3)]
		public string Name { get; set; }

		[Required(ErrorMessage = "You must enter the client Surname!")]
		[MaxLength(100), MinLength(3)]
		public string Surname { get; set; }

		[Required(ErrorMessage = "You must enter the client Phhone number!")]
		
		public string PhoneNum { get; set; }

		[Required(ErrorMessage = "You must enter the client email!")]
		[MaxLength(100), MinLength(3)]
		[EmailAddress]
		public string Email { get; set; }
        private string? _imgProfilePhoto { get; set; }
		[Required(ErrorMessage = "You must enter the UserName!")]
        [MaxLength(100), MinLength(3)]
        public string UserName { get;set; }
        public int TherapistLicenseNo { get;set; }
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

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
            if (PhoneNum == null || PhoneNum.Length != 10)
            {
                yield return new ValidationResult("Phone number's length must be exactly 10!");
            }

            if (Name != null)
            {
                foreach (char c in Name)
                {
                    if (char.IsNumber(c))
                    {
                        yield return new ValidationResult("Name cannot contain numbers!");
                    }
                }
            }

            if (Surname != null)
            {
                foreach (char c in Surname)
                {
                    if (char.IsNumber(c))
                    {
                        yield return new ValidationResult("Surname cannot contain numbers!");
                    }
                }
            }
        }
    }
}
