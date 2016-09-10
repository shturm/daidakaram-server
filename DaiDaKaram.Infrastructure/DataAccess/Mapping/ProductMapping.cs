using System;
using DaiDaKaram.Domain.Entities;
using FluentNHibernate.Mapping;

namespace DaiDaKaram.Infrastructure.DataAccess
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
			HasMany<Photo> (x=>x.Photos)
				//.PropertyRef ("Product")
				.Cascade.All ();
			
		}
	}
}