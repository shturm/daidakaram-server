using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using MonoWebApi.Domain.Entities;
using MonoWebApi.Domain.Infrastructure;

namespace MonoWebApi.Infrastructure
{
	public class ProductRepository : IDisposable, IRepository<Product>
	{
		public void Delete (IList<Product> entity)
		{
			throw new NotImplementedException ();
		}

		public void Delete (Product entity)
		{
			throw new NotImplementedException ();
		}

		public void Dispose ()
		{
			
		}

		public IEnumerable<Product> Get (long id)
		{
			throw new NotImplementedException ();
		}

		public IEnumerable<Product> Get (Expression<Func<Product, bool>> expression)
		{
			throw new NotImplementedException ();
		}

		public IEnumerable<Product> GetAll ()
		{
			throw new NotImplementedException ();
		}

		public IEnumerable<Product> GetAll (Expression<Func<Product, bool>> expression)
		{
			throw new NotImplementedException ();
		}

		public void Insert (IList<Product> entity)
		{
			throw new NotImplementedException ();
		}

		public void Insert (Product entity)
		{
			throw new NotImplementedException ();
		}

		public void Update (IList<Product> entity)
		{
			throw new NotImplementedException ();
		}

		public void Update (Product entity)
		{
			throw new NotImplementedException ();
		}
	}
}