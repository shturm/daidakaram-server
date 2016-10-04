using System;
using DaiDaKaram.Domain.Entities;
using FluentNHibernate.Mapping;

namespace DaiDaKaram.Infrastructure.DataAccess
{
	public class CategoryMapping : SubclassMap<Category>
	{
		public CategoryMapping ()
		{
			Map (c=>c.Name);
			References (c=>c.Parent, "ParentId");
			HasMany (c => c.SubCategories).Cascade.All ();
			HasMany (c=>c.Products).Cascade.All ();

		}
	}
}

