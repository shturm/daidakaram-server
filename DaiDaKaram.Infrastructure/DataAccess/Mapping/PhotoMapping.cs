﻿using System;
using FluentNHibernate.Mapping;
using DaiDaKaram.Domain.Entities;
using NHibernate.Type;

namespace DaiDaKaram.Infrastructure.DataAccess
{
	public class PhotoMapping : SubclassMap<Photo>
	{
		public PhotoMapping ()
		{
			DiscriminatorValue ("no");
			Map (i => i.Hash);
			References<Product> (x => x.Product, "ProductId");
		}
	}
}