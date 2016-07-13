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
			_session = NHibernateConfiguration.OpenSession ();
		}

		[Test]
		[Category ("Database")]
		public void Product_PersistenceSpecification ()
		{
			var photos = new List<Image> () {
				new Image(){ Bytes = new byte[] {1}},
				new Image(){ Bytes = new byte[] {1,2}}
			};
			var thumbnail = new Image () { Bytes = new byte [] { 1, 2, 3 } };

			new PersistenceSpecification<Product> (_session, new DDKComparer ())
				.CheckProperty (x => x.Name, "Product Name 1")
				.CheckProperty (x => x.Description, "Product Descritpion 1")
				.CheckProperty (x => x.Photos, photos)
				.CheckReference (x => x.Thumbnail, thumbnail) // thumbs have separate persistence test
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
				//tx.Commit ();
				var query = from p in _session.Query<Product> ()
							select p;
				var result = query.ToList ();

				Assert.IsNotNull (result);
				Assert.IsNotNull (result [0].Thumbnail);
				Assert.IsNotNull (result [1].Thumbnail);

				result [0].Photos = new List<Image> () { thumb1, thumb2 };
				_session.SaveOrUpdate (result [0]);

				var result2 = _session.Query<Product> ().ToList ();

				Assert.IsNotNull (result [0].Thumbnail);
				Assert.IsNotNull (result [1].Thumbnail);

				Assert.IsNotNull (result2 [0].Thumbnail);
				Assert.IsNotNull (result2 [1].Thumbnail);
			}
		}

		[Test]
		[Category ("Database")]
		public void ProductRelatedEntities ()
		{
			using (var tx = _session.BeginTransaction ()) {
				var product = new Product ();
				_session.SaveOrUpdate (product);
				var image = new Image ();
				var photos = new List<Image> () { new Image ()};

				product.Thumbnail = image;
				product.Photos = photos;
				_session.SaveOrUpdate (product);
				tx.Commit ();
			}

			using(var tx = _session.BeginTransaction ())
			{
				var p = _session.Query<Product> ().SingleOrDefault ();
				Assert.IsNotNull (p.Thumbnail);
				Assert.IsNotNull (p.Photos);
				Assert.AreEqual (1, p.Photos.Count ());
			}
		}

		[Test]
		[Category ("Database")]
		public void Image_PersistenceSpecification ()
		{
			new PersistenceSpecification<Image> (_session, new DDKComparer ())
				.CheckProperty (x => x.Bytes, new byte [] { 1, 2, 3 })
				.CheckProperty (x => x.Created, DateTime.UtcNow)
				.CheckProperty (x => x.Updated, DateTime.UtcNow)
				.VerifyTheMappings ();
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

			if (x is Image)
				return CompareAsImages ((Image)x, (Image)y);

			if (x is IList<Image>)
				return CompareAsImageLists ((IEnumerable<Image>)x, (IEnumerable<Image>)y);

			return x.Equals (y);
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