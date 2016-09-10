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
			tx.Commit ();
			tx.Dispose ();
		}

		[Test]
		[Category ("Database")]
		public void ProductPersistence_FNH ()
		{
			var photos = new List<Photo> () {
				new Photo(){ Bytes = new byte[] {1}},
				new Photo(){ Bytes = new byte[] {1,2}}
			};

			new PersistenceSpecification<Product> (Session, new DDKComparer())
				.CheckProperty (p => p.Name, "fnh spec product name")
				.CheckList (p => p.Photos, photos)
				.CheckProperty( p => p.Thumbnail, new Thumbnail() { Bytes = new byte[] {1,2,3}})
				.VerifyTheMappings ();
		}

		[Test]
		[Category ("Database")]
		public void ProductPersistence_Session ()
		{
			using(var tx = Session.BeginTransaction ())
			{
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

			using (var tx = Session.BeginTransaction ())
			{
				var actualProduct = Session.Get<Product> (1);
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

