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

				var mainMapping =
					AutoMap.AssemblyOf<Product> (new NHAutoMappingConfiguration ())
						.IgnoreBase<BaseEntity> ()
						.Override<Product> (pMap => {
							pMap.Map (p => p.Description).Length (1000);
							pMap.HasMany<Image> (p => p.Photos).Cascade.All ().Fetch.Join ();
							pMap.HasOne<Image> (p => p.Thumbnail).PropertyRef (i => i.ProductOwningAsThumbnail).Cascade.All ();
						}).Override<Image> (iMap => {
							iMap.References<Product> (i => i.ProductOwningAsThumbnail).Cascade.SaveUpdate ();
						});
				FNHConfiguration = Fluently.Configure ()
					.Database (MySQLConfiguration.Standard.ConnectionString (connectionString))
                    .Mappings (mappingConfiguration => mappingConfiguration.AutoMappings.Add (mainMapping))
					.ExposeConfiguration (SetNHConfiguration);

				 
				try {
					_factory = FNHConfiguration.BuildSessionFactory ();
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