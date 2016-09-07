using System;
using Autofac;
using Autofac.Integration.WebApi;
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
using NHibernate.Linq;
using MonoWebApi.Domain.Entities;
using System.Collections.Generic;

using System.Linq;
using System.Reflection;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Criterion;

namespace MonoWebApi.Infrastructure.WebApi.Tests
{
	[TestFixture]
	public class ProductControllerTests : ApiControllerTests<ProductController>
	{
		[TestFixtureSetUp]
		public override void Init ()
		{
			base.Init ();
			using (var tx = Session.BeginTransaction ()) {
				Session.CreateSQLQuery ("SET FOREIGN_KEY_CHECKS = 0").List ();
				tx.Commit ();
			}
		}

		[TestFixtureTearDown]
		public override void ShutDown ()
		{
			using (var tx = Session.BeginTransaction ()) {
				Session.CreateSQLQuery ("SET FOREIGN_KEY_CHECKS = 1").List ();
				tx.Commit ();
			}
		}

		[SetUp]
		public override void SetUp ()
		{
			base.SetUp ();
			using (var tx = Session.BeginTransaction ()) {
				Session.CreateSQLQuery ("truncate Image").List ();
				Session.CreateSQLQuery ("truncate Product").List ();
				tx.Commit ();
			}
		}

		[TearDown]
		public void TearDown ()
		{
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
			var product = new Product () { Name = "Add image to product test" };
			var content = new MultipartFormDataContent ();
			content.Add (new ByteArrayContent (new byte [] { 1, 2, 3, 4 }), "form-field-name-does-not-matter", "anyimg.jpg");
			Controller.Request.Content = content;
			using(var tx = Session.BeginTransaction ())
			{
				Session.Save (product);
				tx.Commit ();
			}

			var productImageIds = await Controller.AddImagesToProduct (product.Id);

			using(var tx2 = Session.BeginTransaction ())
			{
				var resultProduct = (Product)Session.CreateCriteria<Product> ()
										   .SetFetchMode ("Photos", FetchMode.Eager)
										   //.Fetch (p => p.Photos).Eager
										   //.Select (pr => new { pr.Photos})
										   .List () [0];

				//var resultProduct = Session.QueryOver<Product> ()
				//						   .Fetch(p => p.Photos).Eager
				//                           //.Select (pr => new { pr.Photos})
				//                           .List ()
				//                           .FirstOrDefault ();

				//var resultProduct = Session.Query<Product> ()
				//						   .FetchMany(p => p.Photos)
				//						   //.Select (pr => new { pr.Photos }) // https://nhibernate.jira.com/browse/NH-3396
				//						   .ToList ()
				//						   .FirstOrDefault ();

				Assert.IsNotNull (resultProduct.Photos, "Could not get Photos list");

				var actualImageIds = resultProduct.Photos.Select (i => i.Id).ToList ();


				Assert.True (productImageIds.SequenceEqual (actualImageIds));	
			}

		}

		[Test]
		[Category ("Integration")]
		public void DeleteImage ()
		{
			var image = new Image ();
			int imageId = 0;
			var tx = Session.BeginTransaction ();
			Session.Save (image);
			tx.Commit ();
			imageId = image.Id;

			Controller.DeleteImage (imageId);

			tx = Session.BeginTransaction ();
			var result = Session.Query<Image> ().Where (img => img.Id == imageId).ToList ();
			tx.Dispose ();
			Assert.AreEqual (0, result.Count);
		}

		[Test]
		//[Ignore]
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

			using (var tx = Session.BeginTransaction ()) {
				Session.Save (initialProduct);
				tx.Commit ();
			}

			Controller.ChangeThumbnail (initialProduct.Id, 1);

			using (var tx2 = Session.BeginTransaction ())
			{
				//var queriedProduct = Session.Query<Product> ()
				//		.ToList ()
				//                    .FirstOrDefault ();

				//var queriedProduct = Session.Query<Product> ()
				//.Fetch (p => p.Thumbnail).ToFuture ()
				//.ToList ().FirstOrDefault ();

				//var queriedProduct = Session.Query<Product> ()
				//	.Fetch (p => p.Thumbnail)
				//	.ToList ().FirstOrDefault ();

				var queriedProduct = Session.Query<Product> ()
				                            .Fetch (p => p.Thumbnail)
				                            .Select (pr => new { Thumbnail = pr.Thumbnail})
				                            .ToList ().FirstOrDefault ();
				
				//var queriedProduct = Session.CreateCriteria<Product> ()
				//.List<Product> ().FirstOrDefault ();
				Assert.AreEqual (3, Session.Query<Image> ().ToList ().Count, "Total images not as much as expected");


				Assert.IsNotNull (queriedProduct.Thumbnail, "No thumbnail set");
				//Assert.AreNotEqual (secndImage.Id, queriedProduct.Thumbnail.Id);
				//Assert.AreNotEqual (firstImage.Id, queriedProduct.Thumbnail.Id);
				tx2.Commit ();
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
			using(var tx = Session.BeginTransaction ())
			{
				Session.Save (p);
				tx.Commit ();
			}
			p.Name = "Updated name";

			Controller.UpdateProduct (p);

			using (var tx = Session.BeginTransaction ())
			{
				var pr = Session.Query<Product> ().FirstOrDefault ();
				Assert.AreEqual ("Updated name", pr.Name);
			}
		}
	}
}