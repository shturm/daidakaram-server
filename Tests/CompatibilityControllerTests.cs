using System;
using System.Linq;
using System.Web.Http;
using DaiDaKaram.Domain.Entities;
using DaiDaKaram.Infrastructure.WebApi;
using DaiDaKaram.Infrastructure.WebApi.Controllers;
using NHibernate.Linq;
using NUnit.Framework;

namespace Integration
{
	public class CompatibilityControllerTests : ApiControllerTests<CompatibilityController>
	{
		[Test]
		[Category ("Integration")]
		public void GetMakes ()
		{
			var actual = Controller.GetMakes ();
			Assert.AreEqual (185, actual.Count ());
		}

		[Test]
		[Category ("Integration")]
		public void GetModels ()
		{
			var actual = Controller.GetModels ("bmw");
			Assert.AreEqual (38, actual.Count ());

			Assert.Throws<HttpResponseException> (() => {
				Controller.GetModels ("");
			});

			Assert.Throws<HttpResponseException> (() => {
				Controller.GetModels (null);
			});
		}

		[Test]
		[Category ("Integration")]
		public void GetVariants ()
		{
			var actual = Controller.GetVariants ("bmw", "3 Series");
			Assert.AreEqual (12, actual.Count ());
		}

		[Test]
		[Category ("Integration")]
		public void GetBodies ()
		{
			var actual = Controller.GetBodies ("bmw", "3 Series");
			Assert.AreEqual (5, actual.Count ());
		}

		[Test]
		[Category ("Integration")]
		public void GetYearFrom ()
		{
			var actual = Controller.GetYearFrom ("bmw", "3 Series");
			Assert.AreEqual (1975, actual);
		}

		[Test]
		[Category ("Integration")]
		public void GetYearTo ()
		{
			var actual = Controller.GetYearTo ("bmw", "3 Series");
			Assert.AreEqual (2016, actual);
		}

		[Test]
		[Category ("Integration")]
		public void GetTypes ()
		{
			var actual = Controller.GetTypes ("bmw", "3 Series");
			Assert.AreEqual (76, actual.Count ());
		}

		[Test]
		[Category ("Integration")]
		public void GetProductCompatibility ()
		{
			int productId = 0;
			using (var tx = Session.BeginTransaction ()) {
				var prod = new Product () { Name = "Compatibility Test Product" };

				var cSetting1 = new CompatibilitySetting () { Make = "BMW", Model = "3 Series", Variant = "E36", Product = prod };
				var cSetting2 = new CompatibilitySetting () { Make = "BMW", Model = "3 Series", Variant = "F80", Product = prod };
				var cSettingNonUsed = new CompatibilitySetting () {};
				Session.Save (cSetting1);
				Session.Save (cSetting2);
				Session.Save (cSettingNonUsed);
				productId = prod.Id;
				tx.Commit ();
			}

			var actual = Controller.GetProductCompatibilitySettings (productId);
			Assert.AreEqual (2, actual.Count ());
		}

		[Test]
		[Category ("Integration")]
		public void CreateCompatibility_NoneVariantsSpecified_CompactlyUsesSingleRecord ()
		{
			var p = new Product ();
			using (var tx = Session.BeginTransaction ()) {
				Session.Save (p);
				tx.Commit ();
			}

			var cmd = new CreateCompatibilityCommand () {
				Make = "BMW",
				Model = "3 Series",
				ProductId = p.Id
			};

			Controller.CreateCompatibility (cmd);

			using (var tx = Session.BeginTransaction ()) {
				var dbCount = Session.Query<CompatibilitySetting> ().Count ();
				Assert.AreEqual (1, dbCount);
			}
		}

		[Test]
		[Category ("Integration")]
		public void CreateCompatibility_AllVariantsSpecified_CompactlyUsesSingleRecord ()
		{
			var p = new Product ();
			using (var tx = Session.BeginTransaction ()) {
				Session.Save (p);
				tx.Commit ();
			}

			var cmd = new CreateCompatibilityCommand () {
				Make = "BMW",
				Model = "3 Series",
				Variants = new string[] {"E21", "E30", "E36", "E91", "E90", "E46", "F30", "F30, F35, F80", "E92", "E93", "F30, F35", "F31" },
				ProductId = p.Id
			};

			Controller.CreateCompatibility (cmd);

			using (var tx = Session.BeginTransaction ()) {
				var dbCount = Session.Query<CompatibilitySetting> ().Count ();
				Assert.AreEqual (1, dbCount);
			}
		}

		[Test]
		[Category ("Integration")]
		public void CreateCompatibility_SomeVariantsSpecified_UsesIndividualRecords ()
		{
			var p = new Product ();
			using (var tx = Session.BeginTransaction ()) {
				Session.Save (p);
				tx.Commit ();
			}

			var cmd = new CreateCompatibilityCommand () {
				Make = "BMW",
				Model = "3 Series",
				Variants = new string [] { "E21", "E30", "E36" },
				ProductId = p.Id
			};

			Controller.CreateCompatibility (cmd);

			using (var tx = Session.BeginTransaction ()) {
				var dbCount = Session.Query<CompatibilitySetting> ().Count ();
				Assert.AreEqual (3, dbCount);
			}
		}

	}
}