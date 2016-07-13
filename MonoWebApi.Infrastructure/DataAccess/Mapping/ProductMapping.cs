using System;
using MonoWebApi.Domain.Entities;
using FluentNHibernate.Mapping;

namespace MonoWebApi.Infrastructure.DataAccess
{
	//public class ProductMapping : SubclassMap<Product>
	//{
	//	public ProductMapping ()
	//	{
	//		Map (x => x.Name);
	//		Map (x => x.Description).Length (10000);
	//		HasOne<Image> (x => x.Thumbnail)
	//			.Cascade.All ();
	//		HasMany<Image> (x => x.Photos)
	//			.Cascade.All ();
	//	}
	//}
}