using System;
using DaiDaKaram.Domain;
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
			Map (x => x.Price);
			Map (x => x.SKU).Length (10).Unique ();
			Map (x => x.OEM).Length (15);
			Map (x => x.CompatibilityStatus).CustomType<CompatibilityStatus> ();

			// set 1 - saves correctly, reads on PKs
			//HasOne<Image> (x => x.Thumbnail)
			//	.ForeignKey ("Product")
			//	.Cascade.All ();
			References (x=>x.Category, "CategoryId").Cascade.SaveUpdate ();

			HasOne<Thumbnail> (x=>x.Thumbnail)
				.PropertyRef ("Product")
				.Cascade.All ();
			HasMany<Photo> (x=>x.Photos).Cascade.All ();
			HasMany<CompatibilitySetting> (x => x.CompatibilitySettings).Cascade.All ();
		}
	}
}