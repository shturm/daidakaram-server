using System;
using FluentNHibernate.Mapping;
using MonoWebApi.Domain.Entities;

namespace MonoWebApi.Infrastructure.DataAccess
{
	public class ThumbnailMapping : SubclassMap<Thumbnail>
	{
		public ThumbnailMapping ()
		{
			DiscriminatorValue ("yes");
		}
	}
}