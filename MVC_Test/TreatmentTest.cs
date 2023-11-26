using System.Collections.Generic;
using DAL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVC_BLL.Services;
using Xunit.Sdk;

namespace MVC_Test
{
	[TestClass]
	public class TreatmentTest
	{
		private ITreatmentsService _treatmentsService;

		public TreatmentTest()
		{
			var appConfig = new Dictionary<string, string> {
				{"ConnectionStrings:DBConString","Data Source=DESKTOP-QHMSTK7\\SQLEXPRESS;Initial Catalog=FinalProjectDB;Integrated Security=True"}
			};
			IConfiguration config = new ConfigurationBuilder()
											.AddInMemoryCollection(appConfig)
											.Build();
			var serviceCollection = new ServiceCollection()
				.AddSingleton<IConfiguration>(config)
				;
			//DAL.StartUp.RegisterServices(serviceCollection, config);
			MVC_BLL.StartUp.RegisterServices(serviceCollection, config);
			IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();


			_treatmentsService = serviceProvider.GetService(typeof(ITreatmentsService)) as ITreatmentsService;
		}

		[TestMethod]
		public void Test_001_ShouldListDataOK()
		{
			var treatments = _treatmentsService.GetAll();
			Assert.IsNotNull(treatments, "Treatment list is null!");
			Assert.IsTrue(treatments.Count() > 0, "There are no treatments retreaved!");
		}
		[TestMethod]
		public void Test_002_ShouldInsertTreatment()
		{
			var treatment = new MVC_BLL.Models.Treatment
			{
				StartDate = DateTime.UtcNow,
				Description = "Description",
				ClientId = 5,
				Cause = "Cause"
			    
			};
			bool  isAdded = _treatmentsService.Add(treatment);
			Assert.IsTrue(isAdded, "Treatment cannot be added to the DB!");

		}
		[TestMethod]
		public void Test_003_ShouldNotInsertWithSameClientId()
		{
			var treatment = new MVC_BLL.Models.Treatment
			{
				StartDate = DateTime.UtcNow,
				Description = "Description",
				ClientId = 5,
				Cause = "Cause"

			};
			var added = _treatmentsService.Add(treatment);
			Assert.IsTrue(treatment.Id == 0);
			Assert.IsTrue(!added);
		}

		[TestMethod]
		public void Test_004_ShouldNotInsertWithStarTadeInThePast()
		{
			var treatment = new MVC_BLL.Models.Treatment
			{
				StartDate = DateTime.UtcNow.AddDays(-1),
				Description = "Description",
				ClientId = 6,
				Cause = "Cause"

			};
			var added = _treatmentsService.Add(treatment);
			Assert.IsTrue(treatment.Id == 0);
			Assert.IsTrue(!added);
		}
		[TestMethod]
		public void Test_005_ShouldNotInsertInvalidDate()
		{
			var date = new DateTime(1, 1, 1);
			var treatment = new MVC_BLL.Models.Treatment
			{
				StartDate = date,
				Description = "Description",
				ClientId = 6,
				Cause = "Cause"

			};
			var added = _treatmentsService.Add(treatment);
			Assert.IsTrue(treatment.Id == 0,"Date must be valid!");
			Assert.IsTrue(!added, "Date must be valid!");
		}
		[TestMethod]
		public void Test_006_CanInstertWithEndDate()
		{
			var treatment = new MVC_BLL.Models.Treatment
			{
				StartDate = DateTime.Now.AddMinutes(10),
				EndDate = DateTime.Now.AddYears(1),
				Description = "Description",
				ClientId = 43,
				Cause = "Cause"

			};
			bool isAdded = _treatmentsService.Add(treatment);
			Assert.IsTrue(isAdded, "Treatment cannot be added to the DB!");

		}
		[TestMethod]
		public void Test_007_CanInstertWithoutEndDate()
		{
			var treatment = new MVC_BLL.Models.Treatment
			{
				StartDate = DateTime.Now.AddMinutes(10),
				Description = "Description",
				ClientId = 42,
				Cause = "Cause"

			};
			bool isAdded = _treatmentsService.Add(treatment);
			Assert.IsTrue(isAdded, "Treatment cannot be added to the DB!");

		}
		[TestMethod]
		public void Test_008_ShouldNotInsertWithStartDateGreater()
		{
			var treatment = new MVC_BLL.Models.Treatment
			{
				StartDate = DateTime.Now.AddMinutes(30),
				EndDate = DateTime.Now.AddMinutes(20),
				Description = "Description",
				ClientId = 3,
				Cause = "Cause"

			};
			var added = _treatmentsService.Add(treatment);
			Assert.IsTrue(treatment.Id == 0, "Date must be valid!");
			Assert.IsTrue(!added, "Date must be valid!");
		}
		[TestMethod]
		public void Test_009_GetTreatmentById()
		{
			var treatment = _treatmentsService.GetById(22);
			Assert.IsTrue(treatment.Id != 0);
			Assert.IsTrue(treatment != null);
		}
		[TestMethod]
		public void Test_010_GetTreatmentByCLientId()
		{
			var treatment = _treatmentsService.GetTreatmentByClientId(43,43);
			Assert.IsTrue(treatment.Id != 0);
			Assert.IsTrue(treatment != null);
		}
		[TestMethod]
		public async Task Test_011_ShouldGetClientByTreatmentIdAsync()
		{
			var client = await _treatmentsService.GetClientAsync(43,43);
			Assert.IsTrue(client.Id != 0);
			Assert.IsTrue(client != null);
		}
		[TestMethod]
		public void Test_012_ShouldUpdateTreatmentById()
		{
			int treatmentId = 24;

			var treatment = _treatmentsService.GetById(treatmentId);
			Assert.IsNotNull(treatment, "Treatment with ID 24 does not exist.");

			var originalCause = treatment.Cause;
			var newCause = "Depression123";

			
			treatment.Cause = newCause;
			bool updated = _treatmentsService.Update(treatment);

		
			Assert.IsTrue(updated, "Update operation failed.");

			var updatedTreatment = _treatmentsService.GetById(treatmentId);
			Assert.IsNotNull(updatedTreatment, "Failed to retrieve updated treatment.");

			Assert.AreNotEqual(originalCause, updatedTreatment.Cause, "Cause did not change after the update.");
			Assert.AreEqual(newCause, updatedTreatment.Cause, "Cause was not updated to expected value.");
		}

		[TestMethod]
		public void Test_013_ShouldDeleteById()
		{
			var treatmentId = 26;
			var deleted = _treatmentsService.Delete(treatmentId);
			var treatment = _treatmentsService.GetById(treatmentId);
			Assert.IsTrue(deleted);
			Assert.IsTrue(treatment == null);
		}
		[TestMethod]
		public async Task Test_014_ShouldGetTreatmentAddModelByClientId()
		{
			var clientId = 5;
			var treatment = await _treatmentsService.GetByIdAddModelAsync(clientId);
			Assert.IsTrue(treatment != null);
			Assert.IsFalse(treatment.ClientId == 0);
			Assert.AreEqual(clientId, treatment.ClientId);

		}
		[TestMethod]
		public void Test_015_ShouldAddTreatmentAddModelAsync()
		{
			var treatment = new MVC_BLL.Models.Requests.TreatmentAddModel
			{
				StartDate = DateTime.Now.AddMinutes(30),
				EndDate = DateTime.Now.AddMinutes(20),
				Description = "Description",
				ClientId = 3,
				Cause = "Cause"
			};
			 bool added = _treatmentsService.Add(treatment);
			Assert.IsTrue(added);
		}

	}
}
