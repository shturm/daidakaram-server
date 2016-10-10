using System;
using System.Collections.Generic;
using DaiDaKaram.Domain;
using DaiDaKaram.Domain.Entities;
using NHibernate;

namespace DaiDaKaram.Infrastructure
{
	public class ProductRepostiroy : Repository<Product>, IProductRepository
	{
		public ProductRepostiroy (ISession s) :base(s)
		{
		}

		public IEnumerable<Product> GetPage(int pageNumber, int pageSize=20)
		{
			IEnumerable<Product> result;
			using (var tx = _session.BeginTransaction ()) {
				result = _session.QueryOver<Product> ()
						.OrderBy (p => p.SKU).Asc
						.Skip (pageNumber * pageSize)
						.Take (pageSize)
				        .List ();
				tx.Commit ();
			}
			return result;
		}
	}
}

