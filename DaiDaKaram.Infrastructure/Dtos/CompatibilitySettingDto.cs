using System;
using DaiDaKaram.Domain.Entities;

namespace DaiDaKaram.Infrastructure
{
	public class CompatibilitySettingDto
	{
		public Guid Id { get; set;}
		public string Make { get; set;}
		public string Model { get; set;}
		public string Variant { get; set;}
		public int ProductId { get; set;}

		public CompatibilitySettingDto ()
		{
		}

		public CompatibilitySettingDto (CompatibilitySetting s)
		{
			Id = s.Id;
			Make = s.Make;
			Model = s.Model;
			Variant = s.Variant;
			ProductId = s.Product.Id;
		}
	}
}