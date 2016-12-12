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

		public void DeleteById (int categoryId)
		{
			var c = _categoryRepository.Get (categoryId);
			_categoryRepository.Delete (c);
		}

		public Category GetById (int id)
		{
			return _categoryRepository.Get (c => c.Id == id);
		}

		public IEnumerable<Category> GetRootCategories ()
		{
			var childless = _categoryRepository.AsQueryable ()
											   .Where (c => c.Parent == null)
											   .Where (c => c.SubCategories.Count () == 0)
			                                   .Select (c => new Category(){ Id=c.Id, Name=c.Name })
											   .ToList ();
			var flat =  _categoryRepository.AsQueryable ()
									  .Where (c => c.Parent == null)
				                      .SelectMany (c => c.SubCategories, 
				                                   (parent, child) => new {ParentId = parent.Id, ParentName = parent.Name, ChildId = child.Id, ChildName = child.Name})
				                      .ToList ();
			List<Category> parents = new List<Category> ();
			foreach (var child in flat) {
				var parent = parents.Where (c=>c.Id == child.ParentId).FirstOrDefault ();
				if (parent == null) {
					parent = new Category (){Id = child.ParentId, Name = child.ParentName };
					parents.Add (parent);
				}
				parent.SubCategories.Add (new Category(){Id=child.ChildId, Name=child.ChildName});
			}
			var result = parents.Union (childless).ToList ();
			return result;
			//return parents.Union (childless).ToList();
			//return parents;
			//return new List<Category> ();
			//var result = _categoryRepository.GetAll (c => c.Parent == null).ToList ();
			//return result;
		}

		public void Update (Category c)
		{
			_categoryRepository.Update (c);
		}
	}
}