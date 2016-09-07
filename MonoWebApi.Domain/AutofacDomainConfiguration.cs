using System;
using System.Reflection;
using Autofac;
using MonoWebApi.Domain.Infrastructure;

namespace MonoWebApi.Domain
{
	public class AutofacDomainConfiguration
	{
		public static void Configure(ContainerBuilder builder)
		{
			builder.RegisterType<ProductService> ().AsImplementedInterfaces ();
		}
	}
}