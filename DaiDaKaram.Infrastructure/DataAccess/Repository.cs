using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DaiDaKaram.Domain.Infrastructure;
using NHibernate;
using NHibernate.Linq;

namespace DaiDaKaram.Infrastructure
{
	public class Repository<TEntity> : IRepository<TEntity> where TEntity: class
	{
		public Repository ()
		{
		}

		protected readonly ISession _session;

		public Repository (ISession session)
		{
			_session = session;
		}

		public TEntity Get (int id)
		{
			TEntity result;
			using(var tx = _session.BeginTransaction ())
			{
				result = _session.Get<TEntity> (id);
				tx.Commit ();
			}
			return result;
		}

		public TEntity Get (Expression<Func<TEntity, bool>> predicate)
		{
			TEntity result;
			using (var tx = _session.BeginTransaction ()) {
				result = _session.Query<TEntity> ().Where (predicate).ToList ().FirstOrDefault ();
				tx.Commit ();
			}
			return result;
		}

		public IEnumerable<TEntity> GetAll (Expression<Func<TEntity, bool>> expression)
		{
			IEnumerable<TEntity> result;
			using (var tx = _session.BeginTransaction ()) {
				result = _session.Query<TEntity> ().Where (expression).ToList ();
				tx.Commit ();
			}

			return result;
		}

		public IEnumerable<TEntity> GetAll ()
		{
			IList<TEntity> result;
			using (var tx = _session.BeginTransaction ()) {
				result = _session.CreateCriteria<TEntity> ().List<TEntity> ();
				tx.Commit ();
			}
			return result;
		}

		public void Delete (TEntity entity)
		{
			if (entity == null)
			{
				return;
			}

			using (var tx = _session.BeginTransaction ()) 
			{
				_session.Delete (entity);
				tx.Commit ();
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

		public IQueryable<TEntity> AsQueryable ()
		{
			return _session.Query<TEntity> ();
		}
	}
}

