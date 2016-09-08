using Autofac;
using Autofac.Integration.WebApi;
using MonoWebApi.Infrastructure.Services;
using MonoWebApi.Domain.Infrastructure;
using System.Reflection;
using MonoWebApi.Domain.Entities;
using NHibernate;

namespace MonoWebApi.Infrastructure
{
	public class AutofacInfrastructureConfiguration
	{
		public static void Configure(ContainerBuilder builder)
		{
			builder.RegisterType (typeof (ProductRepository)).As (typeof (IRepository<Product>));
			builder.RegisterType(typeof(ImageRepository)).As (typeof(IRepository<Image>));
			builder.RegisterType<ImageManipulator> ().AsImplementedInterfaces ();
			builder.Register<MySQLDatabase> (c => new MySQLDatabase ()).As (typeof(MySQLDatabase));
			builder.Register(c => FNHibernateConfiguration.OpenSession ()).As <ISession>();
		}
	}
}