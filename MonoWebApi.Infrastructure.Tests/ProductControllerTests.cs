using System;
using Autofac;
using MonoWebApi.Infrastructure.WebApi.Controllers;
using MonoWebApi.Infrastructure.DataAccess;
using MonoWebApi.Domain;
using NUnit.Framework;
using System.Configuration;
using System.Web.Http.Hosting;
using System.Web.Http;
using System.Net.Http;
using Newtonsoft.Json;
using NHibernate;
using MonoWebApi.Domain.Entities;
using System.Collections.Generic;
using NHibernate.Linq;
using System.Linq;

namespace MonoWebApi.Infrastructure.Tests
{
	[TestFixture]
	public class ProductControllerTests
	{
		ProductController Controller;
		IContainer Container;
		ILifetimeScope Scope;

		ISession session;

		[TestFixtureSetUp]
		public void Init ()
		{
			ConfigurationManager.ConnectionStrings.Add (
				new ConnectionStringSettings ("DefaultConnection", "Server=localhost;Database=koshiyam;Uid=uniuser;Pwd=unipass;")
			);
			session = NHibernateConfiguration.OpenSession ();

			var builder = new ContainerBuilder ();
			AutofacInfrastructureConfiguration.Configure (builder);
			AutofacDomainConfiguration.Configure (builder);
			Container = builder.Build ();
		}

		[SetUp]
		public void SetUp ()
		{
			TruncateProductsAndImages ();

			Scope = Container.BeginLifetimeScope ();
			try {
				Controller = new ProductController (Scope.Resolve<IProductService> ());
			} catch (Exception ex) {
				Console.WriteLine (ex);
			}
			session = NHibernateConfiguration.OpenSession ();
			Controller.Request = new System.Net.Http.HttpRequestMessage ();
			Controller.Request.Properties.Add (HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration ());
		}

		[TearDown]
		public void TearDown ()
		{
			TruncateProductsAndImages ();
		}

		void TruncateProductsAndImages ()
		{
			using (var tx = session.BeginTransaction ()) {
				session.CreateSQLQuery ("SET FOREIGN_KEY_CHECKS = 0").List ();
				session.CreateSQLQuery ("truncate Image").List ();
				session.CreateSQLQuery ("truncate Product").List ();
				session.CreateSQLQuery ("SET FOREIGN_KEY_CHECKS = 1").List ();
				tx.Commit ();
			}
		}

		[Test]
		[Category ("Integration")]
		public async void CreateProduct ()
		{
			var content = new MultipartFormDataContent ();
			content.Add (new StringContent ("product 1"), "name");
			content.Add (new ByteArrayContent (new byte [] { 1, 2 }), "thumbnail", "pic1.jpg");
			content.Add (new ByteArrayContent (new byte [] { 1, 2, 3 }), "photos", "pic1.jpg");
			content.Add (new ByteArrayContent (new byte [] { 1, 2, 3, 4 }), "photos", "pic2.jpg");
			Controller.Request.Content = content;

			var product = await Controller.CreateProduct ();

			Assert.AreNotEqual (0, product.Id, "Product ID has been generated");
			Assert.IsNotNull (product.Thumbnail, "Product has thumbnail");
			Assert.AreEqual (2, product.Photos.Count, "Product has photos");
		}

		[Test]
		[Category ("Integration")]
		public async void AddImageToProduct ()
		{
			// ensure it can also set it as the new thumbnail
			var product = new Product () { Name = "Add image to product test" };
			var tx = session.BeginTransaction ();
			session.Save (product);
			tx.Commit ();
			tx.Dispose ();

			var content = new MultipartFormDataContent ();
			content.Add (new ByteArrayContent (new byte [] { 1, 2, 3, 4 }), "form-field-name-does-not-matter", "anyimg.jpg");
			Controller.Request.Content = content;

			var productImageIds = await Controller.AddImagesToProduct (product.Id);

			tx = session.BeginTransaction ();
			var resultProduct = session.QueryOver<Product> ()
									   .Fetch (p => p.Photos).Eager.List ()
									   .Single ();

			//var resultProductImages = session.Query<Image> ()
			//								 .Where (i => i.PhotoOfProduct.Id == resultProduct.Id)
			//                                 .ToList ().Select (i => i.Id);

			//Assert.True (productImageIds.SequenceEqual (resultProductImages));
		}

		[Test]
		[Category ("Integration")]
		public void DeleteImage ()
		{
			var image = new Image ();
			int imageId = 0;
			var tx = session.BeginTransaction ();
			session.Save (image);
			tx.Commit ();
			imageId = image.Id;

			Controller.DeleteImage (imageId);

			tx = session.BeginTransaction ();
			var result = session.Query<Image> ().Where (img => img.Id == imageId).ToList ();
			tx.Dispose ();
			Assert.AreEqual (0, result.Count);
		}

		[Test]
		[Category ("Integration")]
		public void ChangeThumbnail ()
		{
			var firstImage = new Image ();
			var secndImage = new Image ();
			var initialProduct = new Product () {
				Photos = new List<Image> {
					firstImage,
					secndImage
				}
			};

			using (var tx = session.BeginTransaction ()) {
				session.Save (initialProduct);
				tx.Commit ();
			}
			Assert.IsNotNull (firstImage.Id);
			Assert.Greater (firstImage.Id, 0);

			Controller.ChangeThumbnail (initialProduct.Id, 1);

			session = NHibernateConfiguration.OpenSession ();
			using (var tx2 = session.BeginTransaction ())
			{
				var queriedProduct = session.Query<Product> ()
				                            .FirstOrDefault ();
				Assert.AreEqual (3, session.Query<Image> ().ToList ().Count);
				Assert.IsNotNull (queriedProduct.Thumbnail, "No thumbnail set");
				Assert.AreNotEqual (secndImage.Id, queriedProduct.Thumbnail.Id);
				Assert.AreNotEqual (firstImage.Id, queriedProduct.Thumbnail.Id);
			}
		}

		[Test]
		[Category ("Integration")]
		public void UpdateProductDetails ()
		{
			var p = new Product () {
				Name = "name1",
				Description = "desc1"
			};
			using(var tx = session.BeginTransaction ())
			{
				session.Save (p);
				tx.Commit ();
			}
			p.Name = "Updated name";

			Controller.UpdateProduct (p);

			session = NHibernateConfiguration.OpenSession ();
			using (var tx = session.BeginTransaction ())
			{
				var pr = session.Query<Product> ().FirstOrDefault ();
				Assert.AreEqual ("Updated name", pr.Name);
			}
		}
	}
}