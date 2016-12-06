using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DaiDaKaram.Domain.Entities;
using DaiDaKaram.Infrastructure;
using DaiDaKaram.Domain;

namespace DaiDaKaram.Infrastructure
{
	[Serializable]
	[DataContract]
	public class ProductDto
	{

		[DataMember] public int Id { get;  set; }
		[DataMember] public string Name { get;  set; }
		[DataMember] public string SKU { get;  set; }
		[DataMember] public decimal Price { get;  set; }
		[DataMember] public int? CategoryId { get;  set; }
		[DataMember] public string CategoryName { get;  set; }
		[DataMember] public string CompatibilityStatus { get; set; }
		[DataMember]
		public IEnumerable<CompatibilitySettingDto> CompatibilitySettings { get; set; }

		public ProductDto ()
		{

		}
		public ProductDto (Product p)
		{
			MapFromProduct (p);
		}

		void MapFromProduct (Product p)
		{
			Id = p.Id;
			Name = p.Name;
			SKU = p.SKU;
			Price = p.Price;
			CompatibilitySettings = p.CompatibilitySettings.Select (s => new CompatibilitySettingDto(s));
			if (p.Category != null)
			{
				CategoryId = p.Category.Id;
				CategoryName = p.Category.Name;
			}

			switch (p.CompatibilityStatus) {
				case Domain.CompatibilityStatus.Unknown:
					CompatibilityStatus = "UNKNOWN";
					break;
				case Domain.CompatibilityStatus.Settings:
					CompatibilityStatus = "SETTINGS";
					break;
				case Domain.CompatibilityStatus.NotApplicable:
					CompatibilityStatus = "NA";
					break;
			default:
				break;
			}


		}
	}
}