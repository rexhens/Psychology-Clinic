using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{

	public abstract class DBEntity
	{

	}
	public abstract class BaseEntity<T1> : DBEntity
	{
		public T1 Id { get; set; }
	}

}
