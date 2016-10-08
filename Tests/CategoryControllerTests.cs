using System;
using System.Linq;
using Newtonsoft.Json;
using Autofac;
using Autofac.Integration.WebApi;
using DaiDaKaram.Infrastructure.WebApi.Controllers;
using DaiDaKaram.Domain;
using NUnit.Framework;
using System.Configuration;
using System.Web.Http.Hosting;
using System.Web.Http;
using System.Net.Http;
using Newtonsoft.Json;
using DaiDaKaram.Domain.Entities;
using System.Collections.Generic;

using System.Linq;
using System.Reflection;
using NHibernate;
using NHibernate.Linq;
using System.Collections;

namespace Integration
{
	[TestFixture]
	public class CategoryControllerTests : ApiControllerTests<CategoryController>
	{
		[Test]
		[Category ("Integration")]
		public void GetRootCategories ()
		{
			var c1 = new Category () {
				Name = "Root 1",
				SubCategories = new List<Category> () {
						new Category () {Name="child 1", SubCategories = new List<Category>() {
								new Category (){Name="child 1.1"}
							}},
						new Category () {Name="child 2"}
					}
			};
			var c2 = new Category () { Name = "Root 2" };
			using (var tx = Session.BeginTransaction ()) {
				Session.Save (c1);
				Session.Save (c2);
				tx.Commit ();
			}

			// act
			IEnumerable<Category> actual;
			try {
				actual = Controller.GetRootCategories ();
			} catch (Exception ex) {
				throw ex;
			}

			string serialized = "";
			CollectionAssert.AreEqual (actual, new[] {c1,c2}, new CategoryComparer());
			Assert.AreEqual (0, actual.Where (c=>c.Name.Contains ("child")).Count ());
			Assert.DoesNotThrow (() => {
				serialized = JsonConvert.SerializeObject (actual);
			});
			Assert.IsTrue (serialized.Contains ("child 1.1"), "child categories are not serialzied");
		}

		[Test]
		[Category ("Integration")]
		public void UpdateWhenNewSubCategory ()
		{
			var x = new Category () { Name = "Root Category" };
			using (var tx = Session.BeginTransaction ()) {
				Session.Save (x);
				tx.Commit ();
			}

			x.SubCategories.Add (new Category () {Name="child"});
			Controller.Update (x);

			Assert.IsNotNull (x.SubCategories.First ().Id);
		}
	}

	class CategoryComparer : IComparer
	{
		public int Compare (object x, object y)
		{
			return Compare ((Category)x, (Category)y);
		}

		int Compare (Category x, Category y)
		{
			if (x.Id != 0 && y.Id != 0) {
				if (x.Id > y.Id) return 1;
				if (x.Id < y.Id) return 1;
				if (x.Id == y.Id) return 0;
			}


			if (x.Name == y.Name) {
				return 0;
			} else {
				return 1;
			}
		}
	}
}