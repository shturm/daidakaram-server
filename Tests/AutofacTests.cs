using System;
using System.Reflection;
using DaiDaKaram.Infrastructure;
using DaiDaKaram.Infrastructure.WebApi;
using DaiDaKaram.Domain;

using Autofac;
using Autofac.Integration.WebApi;
using NUnit.Framework;
using DaiDaKaram.Domain.Infrastructure;
using DaiDaKaram.Domain.Entities;
using DaiDaKaram.Infrastructure.Services;
using NHibernate;
using System.Collections.Generic;
using System.Linq;

namespace Integration
{
	[TestFixture]
	public class AutofacTests
	{
		[Test]
		public void CanResolveClasses ()
		{
			using (var scope = TestUtils.GetAutofacScope ()) {
				// infrastructure
				//Assert.IsInstanceOf (typeof (IRepository<Product>), scope.Resolve<IRepository<Product>> ());
				Assert.IsInstanceOf (typeof (Repository<Product>), scope.Resolve<IRepository<Product>> ());
				Assert.IsInstanceOf (typeof (ProductService), scope.Resolve<IProductService> ());
				Assert.IsInstanceOf (typeof (ImageManipulator), scope.Resolve<IImageManipulator> ());
				Assert.IsInstanceOf (typeof (MySQLDatabase), scope.Resolve<MySQLDatabase> ());
				Assert.IsInstanceOf (typeof (ISession), scope.Resolve<ISession> ());
				Assert.IsInstanceOf (typeof (CategoryService), scope.Resolve<ICategoryService> ());
			}
		}

		[Test]
		[Category ("Unit")]
		public void LinqJoins ()
		{
			var products = new List<Product> () {
				new Product() {Id = 1},
				new Product() {Id = 2},
				new Product() {Id = 3},
			};

			var subscriptions = new List<Subscription> () {
				new Subscription() {Id = 1, ProductId = 1},
				new Subscription() {Id = 2, ProductId = 3},
			};

			var productsWithSubscriptions =
				from p in products
				join s in subscriptions on p.Id equals s.ProductId
                into g
                from s2 in g.DefaultIfEmpty ()
				select new { p, s2 };

			Assert.AreEqual (3, productsWithSubscriptions.Count ());



		}

		class Subscription
		{
			public int Id { get; set; }
			public int ProductId { get; set; }
		}
	}
}