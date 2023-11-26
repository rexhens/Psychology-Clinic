using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_BLL.Services
{
		public interface ILoggerService
		{
			void LogError(string message);
			void LogError(Exception ex);
			void Debug(string message);
		}

		internal class LoggerService : ILoggerService
		{
			public LoggerService(IConfiguration configuration)
			{
				Dir = configuration["AppSettings:LogsDirectory"];
			}
			private string _dir;
			public string Dir
			{
				get { return _dir; }
				set
				{
					try
					{
						if (!Directory.Exists(value))
						{
							Directory.CreateDirectory(value);
							if (!Directory.Exists(value))
								throw new Exception("Direktoria nuk ekziston");
						}
						_dir = value;
					}
					catch { }
				}
			}
			private void LogToFile(string file, string message)
			{
				try
				{
					var fileName = $"{Dir}/{file}-{DateTime.UtcNow:dd-MM-yyyy}.txt";
					StringBuilder sb = new StringBuilder();
					sb.AppendLine($"========{DateTime.UtcNow:dd-MM-yyyy HH:mm:ss}=======");
					sb.AppendLine(message);
					using (StreamWriter sw = File.AppendText(fileName))
					{
						sw.WriteLine(sb.ToString());
						sw.Close();
					}
				}
				catch { }
			}
			public void LogError(string message)
			{
				LogToFile("Errors", message);
			}

			public void LogError(Exception ex)
			{
				LogToFile("Errors", ex.ToString());
			}

			public void Debug(string message)
			{
				LogToFile("Debug", message);
			}
		}
	}

