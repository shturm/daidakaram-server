using NUnit.Framework;
using System.Configuration;
using System.Linq;
using MonoWebApi.Domain.Entities;
using Moq;
using NHibernate;
using System.Collections.Generic;

namespace MonoWebApi.Infrastructure.Tests
{
	[TestFixture]
	public class RepositoryTests
	{
		Mock<ISession> SessionMock;
		Mock<ITransaction> TxMock;
		Repository<Image> sut;

		[SetUp]
		public void Setup()
		{
			//ConfigurationManager.ConnectionStrings.Add (new ConnectionStringSettings ("DefaultConnection", "Server=localhost;Database=koshiyam;Uid=uniuser;Pwd=unipass;"));
			SessionMock = new Mock<ISession> ();
			TxMock = new Mock<ITransaction> ();
			sut = new Repository<Image> (SessionMock.Object);

			SessionMock.Setup (s => s.BeginTransaction ()).Returns (TxMock.Object);
		}

		[Test]
		[Category ("Unit")]
		public void Insert ()
		{
			var entity = new Image ();

			Assert.DoesNotThrow (() => {
				sut.Insert (entity);
			});

			SessionMock.Verify (x => x.Save (entity), Times.Exactly (1));
			TxMock.Verify (t => t.Commit (), Times.Exactly (1));
		}

		[Test]
		[Category ("Unit")]
		public void Update ()
		{
			var entity = new Image ();

			Assert.DoesNotThrow (() => {
				sut.Update(entity);
			});

			SessionMock.Verify (x => x.SaveOrUpdate (entity), Times.Exactly (1));
			TxMock.Verify (t => t.Commit (), Times.Exactly (1));
		}

		[Test]
		[Category ("Unit")]
		public void GetAll ()
		{
			var entity = new Image ();
			int actualCount;
			var resultImages = new List<Image> () { new Image (), new Image () };
			var criteriaMock = new Mock<ICriteria> ();

			criteriaMock.Setup (c => c.List <Image>()).Returns (resultImages);
			SessionMock.Setup (s => s.CreateCriteria <Image>()).Returns (criteriaMock.Object);

			actualCount = sut.GetAll ().Count ();

			Assert.AreEqual (resultImages.Count(), actualCount);
			SessionMock.Verify (x => x.SaveOrUpdate (entity), Times.Never ());
			SessionMock.Verify (x => x.Save (entity), Times.Never ());
			SessionMock.Verify (x => x.Delete (entity), Times.Never ());
		}

	}
}