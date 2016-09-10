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
		public static void Configure(ContainerBuilder builder)
		{
			//builder.RegisterGeneric <Repository<>>().As <IRepository<>>();



			builder.RegisterGeneric (typeof(Repository<>)).As (typeof(IRepository<>));
			builder.RegisterType<ImageManipulator> ().AsImplementedInterfaces ();
			builder.Register<MySQLDatabase> (c => new MySQLDatabase ()).As (typeof(MySQLDatabase));

			builder.Register<ISession> (ctx => FNHibernateConfiguration.OpenSession ()).InstancePerRequest ();
		}
	}
}