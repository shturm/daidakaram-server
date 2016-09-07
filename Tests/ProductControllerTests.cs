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

namespace Integration
{
	[TestFixture]
	public class ProductControllerTests : ApiControllerTests<ProductController>
	{
		[TestFixtureSetUp]
		public override void Init ()
		{

		}

		[TestFixtureTearDown]
		public override void ShutDown ()
		{

		}

		[SetUp]
		public override void SetUp ()
		{
			base.SetUp ();
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

			// REFACTOR persist `product`

			var productImageIds = await Controller.AddImagesToProduct (product.Id);



			// REFACTOR load `resultProduct` from db
			Product resultProduct = null;

			Assert.IsNotNull (resultProduct.Photos, "Could not get Photos list");

			var actualImageIds = resultProduct.Photos.Select (i => i.Id).ToList ();


			Assert.True (productImageIds.SequenceEqual (actualImageIds));


		}

		[Test]
		[Category ("Integration")]
		public void DeleteImage ()
		{
			var image = new Image ();
			int imageId = 0;
			// REFACTOR save `image`
			imageId = image.Id;

			Controller.DeleteImage (imageId);

			// REFACTOR load all images in `result`
			var result = new List<int> ();
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

			// REFACTOR persist `initialProduct`

			Controller.ChangeThumbnail (initialProduct.Id, 1);



			// REFACTOR load one (the only) product in `queriedProduct`
			var queriedProduct = new Product();

			// REFACTOR get total image count from db
			int totalImagesCount = 42;
			Assert.AreEqual (3, totalImagesCount, "Total images not as much as expected");


			Assert.IsNotNull (queriedProduct.Thumbnail, "No thumbnail set");

		}

		[Test]
		[Category ("Integration")]
		public void UpdateProductDetails ()
		{
			var p = new Product () {
				Name = "name1",
				Description = "desc1"
			};
			//  REFACTOR persist product `p`
			p.Name = "Updated name";

			Controller.UpdateProduct (p);

			// REFACTOR load product `pr`
			var pr = new Product ();
			Assert.AreEqual ("Updated name", pr.Name);
		}
	}
}