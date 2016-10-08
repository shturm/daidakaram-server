using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using DaiDaKaram.Domain;
using DaiDaKaram.Domain.Entities;

namespace DaiDaKaram.Infrastructure.WebApi.Controllers
{
	public class CategoryController : ApiController
	{
		readonly ICategoryService _categoryService;

		public CategoryController (ICategoryService categoryService)
		{
			_categoryService = categoryService;
		}

		[HttpPost]
		[Route ("api/category")]
		public Category AddCategory (Category c)
		{
			_categoryService.AddCategory (c);
			return c;
		}

		[HttpGet]
		[Route ("api/category/roots")]
		public IEnumerable<Category> GetRootCategories ()
		{
			var rootCategories = _categoryService.GetRootCategories ();
			var query = from c in rootCategories.AsQueryable ()
						select new Category {
							Name = c.Name,
							SubCategories = new List<Category> (c.SubCategories)
						};

			//var result =  new [] {
			//	new Category() {Name="Cat 1", SubCategories = new List<Category> {
			//			new Category() {Name="Cat 1.1"},
			//			new Category() {Name="Cat 1.2"},
			//		}},
			//	new Category() {Name="Cat 2", SubCategories = new List<Category> {
			//			new Category() {Name="Cat 2.1"},
			//			new Category() {Name="Cat 2.2"},
			//			new Category() {Name="Cat 2.3"},
			//		}},
			//};
			return rootCategories;
			//return result;

			//return query.ToList ();
		}

		[HttpPut]
		[Route ("api/category")]
		public Category Update (Category c)
		{
			_categoryService.Update (c);
			return c;
		}

		[HttpDelete]
		[Route ("api/category/delete/{categoryId}")]
		public void Delete (int categoryId)
		{
			_categoryService.DeleteById (categoryId);
		}

	}
}