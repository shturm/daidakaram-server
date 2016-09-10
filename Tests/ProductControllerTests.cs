using System;
using Autofac;
using Autofac.Integration.WebApi;
using MonoWebApi.Infrastructure.WebApi.Controllers;
using MonoWebApi.Domain;
using NUnit.Framework;
using System.Configuration;
using System.Web.Http.Hosting;
using System.Web.Http;
using System.Net.Http;
using Newtonsoft.Json;
using MonoWebApi.Domain.Entities;
using System.Collections.Generic;

using System.Linq;
using System.Reflection;
using NHibernate;
using NHibernate.Linq;

namespace Integration
{
	[TestFixture]
	public class ProductControllerTests : ApiControllerTests<ProductController>
	{

		ISession Session;

		[TestFixtureSetUp]
		public override void Init ()
		{
			base.Init ();
			Session = Scope.Resolve<ISession> ();
		}

		[SetUp]
		public override void SetUp ()
		{
			base.SetUp ();

			using (var tx = Session.BeginTransaction ()) {
				Session.CreateSQLQuery ("truncate Product").List ();
				Session.CreateSQLQuery ("truncate Image").List ();
				tx.Commit ();
			}
			//Session.Flush ();
		}

		[TearDown]
		public void TearDown ()
		{
			//Session.Dispose ();
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
			var content = new MultipartFormDataContent ();
			content.Add (new ByteArrayContent (new byte [] { 1, 2, 3 }), "form-field-name-does-not-matter", "anyimg.jpg");
			content.Add (new ByteArrayContent (new byte [] { 1, 2, 3, 4 }), "form-field-name-does-not-matter2", "anyimg2.jpg");
			Controller.Request.Content = content;
			var product = new Product () { Name = "add image to product test product" };
			using (var tx = Session.BeginTransaction ()) {
				Session.Save (product);
				tx.Commit ();
			}
			//Session.Lock (product, LockMode.Upgrade);

			var productImageIds = await Controller.AddImagesToProduct (product.Id);
			//Session.Evict (product); // either this or get new session, otherwise serves cache and without Photos
			Session.Flush ();

			Product queryProduct = null;
			using(var tx = Session.BeginTransaction ())
			{
				//queryProduct = Session.Load<Product> (1);
				//queryProduct = Session.Get<Product> (1);
				queryProduct = Session.Query<Product> ().FirstOrDefault ();
				tx.Commit ();
			}

			Assert.IsNotNull (queryProduct.Photos, "Could not get Photos list");
			var actualImageIds = queryProduct.Photos.Select (i => i.Id).ToList ();

			Assert.True (productImageIds.SequenceEqual (actualImageIds));
		}

		[Test]
		[Category ("Integration")]
		public void DeleteImage ()
		{
			var photo = new Photo ();
			int imageId = 0;
			using(var tx = Session.BeginTransaction ())
			{
				Session.Save (photo);
				tx.Commit ();
			}
			imageId = photo.Id;

			Controller.DeleteImage (imageId);

			var result = Session.Query<Photo> ().ToList ();
			Assert.AreEqual (0, result.Count);
		}

		[Test]
		[Category ("Integration")]
		public void ChangeThumbnail ()
		{
			var firstImage = new Photo ();
			var secndImage = new Photo ();
			var initialProduct = new Product () {
				Photos = new List<Photo> {
					firstImage,
					secndImage
				}
			};
			using(var tx = Session.BeginTransaction ())
			{
				Session.Save (initialProduct);
				tx.Commit ();
			}
			Session.Evict (initialProduct);

			Controller.ChangeThumbnail (initialProduct.Id, 1);


			var queriedProduct = Session.Get<Product> (1);

			int totalImagesCount = Session.Query<Image> ().ToList ().Count;
			Assert.AreEqual (3, totalImagesCount, "Total images not as much as expected");


			Assert.IsNotNull (queriedProduct.Thumbnail, "No thumbnail set");
		}

		[Test]
		[Category ("Integration")]
		public void UpdateProductDetails ()
		{
			var initialProduct = new Product () {
				Name = "old name",
				Description = "desc1"
			};
			//  REFACTOR persist product `p`
			using(var tx = Session.BeginTransaction ())
			{
				Session.Save (initialProduct);
				tx.Commit ();
			}

			initialProduct.Name = "new name";
			Controller.UpdateProduct (initialProduct);

			var pr = Session.Query<Product> ().Where (prod=>prod.Id ==initialProduct.Id).FirstOrDefault ();
			Assert.AreEqual ("new name", pr.Name);
		}
	}
}