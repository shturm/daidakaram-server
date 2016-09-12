using System;

namespace DaiDaKaram.Domain.Entities
{
	public class Image
	{
		public Image ()
		{
			Created = DateTime.Now;
		}
		public virtual int Id { get; set; }
		public virtual DateTime Created { get; set; }
		public virtual DateTime Updated { get; set; }

		public virtual byte [] Bytes { get; set; }
		public virtual Product Product { get; set; }
		public virtual bool IsThumbnail { get; set;}
	}
}