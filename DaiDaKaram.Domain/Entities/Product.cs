using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DaiDaKaram.Domain.Entities
{
	[DataContract]
	public class Product : BaseEntity
	{
		public Product () :base()
		{
			Photos = new List<Photo> ();
		}
		[DataMember] public virtual string Name { get; set; }
		public virtual string Description { get; set; }
		[DataMember] public virtual decimal Price { get; set;}
		[DataMember] public virtual Category Category { get; set;}
		public virtual IList<Photo> Photos { get; set; }
		public virtual Thumbnail Thumbnail { get; set; }
		[DataMember] public virtual string SKU { get; set; }
		[DataMember] public virtual string OEM { get; set; }
	}
}