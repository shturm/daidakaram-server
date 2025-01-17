﻿using System;
using DaiDaKaram.Domain.Entities;
using FluentNHibernate.Mapping;
using NHibernate.Type;

namespace DaiDaKaram.Infrastructure.DataAccess
{
	public class ImageMapping : ClassMap<Image>
	{
		public ImageMapping ()
		{
			Table ("Image");

			//UseUnionSubclassForInheritanceMapping ();

			Id (x => x.Id).GeneratedBy.Increment ();
			Map (x => x.Created).CustomType<UtcDateTimeType> ();
			Map (x => x.Updated).CustomType<UtcDateTimeType> ();
			Map (x => x.Bytes);


			// set 1 - saves correctly, reads on PKs
			//References<Product> (x => x.Product)
			//	.Column ("ProductId")
			//	.ForeignKey ("Thumbnail");

			//References<Product> (x => x.Product, "ProductId");
				//.Formula ("(CASE WHEN IsThumbnail = 1 THEN 42 ELSE 142 END)");
				//.Unique ();

			DiscriminateSubClassesOnColumn("IsThumbnail")
				.AlwaysSelectWithValue ();

			//Version (i=>i.Updated).Column ("Updated");
			//OptimisticLock.Version ();
		}
	}
}