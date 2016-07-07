using NHibernate;
using Autofac;
using Autofac.Integration.WebApi;
using MonoWebApi.Infrastructure.Services;
using MonoWebApi.Infrastructure.DataAccess;
using MonoWebApi.Domain.Infrastructure;
using System.Reflection;

namespace MonoWebApi.Infrastructure
{
	public class AutofacInfrastructureConfiguration
	{
		public static void Configure(ContainerBuilder builder)
		{
			//var session = NHibernateConfiguration.OpenSession ();
			//builder.Register (c => {
			//	return NHibernateConfiguration.OpenSession ();
			//}).As <ISession>().InstancePerLifetimeScope ();

			builder.Register (c => NHibernateConfiguration.OpenSession()).As <ISession>();
			builder.RegisterGeneric (typeof(Repository<>)).As (typeof(IRepository<>));
			builder.RegisterType<MyInfrastructureService> ().AsImplementedInterfaces ();
			builder.RegisterType<ImageService> ().AsImplementedInterfaces ();
		}
	}
}