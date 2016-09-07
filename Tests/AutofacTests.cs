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
				Assert.AreEqual (typeof (ProductRepository), scope.Resolve<IRepository<Product>> ().GetType ());
				Assert.AreEqual (typeof (ProductService), scope.Resolve<IProductService> ().GetType ());
				Assert.AreEqual (typeof (ImageManipulator), scope.Resolve<IImageManipulator> ().GetType ());
			}

		}
	}
}

