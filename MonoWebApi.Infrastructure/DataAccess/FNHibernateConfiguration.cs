using System;
using System.Configuration;
using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using MonoWebApi.Domain.Entities;
using NHibernate;
using NHibernate.SqlCommand;

namespace MonoWebApi.Infrastructure
{
	public class FNHibernateConfiguration
	{
		public FNHibernateConfiguration ()
		{
		}

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
				                            .Database (MySQLConfiguration.Standard.ConnectionString (connectionString).ShowSql ())
					//.Mappings (x => {
					//	x.FluentMappings.AddFromAssembly (Assembly.GetExecutingAssembly ());
					//	//x.FluentMappings.Conventions.Add <NHM2MTableNameConvention>();
					//});
					.Mappings (x => x.FluentMappings.AddFromAssembly (Assembly.GetExecutingAssembly ()))
											.ExposeConfiguration (SetNHConfiguration)
				                            .ExposeConfiguration (conf => {
												conf.SetInterceptor (new SqlStatementInterceptor());
											});
				FNHConfiguration = configuration;
				_factory = configuration.BuildSessionFactory ();
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

	public class SqlStatementInterceptor : EmptyInterceptor
	{
		public override bool OnSave (object entity, object id, object [] state, string [] propertyNames, NHibernate.Type.IType [] types)
		{
			// caveat for setting relation on both entities.
			// only required for this one-to-one relation
			if (entity is Product) {
				((Product)entity).Thumbnail.Product = (Product)entity;
			}

			return base.OnSave (entity, id, state, propertyNames, types);
		}

		public override SqlString OnPrepareStatement (SqlString sql)
		{
			//Console.WriteLine (sql);

			return sql;
		}

	}
}

