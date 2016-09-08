using System;
using MonoWebApi.Domain.Entities;
using FluentNHibernate.Mapping;

namespace MonoWebApi.Infrastructure.DataAccess
{
	public class ProductMapping : SubclassMap<Product>
	{
		public ProductMapping ()
		{
			Map (x => x.Name);
			Map (x => x.Description);

			// set 1 - saves correctly, reads on PKs
			//HasOne<Image> (x => x.Thumbnail)
			//	.ForeignKey ("Product")
			//	.Cascade.All ();

			HasOne<Thumbnail> (x=>x.Thumbnail)
				.PropertyRef ("Product")
				.Cascade.All ();

		}
	}
}