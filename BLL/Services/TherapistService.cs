using DAL;
using MVC_BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_BLL.Services
{
	public interface ITherapistService
	{
		List<Models.Therapist> GetTherapists();
	}
	public class TherapistService : BaseService, ITherapistService
	{
		public TherapistService(IServiceProvider unitOfWork) : base(unitOfWork)
		{
		}
		public List<Therapist> GetTherapists()
		{
			var result = new List<Therapist>();
			try
			{
				var dalTherapist = _unitOfWork.TherapistRepository.GetAll();
				foreach (var d in dalTherapist)
				{
					result.Add(
						new Models.Therapist
						{
							LicenseNo = d.Id,
							Degree = d.Degree,
							FullName = d.FullName,
							YearsExperience = d.YearsExperience,
							Department = d.Department,
						});
				}
			}
			catch
			{

			}
			return result;
		}
	}
}
