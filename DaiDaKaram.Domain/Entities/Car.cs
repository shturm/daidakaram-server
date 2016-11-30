using System;
namespace DaiDaKaram.Domain.Entities
{
	public class Car
	{
		public virtual int Id { get; set; }
		public virtual string Make { get; set; }
		public virtual string Model { get; set; }
		public virtual string Variant { get; set; }
		public virtual string Body { get; set; }
		public virtual string Type { get; set; }
		public virtual int YearFrom { get; set; }
		public virtual int YearTo { get; set; }
		public virtual int EngineCcm { get; set; }
		public virtual int EngineHp { get; set; }
		public virtual int EngineKw { get; set; }
		public virtual string EngineFuel { get; set; }	}
}