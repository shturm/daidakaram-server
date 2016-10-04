using System;
using System.Configuration;
using System.Reflection;
using Autofac;
using Autofac.Integration.WebApi;
using DaiDaKaram.Infrastructure.WebApi;
using DaiDaKaram.Infrastructure;
using DaiDaKaram.Domain;
using NUnit.Framework;
using System.Web.Http.Hosting;
using System.Web.Http;
using NHibernate;

namespace Integration
{
	[TestFixture]
	public abstract class ApiControllerTests<TController> where TController : ApiController
	{
		protected ISession Session;

		protected ILifetimeScope Scope { get; private set; }
		protected TController Controller { get; private set; }

		[TestFixtureSetUp]
		public virtual void Init ()
		{
			Scope = TestUtils.GetAutofacScope ();
			Session = Scope.Resolve<ISession> ();
		}

		[SetUp]
		public virtual void SetUp ()
		{
			using (var tx = Session.BeginTransaction ()) {
				Session.CreateSQLQuery ("truncate Product").List ();
				Session.CreateSQLQuery ("truncate Category").List ();
				Session.CreateSQLQuery ("truncate Image").List ();
				tx.Commit ();
			}

			try {
				Controller = Scope.Resolve<TController> ();
			} catch (Exception ex) {
				Console.WriteLine (ex);
			}

			Controller.Request = new System.Net.Http.HttpRequestMessage ();
			Controller.Request.Properties.Add (HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration ());
		}

		[TestFixtureTearDown]
		public virtual void ShutDown()
		{
			Scope.Dispose ();
		}
	}
}
