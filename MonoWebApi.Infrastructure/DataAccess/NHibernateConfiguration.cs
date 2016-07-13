using System;
using NHibernate;
using FluentNHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using System.Reflection;
using System.Configuration;
using NHibernate.Cfg;
using FluentNHibernate.Automapping;
using MonoWebApi.Domain.Entities;

namespace MonoWebApi.Infrastructure.DataAccess
{
	public class NHibernateConfiguration
	{
		static ISessionFactory _factory;
		public static readonly object lockObject = new object ();
		public static FluentConfiguration FNHConfiguration { get; private set; }

		public static NHibernate.Cfg.Configuration NHConfiguration { get; private set; }


		public static ISessionFactory GetSessionFactory ()
		{
			if (_factory != null) {
				return _factory;
			}

			lock (lockObject) {
				var connectionString = ConfigurationManager.ConnectionStrings ["DefaultConnection"].ConnectionString;
				var configuration = Fluently.Configure ()
					.Database (MySQLConfiguration.Standard.ConnectionString (connectionString))
					.Mappings (x => {
						//x.FluentMappings.Conventions.Add <NHM2MTableNameConvention>();
						//x.FluentMappings.AddFromAssembly (Assembly.GetExecutingAssembly ()); // use ProductMapping, ImageMapping, etc
						var mainMapping = AutoMap.AssemblyOf<Product> (new NHAutoMappingConfiguration ())
												.IgnoreBase<BaseEntity> ()
					                             .Override<Product> (pMap => {
													pMap.Map (p=>p.Description).Length (1000);
													pMap.HasMany<Image> (p=>p.Photos).Cascade.All ();
													pMap.References<Image> (p=>p.Thumbnail).Cascade.All ().Fetch.Join ();
												});
						x.AutoMappings.Add (mainMapping);
					})
					.ExposeConfiguration (SetNHConfiguration);
				FNHConfiguration = configuration;
				try {
					_factory = configuration.BuildSessionFactory ();
				} catch (Exception ex) {
					Console.WriteLine (ex);
					throw ex;
				}

			}

			return _factory;
		}

		public static ISession OpenSession ()
		{
			var factory = GetSessionFactory ();
			var session = factory.OpenSession ();

			return session;
		}

		static void SetNHConfiguration (NHibernate.Cfg.Configuration config)
		{
			NHConfiguration = config;
		}
	}
}