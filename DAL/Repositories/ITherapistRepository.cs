using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
	public interface ITherapistRepository: IBaseRepository<Therapist,int>
	{
	}
	public class TherapistRepository : BaseRepository<Therapist, int>, ITherapistRepository
	{
		public TherapistRepository(Psychology_ClinicDBContext context) : base(context)
		{

		}
	}


}
