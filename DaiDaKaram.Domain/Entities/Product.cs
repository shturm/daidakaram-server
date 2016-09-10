using System;
using System.Collections.Generic;
using System.Linq;

namespace DaiDaKaram.Domain.Entities
{
	public class Product : BaseEntity
	{
		public Product () :base()
		{
			Photos = new List<Photo> ();
		}
		public virtual string Name { get; set; }
		public virtual string Description { get; set; }
		public virtual IList<Photo> Photos { get; set; }
		public virtual Thumbnail Thumbnail { get; set; }
	}
}