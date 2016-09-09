using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MonoWebApi.Domain.Infrastructure;
using NHibernate;
using NHibernate.Linq;

namespace MonoWebApi.Infrastructure
{
	public class Repository<TEntity> : IRepository<TEntity> where TEntity: class
	{
		public Repository ()
		{
		}

		readonly ISession _session;

		public Repository (ISession session)
		{
			_session = session;
		}

		public TEntity Get (long id)
		{
			throw new NotImplementedException ();
		}

		public TEntity Get (Expression<Func<TEntity, bool>> predicate)
		{
			using (var tx = _session.BeginTransaction ()) {
				return _session.Query<TEntity> ().Where (predicate).ToList ().FirstOrDefault ();
			}
		}

		public IEnumerable<TEntity> GetAll (Expression<Func<TEntity, bool>> expression)
		{
			throw new NotImplementedException ();
		}

		public IEnumerable<TEntity> GetAll ()
		{
			using (var tx = _session.BeginTransaction ()) {
				return _session.CreateCriteria<TEntity> ().List<TEntity> ();
			}
		}

		public void Delete (TEntity entity)
		{
			using (_session.BeginTransaction ()) {
				_session.Delete (entity);
			}
		}

		public void Delete (IList<TEntity> entity)
		{
			throw new NotImplementedException ();
		}

		public void Insert (TEntity entity)
		{
			using (var tx = _session.BeginTransaction ()) {
				_session.Save (entity);
				tx.Commit ();
			}
		}

		public void Insert (IList<TEntity> entity)
		{
			throw new NotImplementedException ();
		}

		public void Update (TEntity entity)
		{
			using (var tx = _session.BeginTransaction ()) {
				_session.SaveOrUpdate (entity);
				tx.Commit ();
			}
		}

		public void Update (IList<TEntity> entity)
		{
			throw new NotImplementedException ();
		}

		public void Dispose ()
		{
			_session.Dispose ();
		}




	}
}

