using System;
using System.Runtime.Serialization;

namespace DaiDaKaram.Domain.Entities
{
	[Serializable]
	[DataContract]
	public class CompatibilitySetting
	{
		public virtual Guid Id { get; set; }
		public virtual string Make { get; set; }
		public virtual string Model { get; set; }
		public virtual string Variant { get; set; }
		public virtual Product Product { get; set;}

	}
}