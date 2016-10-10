using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DaiDaKaram.Domain.Entities
{
	[DataContract (IsReference = true)]
	[KnownType (typeof (Product))]
	[KnownType (typeof (Category))]
	public class Category : BaseEntity
	{
		public Category () : base()
		{
			Products = new List<Product> ();
			SubCategories = new List<Category> ();
		}

		[DataMember] public virtual string Name { get; set; }
		[DataMember] public virtual Category Parent { get; set;}
		[DataMember] public virtual IList<Product> Products { get; set; }
		[DataMember] public virtual IList<Category> SubCategories { get; set; }

	}
}

