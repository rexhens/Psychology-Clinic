using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public interface IBaseRepository<T,T1>
	{
		T GetById(T1 id);
		bool Delete(T1 id);
		T1 Add(T entity);
		IEnumerable<T> GetAll();
		T Update(T entity);

	}
	public abstract class BaseRepository<T,T1> where T : BaseEntity<T1>
	{
		protected readonly DbSet<T> _set;
		public BaseRepository(Psychology_ClinicDBContext context)
		{
			_set = context.Set<T>();
		}
		public IEnumerable<T> GetAll()
		{
			return _set.ToList();
		}
		
		public T GetById(T1 id)
		{
			return _set.Find(id);
		}

		public T1 Add(T entity)
		{
			_set.Add(entity);
			return entity.Id;
		}
		public bool Delete(T1 id)
		{
			var entity = GetById(id);
			_set.Remove(entity);
			return true;
		}

		public T Update(T entity)
		{
			var exist = _set.Find(entity.Id);
			if (exist != null)
			{
				_set.Update(entity);
				
			}
			return entity;
		}
	}
}
