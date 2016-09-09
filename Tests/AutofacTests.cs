using System;
using System.Reflection;
using MonoWebApi.Infrastructure;
using MonoWebApi.Infrastructure.WebApi;
using MonoWebApi.Domain;

using Autofac;
using Autofac.Integration.WebApi;
using NUnit.Framework;
using MonoWebApi.Domain.Infrastructure;
using MonoWebApi.Domain.Entities;
using MonoWebApi.Infrastructure.Services;

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
				Assert.IsInstanceOf (typeof (IRepository<Product>), scope.Resolve<IRepository<Product>> ());
				Assert.IsInstanceOf (typeof (Repository<Product>), scope.Resolve<IRepository<Product>> ());
				Assert.IsInstanceOf (typeof (ProductService), scope.Resolve<IProductService> ());
				Assert.IsInstanceOf (typeof (ImageManipulator), scope.Resolve<IImageManipulator> ());
				Assert.IsInstanceOf (typeof (MySQLDatabase), scope.Resolve<MySQLDatabase> ());
			}

		}
	}
}