using System;
using System.Collections.Generic;

namespace DaiDaKaram.Domain.Entities
{
	public class Category : BaseEntity
	{
		public Category () : base()
		{
			Products = new List<Product> ();
			SubCategories = new List<Category> ();
		}
		public virtual string Name { get; set; }
		public virtual Category Parent { get; set;}
		public virtual IList<Product> Products { get; set; }
		public virtual IList<Category> SubCategories { get; set; }

	}
}

