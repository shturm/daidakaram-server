using System;
using System.Linq;
using System.Collections.Generic;
using DaiDaKaram.Domain.Entities;
using DaiDaKaram.Domain.Infrastructure;

namespace DaiDaKaram.Domain
{
	public class CategoryService : ICategoryService
	{
		readonly IRepository<Category> _categoryRepository;

		public CategoryService (IRepository<Category> repo)
		{
			_categoryRepository = repo;
		}

		public void AddCategory (Category c)
		{
			_categoryRepository.Insert (c);
		}

		public Category GetById (int id)
		{
			return _categoryRepository.Get (c => c.Id == id);
		}

		public IEnumerable<Category> GetRootCategories ()
		{
			var result = _categoryRepository.GetAll (c => c.Parent == null).ToList ();
			return result;
		}
	}
}