using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

namespace MonoWebApi.Domain.Infrastructure
{
	public interface IRepository<T> where T : class
	{
		IEnumerable<T> GetAll (Expression<Func<T, bool>> expression);
		IEnumerable<T> GetAll ();
		IEnumerable<T> Get (Expression<Func<T, bool>> expression);
		IEnumerable<T> Get (long id);
		void Insert (T entity);
		void Insert (IList<T> entity);
		void Delete (T entity);
		void Delete (IList<T> entity);
		void Update (T entity);
		void Update (IList<T> entity);
	}
}