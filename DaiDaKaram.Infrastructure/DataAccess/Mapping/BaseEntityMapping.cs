using System;
using FluentNHibernate.Mapping;
using DaiDaKaram.Domain.Entities;
using NHibernate.Type;

namespace DaiDaKaram.Infrastructure.DataAccess
{
	public class BaseEntityMapping : ClassMap<BaseEntity>
	{
		public BaseEntityMapping ()
		{
			UseUnionSubclassForInheritanceMapping ();

			//Id (x => x.Id).GeneratedBy.Identity (); // not working 
			//Id (x => x.Id).GeneratedBy.Native("Id"); // not working 
			//Id (x => x.Id).GeneratedBy.Sequence ("Id"); // not working
			Id (x => x.Id).GeneratedBy.Increment ();// works, unique incremented values across all tables
			Map (x => x.Created).CustomType <UtcDateTimeType>();
			Map (x => x.Updated).CustomType<UtcDateTimeType> ();

			//Version (i => i.Updated).Column ("Updated");
			//OptimisticLock.Version ();
		}
	}
}