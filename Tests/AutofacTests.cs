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

namespace Integration
{
	[TestFixture]
	public class AutofacTests
	{
		[Test]
		public void CanResolveClasses ()
		{
			using (var scope = TestUtils.GetAutofacScope ())
			{
				// infrastructure
				//Assert.IsInstanceOf (typeof (IRepository<Product>), scope.Resolve<IRepository<Product>> ());
				Assert.IsInstanceOf (typeof (Repository<Product>), scope.Resolve<IRepository<Product>> ());
				Assert.IsInstanceOf (typeof (ProductService), scope.Resolve<IProductService> ());
				Assert.IsInstanceOf (typeof (ImageManipulator), scope.Resolve<IImageManipulator> ());
				Assert.IsInstanceOf (typeof (MySQLDatabase), scope.Resolve<MySQLDatabase> ());
				Assert.IsInstanceOf (typeof (ISession), scope.Resolve<ISession> ());
			}
		}
	}
}