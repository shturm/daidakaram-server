using System;
namespace DaiDaKaram.Infrastructure.WebApi
{
	public class CreateCompatibilityCommand
	{
		public string Make { get; set; }
		public string Model { get; set; }
		public string [] Variants { get; set; }
		public int ProductId { get; set;}

		public CreateCompatibilityCommand ()
		{
			Variants = new string [] {};
		}
	}
}