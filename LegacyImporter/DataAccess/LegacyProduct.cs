using System;
namespace LegacyImporter
{
	public class LegacyProduct
	{
		public LegacyProduct ()
		{
		}

		public virtual string TypeName { get; set; }
		public virtual string GroupName { get; set; }
		public virtual string ItemName { get; set; }
		public virtual string SKU { get; set; }
		public virtual string OEM { get; set;}

	}
}

