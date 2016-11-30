using System;
namespace DaiDaKaram.Domain.Entities
{
	public class Compatibility
	{
		public int Id { get; set; }
		public string Make { get; set; }
		public string Model { get; set; }
		public string Variant { get; set; }
		public string Body { get; set; }
		public string Type { get; set; }
		//public int YearFrom { get; set; }
		//public int YearTo { get; set; }
		//public int EngineCcm { get; set; }
		public int EngineHp { get; set; }
		//public int EngineKw { get; set; }
		public int EngineFuel { get; set; }
	}
}