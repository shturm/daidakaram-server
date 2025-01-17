﻿using System;
using Autofac;
using Autofac.Integration.WebApi;
using DaiDaKaram.Infrastructure.WebApi.Controllers;
using DaiDaKaram.Domain;
using NUnit.Framework;
using System.Configuration;
using System.Web.Http.Hosting;
using System.Web.Http;
using System.Net.Http;
using Newtonsoft.Json;
using DaiDaKaram.Domain.Entities;
using System.Collections.Generic;

using System.Linq;
using System.Reflection;
using NHibernate;
using NHibernate.Linq;
using DaiDaKaram.Infrastructure;

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

		[Test]
		[Category ("Integration")]
		public void GetPage ()
		{
			// arrange
			using (var tx = Session.BeginTransaction ()) {
				for (int i = 0; i < 35; i++) {
					var x = new Product () { };
					if (i % 2 != 0)
					{
						var cs1 = new CompatibilitySetting () { Make = "BMW", Model = "3 Series", Variant = "E30", Product = x };
						var cs2 = new CompatibilitySetting () { Make = "BMW", Model = "3 Series", Variant = "E46", Product = x };
						x.CompatibilitySettings.Add (cs1);
						x.CompatibilitySettings.Add (cs2);
					}

					Session.Save (x);
				}
				tx.Commit ();
			}

			// act
			var firstProducts = Controller.GetPage (1);
			var secondProducts = Controller.GetPage (2);

			// assert
			Assert.AreEqual (20, firstProducts.Count ());
			Assert.AreEqual (15, secondProducts.Count ());

			Assert.AreEqual (2, firstProducts.ElementAt (1).CompatibilitySettings.Count ());
			Assert.AreEqual ("SETTINGS", firstProducts.ElementAt (1).CompatibilityStatus);

			Assert.AreEqual (0, firstProducts.ElementAt (2).CompatibilitySettings.Count ());
			Assert.AreEqual ("UNKNOWN", firstProducts.ElementAt (2).CompatibilityStatus);
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
			using (var tx = Session.BeginTransaction ()) {
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
			using (var tx = Session.BeginTransaction ()) {
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
			var initialProduct = new Product () {
				Photos = new List<Photo> {
					new Photo (),
					new Photo()
				}
			};
			using (var tx = Session.BeginTransaction ()) {
				Session.Save (initialProduct);
				tx.Commit ();
			}
			//Session.Evict (initialProduct);

			Controller.ChangeThumbnail (initialProduct.Id, 1);

			var queriedProduct = Session.Query<Product> ().Where (prod => prod.Id == initialProduct.Id).FirstOrDefault ();
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
			using (var tx = Session.BeginTransaction ()) {
				Session.Save (initialProduct);
				tx.Commit ();
			}

			initialProduct.Name = "new name";
			var dto = new ProductDto (initialProduct);
			Controller.UpdateProduct (dto);

			var pr = Session.Query<Product> ().Where (prod => prod.Id == initialProduct.Id).FirstOrDefault ();
			Assert.AreEqual ("new name", pr.Name);
		}

		[Test]
		[Category ("Integration")]
		public void UpdateProductCategory ()
		{
			var p = new Product () { Name = "update product category" };
			var sc = new Category () { Name = "assigned category" };
			var c = new Category () {
				Name = "parent",
				SubCategories = new List<Category> () { sc }
			};
			sc.Parent = c;
			using (var tx = Session.BeginTransaction ()) {
				Session.Save (p);
				Session.Save (c);
				Session.Save (sc);
				tx.Commit ();
			}

			Session.Evict (p);
			Session.Evict (c);

			p.Category = new Category () { Name = "assigned", Id = sc.Id }; // dto

			var dto = new ProductDto (p);
			Controller.UpdateProduct (dto);

			Session.Refresh (p);
			Session.Refresh (c);

			Assert.AreEqual (c.SubCategories.First ().Id, p.Category.Id, "Product has category");
			Assert.AreEqual (c.Id, sc.Parent.Id, "Assigned category remains child of the Parent category");
			Assert.AreEqual (2, Session.Query<Category> ().ToList ().Count, "Category count is not changed");
			//Assert.AreEqual (p, c.SubCategories.First ().Products.First (), "Product is added in category");
		}
	}
}