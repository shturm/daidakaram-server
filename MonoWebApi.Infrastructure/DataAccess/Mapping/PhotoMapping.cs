using System;
using FluentNHibernate.Mapping;
using MonoWebApi.Domain.Entities;
using NHibernate.Type;

namespace MonoWebApi.Infrastructure.DataAccess
{
	public class PhotoMapping : SubclassMap<Photo>
	{
		public PhotoMapping ()
		{
			DiscriminatorValue ("no");
		}
	}
}