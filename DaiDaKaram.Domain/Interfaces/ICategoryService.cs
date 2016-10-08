using System;
using System.Collections.Generic;
using DaiDaKaram.Domain.Entities;

namespace DaiDaKaram.Domain
{
	public interface ICategoryService
	{
		void AddCategory (Category c);
		IEnumerable<Category> GetRootCategories ();
		void Update (Category c);
		void DeleteById (int categoryId);
	}
}