using System;
using System.Reflection;
using Autofac;
using DaiDaKaram.Domain.Entities;
using DaiDaKaram.Domain.Infrastructure;

namespace DaiDaKaram.Domain
{
	public class AutofacDomainConfiguration
	{
		public static void Configure(ContainerBuilder builder)
		{
			builder.RegisterType<ProductService> ().AsImplementedInterfaces ();
			builder.RegisterType<CategoryService> ().As<ICategoryService> ();
			builder.RegisterType<CompatibilityService> ().As<ICompatibilityService>();
		}
	}
}