using System;
using System.Runtime.Serialization;

namespace DaiDaKaram.Domain.Entities
{
	[Serializable]
	[DataContract (IsReference = true)]
	public class BaseEntity
	{
		public BaseEntity ()
		{
			Created = DateTime.Now;
		}
		[DataMember]
		public virtual int Id { get; set; }
		public virtual DateTime Created { get; set; }
		public virtual DateTime Updated { get; set; }
	}
}