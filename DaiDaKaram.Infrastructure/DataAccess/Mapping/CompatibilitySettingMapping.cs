using System;
using DaiDaKaram.Domain.Entities;
using FluentNHibernate.Mapping;

namespace DaiDaKaram.Infrastructure.DataAccess
{
	public class CompatibilitySettingMapping : ClassMap<CompatibilitySetting>
	{
		public CompatibilitySettingMapping ()
		{
			Id (x => x.Id).GeneratedBy.Guid ();
			Map (x => x.Make);
			Map (x => x.Model);
			Map (x => x.Variant);
			References<Product> (x=>x.Product, "ProductId").Cascade.SaveUpdate ();
		}
	}
}

