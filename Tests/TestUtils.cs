using System;
using System.Configuration;
using System.Reflection;
using Autofac;
using Autofac.Integration.WebApi;
using DaiDaKaram.Domain;
using DaiDaKaram.Infrastructure;
using DaiDaKaram.Infrastructure.WebApi;
using NHibernate;

namespace Integration
{
	public class TestUtils
	{
		private static IContainer Container;

		public static ILifetimeScope GetAutofacScope()
		{
			if (TestUtils.Container == null)
			{
				InitializeAutofacContainer ();
			}

			return Container.BeginLifetimeScope ();
		}

		public static void InitializeAutofacContainer()
		{
			ConfigurationManager.ConnectionStrings.Add (
				new ConnectionStringSettings (
					"DefaultConnection",
					"Server=localhost;Database=daidakaram-test;Uid=uniuser;Pwd=unipass;")
			);

			var builder = new ContainerBuilder ();
			builder.RegisterApiControllers (Assembly.GetAssembly (typeof (Startup)));
			AutofacInfrastructureConfiguration.Configure (builder);
			AutofacDomainConfiguration.Configure (builder);

			ISession nhSession = null;
			builder.Register (c => {
				if (nhSession == null || !nhSession.IsOpen) {
					nhSession = FNHibernateConfiguration.OpenSession ();
					Console.WriteLine ("Session initiated ");
				}

				return nhSession;
			}).As<ISession> ();

			Container = builder.Build ();
		}
	}
}

