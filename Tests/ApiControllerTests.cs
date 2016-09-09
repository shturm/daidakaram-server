using System;
using System.Configuration;
using System.Reflection;
using Autofac;
using Autofac.Integration.WebApi;
using MonoWebApi.Infrastructure.WebApi;
using MonoWebApi.Infrastructure;
using MonoWebApi.Domain;
using NUnit.Framework;
using System.Web.Http.Hosting;
using System.Web.Http;

namespace Integration
{
	[TestFixture]
	public abstract class ApiControllerTests<TController> where TController : ApiController
	{
		public ILifetimeScope Scope { get; private set; }
		public TController Controller { get; private set; }

		[TestFixtureSetUp]
		public virtual void Init ()
		{
		}

		[SetUp]
		public virtual void SetUp ()
		{
			Scope = TestUtils.GetAutofacScope ();
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
		}
	}
}
