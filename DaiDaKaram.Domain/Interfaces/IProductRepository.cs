using System;
using System.Collections.Generic;
using DaiDaKaram.Domain.Entities;
using DaiDaKaram.Domain.Infrastructure;

namespace DaiDaKaram.Domain
{
	public interface IProductRepository : IRepository<Product>
	{
		IEnumerable<Product> GetPage (int pageNumber, int pageSize = 20);
	}
}

