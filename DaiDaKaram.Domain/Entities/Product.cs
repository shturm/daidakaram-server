using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DaiDaKaram.Domain.Entities
{
	[DataContract]
	public class Product : BaseEntity
	{
		CompatibilityStatus _compatibilityStatus = CompatibilityStatus.Unknown;

		public Product () :base()
		{
			Photos = new List<Photo> ();
			CompatibilitySettings = new List<CompatibilitySetting> ();
			CompatibilityStatus = CompatibilityStatus.Unknown;

			if (CompatibilitySettings.Count () > 0)
			{
				CompatibilityStatus = CompatibilityStatus.Settings;
			}
		}
		[DataMember] public virtual string Name { get; set; }
		public virtual string Description { get; set; }
		[DataMember] public virtual decimal Price { get; set;}
		[DataMember] public virtual Category Category { get; set;}
		public virtual IList<Photo> Photos { get; set; }
		public virtual Thumbnail Thumbnail { get; set; }
		[DataMember] public virtual string SKU { get; set; }
		[DataMember] public virtual string OEM { get; set; }
		[DataMember] public virtual CompatibilityStatus CompatibilityStatus { 
			get {
				if (CompatibilitySettings.Count () > 0)
				{
					return CompatibilityStatus.Settings;
				}
				return _compatibilityStatus;
			}
			set {
				_compatibilityStatus = value;
			}
		}
		[DataMember]
		public virtual IList<CompatibilitySetting> CompatibilitySettings { get; set;}

		public override string ToString ()
		{
			return string.Format ("[Product#{0}]", Id);
		}

	}
}