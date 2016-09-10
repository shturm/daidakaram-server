using System;
using FluentNHibernate.Mapping;
using DaiDaKaram.Domain.Entities;

namespace DaiDaKaram.Infrastructure.DataAccess
{
	public class ThumbnailMapping : SubclassMap<Thumbnail>
	{
		public ThumbnailMapping ()
		{
			DiscriminatorValue ("yes");
			References<Product> (x => x.Product, "ProductId");
		}
	}
}