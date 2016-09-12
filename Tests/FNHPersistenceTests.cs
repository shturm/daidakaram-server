using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using FluentNHibernate.Testing;
using DaiDaKaram.Domain.Entities;
using NHibernate;
using NUnit.Framework;

namespace Integration
{
	[TestFixture]
	public class PersistenceTests
	{
		ISession Session;
		ILifetimeScope Scope;

		[TestFixtureSetUp]
		public void Init ()
		{
			try {
				Scope = TestUtils.GetAutofacScope ();
				Session = Scope.Resolve<ISession> ();
			} catch (Exception ex) {
				Console.WriteLine (ex.InnerException.Message);
				throw (ex);
			}
		}

		[SetUp]
		public void SetUp ()
		{
			var tx = Session.BeginTransaction ();
			Session.CreateSQLQuery ("truncate Product").List ();
			Session.CreateSQLQuery ("truncate Image").List ();
			Session.CreateSQLQuery ("truncate Category").List ();
			tx.Commit ();
			tx.Dispose ();
			Session.Clear ();
		}

		[Test]
		[Category ("Database")]
		public void ProductPersistence_FNH ()
		{
			var photos = new List<Photo> () {
				new Photo(){ Bytes = new byte[] {1}},
				new Photo(){ Bytes = new byte[] {1,2}}
			};

			var category = new Category () { Name = "fnh spec product category name" };
			using (var tx = Session.BeginTransaction ()) {
				Session.Save (category);
				tx.Commit ();
			}

			new PersistenceSpecification<Product> (Session, new DDKComparer ())
				.CheckProperty (p => p.Name, "fnh spec product name")
				.CheckList (p => p.Photos, photos)
				.CheckProperty (p => p.Category, category)
				.CheckProperty (p => p.Thumbnail, new Thumbnail () { Bytes = new byte [] { 1, 2, 3 } })
				.VerifyTheMappings ();
		}

		[Test]
		[Category ("Database")]
		public void CategoryPersistence_FNH ()
		{
			var parent = new Category () { Name = "fnh category test parent cat 1" };
			//using(var tx = Session.BeginTransaction ())
			//{
			//	Session.Save (parent);
			//	tx.Commit ();
			//}

			var subCategories = new List<Category> () {
				new Category() {Name="Child category 1"},
				new Category() {Name="Child category 2"},
			};

			var products = new List<Product> () {
				new Product() {Name="Product 1"},
				new Product() {Name="Product 2"},
			};

			new PersistenceSpecification<Category> (Session, new DDKComparer ())
				.CheckProperty (c => c.Name, "fnh category name")
				.CheckReference (c => c.Parent, parent)
				.CheckList (c => c.Products, products)
				.CheckList (c => c.SubCategories, subCategories);
		}

		[Test]
		[Category ("Database")]
		public void CategoryPersistence_Session ()
		{
			var category = new Category () { Name = "fnh category test parent cat 1" };

			var subCategories = new List<Category> () {
				new Category() {Name="Child category 1", Parent=category},
				new Category() {Name="Child category 2", Parent=category},
			};

			var products = new List<Product> () {
				new Product() {Name="Product 1", Category=category},
				new Product() {Name="Product 2", Category=category},
			};

			category.SubCategories = subCategories;
			category.Products = products;

			using (var tx = Session.BeginTransaction ()) {
				Session.Save (category);
				tx.Commit ();
			}

			Assert.AreNotEqual (0, category.Id);
			Assert.AreNotEqual (0, subCategories [0].Id);
			Assert.AreNotEqual (0, subCategories [1].Id);
			Assert.AreNotEqual (0, products [0].Id);
			Assert.AreNotEqual (0, products [1].Id);
		}

		[Test]
		[Category ("Database")]
		public void ProductPersistence_Session ()
		{
			using (var tx = Session.BeginTransaction ()) {
				var preparedProduct = new Product () {
					Photos = new List<Photo> () {
						new Photo(){ Bytes = new byte[] {1}},
						new Photo(){ Bytes = new byte[] {1,2}}
					},
					Thumbnail = new Thumbnail () { Bytes = new byte [] { 1, 2, 3 } }
				};
				Session.Save (preparedProduct);
				tx.Commit ();
			}

			using (var tx = Session.BeginTransaction ()) {
				var actualProduct = Session.QueryOver<Product> ().List ().FirstOrDefault ();
				Assert.AreEqual (2, actualProduct.Photos.Count ());
			}
		}
	}

	class DDKComparer : IEqualityComparer
	{
		// x value is the one specified in the test
		bool IEqualityComparer.Equals (object x, object y)
		{
			if (x is DateTime && y is DateTime) {
				return CompareAsDates (x, y);
			}

			//if (x is BaseEntity && y is BaseEntity)
			//{
			//	x = x as BaseEntity;
			//	y = y as BaseEntity;
			//	if (((BaseEntity)x).Id != 0 && (((BaseEntity)y).Id != 0))
			//	{
			//		return (((BaseEntity)x).Id.Equals ((((BaseEntity)y).Id)));
			//	}
			//}

			if (x is Image)
				return CompareAsImages ((Image)x, (Image)y);

			if (x is Category)
				return CompareAsCategories ((Category)x, (Category)y);

			if (x is IList<Image>)
				return CompareAsImageLists ((IEnumerable<Image>)x, (IEnumerable<Image>)y);

			return x.Equals (y);
		}

		bool CompareAsCategories (Category x, Category y)
		{
			if (x == null) return false;
			if (y == null) return false;
			if (x.Id > 0 && y.Id > 0)
				return x.Id == y.Id;

			return x.Name.Equals (y.Name);
		}

		bool CompareAsImages (Image x, Image y)
		{
			if (x == null) return false;
			if (y == null) return false;
			if (x.Id > 0 && y.Id > 0)
				return x.Id == y.Id;

			return x.Bytes.SequenceEqual (y.Bytes);
		}

		bool CompareAsImageLists (IEnumerable<Image> x, IEnumerable<Image> y)
		{
			return x.SequenceEqual (y, new ImageComparer ());
		}

		bool CompareAsDates (object x, object y)
		{
			DateTime xDate = (DateTime)x;
			DateTime yDate = (DateTime)y;
			return xDate.ToString () == yDate.ToString ();
		}

		public int GetHashCode (object obj)
		{
			throw new NotImplementedException ();
		}
	}

	class ImageComparer : IEqualityComparer<Image>
	{
		public bool Equals (Image x, Image y)
		{
			return x.Bytes.SequenceEqual (y.Bytes);
		}

		public int GetHashCode (Image obj)
		{
			throw new NotImplementedException ();
		}
	}
}

