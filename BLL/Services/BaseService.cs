using DAL;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Repositories;

namespace MVC_BLL.Services
{
	public abstract class BaseService
	{
		protected readonly IUnitOfWork _unitOfWork;
		protected readonly ILoggerService _loggerService;
		public BaseService(IServiceProvider serviceProvider)
		{
			_unitOfWork = serviceProvider.GetService(typeof(IUnitOfWork)) as IUnitOfWork;
			_loggerService = serviceProvider.GetService(typeof(ILoggerService)) as ILoggerService;
		}
	}
}

