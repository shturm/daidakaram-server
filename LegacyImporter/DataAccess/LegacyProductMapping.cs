using System;
using FluentNHibernate.Mapping;

namespace LegacyImporter
{
	public class LegacyProductMapping : ClassMap<LegacyProduct>
	{
		public LegacyProductMapping ()
		{
			Table ("sample_1");
			ReadOnly ();

			Id (p => p.SKU, "sku");
			Map (p => p.ItemName, "item");
			Map (p => p.GroupName, "group");
			Map (p => p.TypeName, "type");
			Map (p => p.OEM, "oem");

		}
	}
}

