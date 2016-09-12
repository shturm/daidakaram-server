using System;
using Autofac;
using Autofac.Integration.WebApi;
using DaiDaKaram.Infrastructure.Services;
using DaiDaKaram.Domain.Infrastructure;
using System.Reflection;
using DaiDaKaram.Domain.Entities;
using NHibernate;
using System.Diagnostics;

namespace DaiDaKaram.Infrastructure
{
	public class AutofacInfrastructureConfiguration
	{
		public static void Configure(ContainerBuilder builder, bool webApplication = false)
		{
			//builder.RegisterGeneric <Repository<>>().As <IRepository<>>();



			builder.RegisterGeneric (typeof(Repository<>)).As (typeof(IRepository<>));
			builder.RegisterType<ImageManipulator> ().AsImplementedInterfaces ();
			builder.Register<MySQLDatabase> (c => new MySQLDatabase ()).As (typeof(MySQLDatabase));

			// TODO use Autofac.Module instead
			if (webApplication) {
				// autofac takes care of only making 1 session per webapi/mvc request
				builder.Register<ISession> (ctx => FNHibernateConfiguration.OpenSession ()).InstancePerRequest ();
			} else {
				// we take care of only making 1 session per webapi/mvc request
				ISession nhSession = null;
				builder.Register (c => {
					if (nhSession == null || !nhSession.IsOpen) {
						nhSession = FNHibernateConfiguration.OpenSession ();
						Console.WriteLine ("Session initiated ");
					}

					return nhSession;
				}).As<ISession> ();
			}

		}
	}
}