using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using DaiDaKaram.Domain;
using DaiDaKaram.Domain.Entities;
using System.Runtime.Serialization;

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
			return _categoryService.GetRootCategories ();
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

		//[HttpGet]
		//[Route ("api/category/ref")]
		//public Item GetItems()
		//{
		//	var i = new Item () {Name="aaa"};
		//	i.Children = new List<Item> () {
		//		new Item() {Name="a1", Parent=i},
		//		new Item() {Name="a2", Parent=i},
		//		new Item() {Name="a3", Parent=i}
		//	};
		//	return i;
		//}

		//[DataContract(IsReference = true)]
		//public class Item
		//{
		//	[DataMember] public string Name { get; set;}
		//	[DataMember] public Item Parent { get; set; }
		//	[DataMember] public List<Item> Children { get; set;}
		//}
	}
}