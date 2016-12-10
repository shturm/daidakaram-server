using System;
using System.Collections.Generic;
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
		public void CreateCompatibility_NoneSpecified_SavesGenericRecord ()
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
		public void CreateCompatibility_AllSpecified_NoneExist_SavesGenericRecord ()
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
		[Category("Database")]
		[Description("When creating SOME variants" +
		             "And NONE exist" +
		             "Individual records are saved")]
		public void CreateCompatibility_SomeSpecified_NoneExist_SavesIndividualRecords ()
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

		[Test]
		[Category ("Integration")]
		[Category ("Database")]
		[Description("When create some settings" +
		             "And overlapping settings exist" +
		             "Then only unique settings should be saved")]
		public void CreateCompatibility_SomeSpecified_SomeExist_SavesUniqueRecords ()
		{
			// arrange
			var p = new Product () {
				CompatibilitySettings = new List<CompatibilitySetting>() {
					new CompatibilitySetting (){Make="BMW",Model="3 Series",Variant="E21"},
					new CompatibilitySetting (){Make="BMW",Model="3 Series",Variant="E30"},
				}
			};
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

			// act
			Controller.CreateCompatibility (cmd);

			// assert
			using (var tx = Session.BeginTransaction ()) {
				var variants = Session.Query<CompatibilitySetting> ().Select(s=>s.Variant).ToList ();
				Assert.AreEqual (3, variants.Count());
				Assert.Contains ("E36", variants);
			}
		}

		[Test]
		[Category ("Integration")]
		[Category ("Database")]
		[Description ("When create some settings" +
		              "And setting for all settings exist" +
					 "Then no settings should be saved")]
		public void CreateCompatibility_SomeSpecified_AllExist_SavesNothing ()
		{
			// arrange
			var p = new Product () {
				CompatibilitySettings = new List<CompatibilitySetting> () {
					new CompatibilitySetting (){Make="BMW",Model="3 Series"}
				}
			};
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

			// act
			Controller.CreateCompatibility (cmd);

			// assert
			using (var tx = Session.BeginTransaction ()) {
				var variants = Session.Query<CompatibilitySetting> ().Select (s => s.Variant).ToList ();
				Assert.AreEqual (1, variants.Count ());
				Assert.IsNullOrEmpty (variants[0]);
			}
		}

		[Test]
		[Category ("Integration")]
		[Category ("Database")]
		[Description ("When create all settings" +
					  "And setting for all settings exist" +
					 "Then no settings should be saved")]
		public void CreateCompatibility_AllSpecified_AllExist_SavesNothing ()
		{
			// arrange
			var p = new Product () {
				CompatibilitySettings = new List<CompatibilitySetting> () {
					new CompatibilitySetting (){Make="BMW",Model="3 Series"} // all exist
				}
			};
			using (var tx = Session.BeginTransaction ()) {
				Session.Save (p);
				tx.Commit ();
			}

			var cmd = new CreateCompatibilityCommand () {
				Make = "BMW",
				Model = "3 Series",
				// all specified
				Variants = new string [] { "E21", "E30", "E36", "E91", "E90", "E46", "F30", "F30, F35, F80", "E92", "E93", "F30, F35", "F31" },
				ProductId = p.Id
			};

			// act
			Controller.CreateCompatibility (cmd);

			// assert
			using (var tx = Session.BeginTransaction ()) {
				var variants = Session.Query<CompatibilitySetting> ().Select (s => s.Variant).ToList ();
				Assert.AreEqual (1, variants.Count ());
				Assert.IsNullOrEmpty (variants [0]);
			}
		}

		[Test]
		[Category ("Integration")]
		[Category ("Database")]
		[Description ("When create all settings" +
					  "And some settings exist" +
					 "Then only generic setting should be left")]
		public void CreateCompatibility_AllSpecified_SomeExist_SavesGenericRecord ()
		{
			// arrange
			var p = new Product () {};
			var s1 = new CompatibilitySetting () { Make = "BMW", Model = "3 Series", Variant = "E21", Product=p };
			var s2 = new CompatibilitySetting () { Make = "BMW", Model = "3 Series", Variant = "E30", Product=p };
			p.CompatibilitySettings.Add (s1);
			p.CompatibilitySettings.Add (s2);
			using (var tx = Session.BeginTransaction ()) {
				Session.Save (p);
				tx.Commit ();
			}

			var cmd = new CreateCompatibilityCommand () {
				Make = "BMW",
				Model = "3 Series",
				// all specified
				Variants = new string [] { "E21", "E30", "E36", "E91", "E90", "E46", "F30", "F30, F35, F80", "E92", "E93", "F30, F35", "F31" },
				ProductId = p.Id
			};

			// act
			Controller.CreateCompatibility (cmd);

			// assert
			using (var tx = Session.BeginTransaction ()) {
				var variants = Session.Query<CompatibilitySetting> ().Select (s => s.Variant).ToList ();
				Assert.AreEqual (1, variants.Count ());
				Assert.IsNullOrEmpty (variants [0]);
			}
		}

		// test delete setting
		[Test]
		[Category ("Integration")]
		public void DeleteCompatibility ()
		{
			Guid settingId;
			var s = new CompatibilitySetting () { Make = "BMW", Model = "3 Series" };
			var p = new Product () {};

			s.Product = p;
			p.CompatibilitySettings.Add (s);

			using (var tx = Session.BeginTransaction ()) {
				Session.Save (p);
				tx.Commit ();
			}
			settingId = Guid.Parse (p.CompatibilitySettings.FirstOrDefault ().Id.ToString ());

			Controller.DeleteSetting (settingId);

			Assert.AreEqual (0, Session.Query<CompatibilitySetting> ().Count ());
		}
	}
}