using System;
using System.Linq;
using Autofac;
using DaiDaKaram.Domain;
using DaiDaKaram.Domain.Entities;
using NHibernate;
using NHibernate.Linq;
using NUnit.Framework;

namespace Integration
{
	[TestFixture]
	public class ProductServiceTests
	{
		public ILifetimeScope Scope { get; private set; }
		public IProductService Service { get; private set; }
		public ISession Session { get; private set; }


		
		[TestFixtureSetUp]
		public void Init ()
		{
			Scope = TestUtils.GetAutofacScope ();
			Session = Scope.Resolve<ISession> ();
		}

		[SetUp]
		public void SetUp ()
		{
			using(var tx = Session.BeginTransaction ())
			{
				Session.CreateSQLQuery ("truncate Category").List ();
				Session.CreateSQLQuery ("truncate Product").List ();
				//Session.CreateSQLQuery ("truncate Category");
				tx.Commit ();
			}
			Service = Scope.Resolve<IProductService> ();
		}

		[Test]
		[Category ("Integration")]
		[Category ("Database")]
		public void ImportProduct_CreatesNewHierachy_WhenNoneExists()
		{
			string sku = "stpd-42";
			string productName = "iphone holder";
			string groupName = "phone holders";
			string typeName = "accessories";
			string oem = "123123123";
			Service.ImportProduct (typeName,groupName, productName, sku,oem);

			var product = Session.Query<Product> ().Where (arg => arg.SKU == sku).ToList ().FirstOrDefault ();
			var catGroup = Session.Query<Category> ().Where (arg => arg.Name == groupName).ToList ().FirstOrDefault ();
			var catType = Session.Query<Category> ().Where (arg => arg.Name == typeName).ToList ().FirstOrDefault ();

			Assert.IsNotNull (product);
			Assert.AreEqual (product.Category, catGroup);
			Assert.AreEqual (catGroup.Parent, catType);
		}

	}
}

