using NUnit.Framework;
using System.Configuration;
using System.Linq;
using MonoWebApi.Domain.Entities;
using MonoWebApi.Infrastructure;
using Moq;
using System.Collections.Generic;

namespace Integration
{
	[TestFixture]
	public class RepositoryTests
	{

		[SetUp]
		public void Setup()
		{
			//ConfigurationManager.ConnectionStrings.Add (new ConnectionStringSettings ("DefaultConnection", "Server=localhost;Database=koshiyam;Uid=uniuser;Pwd=unipass;"));
			//SessionMock = new Mock<ISession> ();
			//TxMock = new Mock<ITransaction> ();
			//sut = new Repository<Image> (SessionMock.Object);

		}

	}
}