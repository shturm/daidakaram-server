using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;

using MonoWebApi.Domain.Entities;
using MonoWebApi.Infrastructure.DataAccess;

using FluentNHibernate.Testing;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Collection.Generic;
using NUnit.Framework;
using System.Collections;
using NHibernate.Tool.hbm2ddl;

namespace MonoWebApi.Infrastructure.Tests
{
	[TestFixture]
	public class FNHConfigurationTests
	{
		ISession _session;
		SchemaExport _schema;

		[TestFixtureSetUp]
		public void FixtureSetUp ()
		{
			//ConfigurationManager.ConnectionStrings.Add ();
			foreach (ConnectionStringSettings str in ConfigurationManager.ConnectionStrings) {
				Console.WriteLine ("{0}: {1}", str.Name, str.ConnectionString);
			}
			ConfigurationManager.ConnectionStrings.Add (
				new ConnectionStringSettings (
					"DefaultConnection",
					"Server=localhost;Database=koshiyam;Uid=uniuser;Pwd=unipass;"
				)
			);
			_session = NHibernateConfiguration.OpenSession ();
			_schema = new SchemaExport (NHibernateConfiguration.NHConfiguration);
		}

		[SetUp]
		public void SetUp ()
		{
			_schema.Drop (true, true);
			_schema.Create (true, true);
		}

		[Test]
		[Category ("Database")]
		public void Ingredient_PersistenceSpecification ()
		{
			var recipes = new List<Recipe> () {
				new Recipe() {Name="Recipe 1"},
				new Recipe() {Name="Recipe 2"}
			};
			new PersistenceSpecification<Ingredient> (_session, new MonoWebApiComparer ())
				.CheckProperty (x => x.Name, "Ingredient Test 1")
				.CheckProperty (x => x.Created, DateTime.UtcNow)
				.CheckProperty (x => x.Updated, DateTime.UtcNow)
				.CheckProperty (x => x.Recipes, recipes)
				.VerifyTheMappings ();
		}

		[Test]
		[Category ("Database")]
		public void Recipe_PersistenceSpecification ()
		{
			var ingredients = new List<Ingredient> () {
				new Ingredient () { Name = "Ingredient 1" },
				new Ingredient () { Name = "Ingredient 2" },
			};
			new PersistenceSpecification<Recipe> (_session, new MonoWebApiComparer ())
				.CheckProperty (x => x.Name, "Recipe Test 1")
				.CheckProperty (x => x.Created, DateTime.UtcNow)
				.CheckProperty (x => x.Updated, DateTime.UtcNow)
				.CheckProperty (x => x.Ingredients, ingredients)
				.VerifyTheMappings ();
		}

		[Test]
		[Category ("Database")]
		public void Product_PersistenceSpecification ()
		{
			var photos = new List<Image> () {
				new Image(){ Bytes = new byte[] {2}},
				new Image() {Bytes = new byte[] {3}}
			};
			new PersistenceSpecification<Product> (_session, new MonoWebApiComparer ())
				.CheckProperty (x => x.Name, "Product Name 1")
				.CheckProperty (x => x.Description, "Product Descritpion 1")
				.CheckProperty (x => x.Photos, photos)
				//.CheckProperty (x => x.Thumbnail, thumbnail) // thumbs have separate persistence test
				.CheckProperty (x => x.Created, DateTime.UtcNow)
				.CheckProperty (x => x.Updated, DateTime.UtcNow)
				.VerifyTheMappings ();
		}

		[Test]
		[Category ("Database")]
		public void ProductAndThumbnail_PersistenceSpecification_Custom ()
		{
			var thumb1 = new Image () { Bytes = new byte [] { 41 } };
			var thumb2 = new Image () { Bytes = new byte [] { 42, 43 } };
			var prod1 = new Product ();
			var prod2 = new Product ();
			prod1.Thumbnail = thumb1;
			prod2.Thumbnail = thumb2;

			using (var tx = _session.BeginTransaction ()) {
				_session.SaveOrUpdate (thumb1);
				_session.SaveOrUpdate (thumb2);
				_session.SaveOrUpdate (prod1);
				_session.SaveOrUpdate (prod2);
				tx.Commit ();
				var query = from p in _session.Query<Product> ()
							select p;
				var result = query.ToList ();

				Assert.IsNotNull (result);
				Assert.IsNotNull (result [0].Thumbnail);
				Assert.IsNotNull (result [1].Thumbnail);
			}
		}

		[Test]
		[Category ("Database")]
		public void Image_PersistenceSpecification ()
		{
			new PersistenceSpecification<Image> (_session, new MonoWebApiComparer ())
				.CheckProperty (x => x.Bytes, new byte [] { 1, 2, 3 })
				.CheckProperty (x => x.Created, DateTime.UtcNow)
				.CheckProperty (x => x.Updated, DateTime.UtcNow)
				.VerifyTheMappings ();
		}
	}

	class MonoWebApiComparer : IEqualityComparer
	{
		// x value is the one specified in the test
		bool IEqualityComparer.Equals (object x, object y)
		{
			if (x is DateTime && y is DateTime) {
				return CompareAsDates (x, y);
			}

			if (x is Image)
				return CompareAsImages (x, y);

			if (x is IList<Image>)
				return CompareAsImageLists (x, y);

			if (x is IList<Ingredient>) {
				return CompareAsIngredientLists (x, y);
			}

			if (x is IList<Recipe>) {
				return CompareAsRecipeLists (x, y);
			}

			return x.Equals (y);
		}

		bool CompareAsImages (object x, object y)
		{
			var xImage = (Image)x;
			var yImage = (Image)y;
			if (yImage == null) return false;
			if (xImage == null) return false;
			return xImage.Bytes.SequenceEqual (yImage.Bytes);
		}

		bool CompareAsImageLists (object x, object y)
		{
			var xList = x as List<Image>;
			var yIList = y as PersistentGenericBag<Image>;
			for (int i = 0; i < xList.Count; i++) {
				var xImage = xList [i];
				var yImage = yIList [i];
				if (!xImage.Bytes.SequenceEqual (yImage.Bytes)) {
					return false;
				}
			}
			return true;
		}

		bool CompareAsDates (object x, object y)
		{
			DateTime xDate = (DateTime)x;
			DateTime yDate = (DateTime)y;
			return xDate.ToString () == yDate.ToString ();
		}

		bool CompareAsIngredientLists (object x, object y)
		{
			var xList = x as List<Ingredient>;
			var yIList = y as PersistentGenericBag<Ingredient>;
			for (int i = 0; i < xList.Count; i++) {
				var xName = xList [i].Name;
				var yName = yIList [i].Name;
				if (xName != yName) {
					return false;
				}
			}
			return true;
		}

		bool CompareAsRecipeLists (object x, object y)
		{
			var xList = x as List<Recipe>;
			var yIList = y as PersistentGenericBag<Recipe>;
			for (int i = 0; i < xList.Count; i++) {
				var xName = xList [i].Name;
				var yName = yIList [i].Name;
				if (xName != yName) {
					return false;
				}
			}
			return true;
		}

		public int GetHashCode (object obj)
		{
			throw new NotImplementedException ();
		}
	}
}