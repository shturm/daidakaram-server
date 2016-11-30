using System;
using System.Linq;
using System.Web.Http;
using DaiDaKaram.Infrastructure.WebApi.Controllers;
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
			Assert.AreEqual (185, actual.Count());
		}

		[Test]
		[Category ("Integration")]
		public void GetModels()
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
		public void GetYearFrom()
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
			Assert.AreEqual (76, actual.Count());
		}
	}
}