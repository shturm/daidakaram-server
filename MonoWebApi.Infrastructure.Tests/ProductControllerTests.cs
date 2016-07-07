using System;
using Autofac;
using MonoWebApi.Infrastructure.WebApi.Controllers;
using MonoWebApi.Domain;
using NUnit.Framework;
using System.Configuration;
using System.Web.Http.Hosting;
using System.Web.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using MonoWebApi.Domain.Entities;

namespace MonoWebApi.Infrastructure.Tests
{
	[TestFixture]
	public class ProductControllerTests
	{
		ProductController Controller;
		IContainer Container;
		ILifetimeScope Scope;

		[TestFixtureSetUp]
		public void Init ()
		{
			ConfigurationManager.ConnectionStrings.Add (
				new ConnectionStringSettings ("DefaultConnection", "Server=localhost;Database=koshiyam;Uid=uniuser;Pwd=unipass;")
			);

			var builder = new ContainerBuilder ();
			AutofacInfrastructureConfiguration.Configure (builder);
			AutofacDomainConfiguration.Configure (builder);
			Container = builder.Build ();
		}

		[SetUp]
		public void SetUp ()
		{
			Scope = Container.BeginLifetimeScope ();
			try {
				Controller = new ProductController (Scope.Resolve<IProductService> ());
			} catch (Exception ex) {
				Console.WriteLine (ex);
			}

			Controller.Request = new System.Net.Http.HttpRequestMessage ();
			Controller.Request.Properties.Add (HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration ());
		}

		[Test]
		[Category ("Integration")]
		public async void GivenValidProductAndImageData_CreatesProduct ()
		{
			var content = new MultipartFormDataContent ();
			content.Add (new StringContent ("product 1"), "name");
			content.Add (new ByteArrayContent (new byte [] { 1, 2 }), "thumbnail", "pic1.jpg");
			content.Add (new ByteArrayContent (new byte [] { 1, 2, 3 }), "photos", "pic1.jpg");
			content.Add (new ByteArrayContent (new byte [] { 1, 2, 3, 4 }), "photos", "pic2.jpg");
			Controller.Request.Content = content;

			var product = await Controller.Post ();

			Assert.AreNotEqual (0, product.Id, "Product ID has been generated");
			Assert.IsNotNull (product.Thumbnail, "Product has thumbnail");
			Assert.AreEqual (2, product.Photos.Count, "Product has photos");
		}
	}
}