using System;
using Autofac;
using Autofac.Integration.WebApi;
using MonoWebApi.Infrastructure.Services;
using MonoWebApi.Domain.Infrastructure;
using System.Reflection;
using MonoWebApi.Domain.Entities;
using NHibernate;
using System.Diagnostics;

namespace MonoWebApi.Infrastructure
{
	public class AutofacInfrastructureConfiguration
	{
		public static void Configure(ContainerBuilder builder)
		{
			//builder.RegisterGeneric <Repository<>>().As <IRepository<>>();

			ISession nhSession = null;

			builder.RegisterGeneric (typeof(Repository<>)).As (typeof(IRepository<>));
			builder.RegisterType<ImageManipulator> ().AsImplementedInterfaces ();
			builder.Register<MySQLDatabase> (c => new MySQLDatabase ()).As (typeof(MySQLDatabase));

			builder.Register (c => {
				if (nhSession == null || !nhSession.IsOpen)
				{
					nhSession = FNHibernateConfiguration.OpenSession ();	
					Console.WriteLine ("Session initiated ");
				}

				return nhSession;
			}).As <ISession> ();
		}
	}
}