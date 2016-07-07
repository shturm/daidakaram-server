using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoWebApi.Domain.Entities
{
	public class Product : BaseEntity
	{
		public virtual string Name { get; set; }
		public virtual string Description { get; set; }
		public virtual IList<Image> Photos { get; set; }
		public virtual Image Thumbnail { get; set; }
	}
}