using System;
namespace DaiDaKaram.Domain.Entities
{
	public class Photo : Image
	{
		public Photo ():base() {}
		public virtual string Hash { get; set; }
	}
}