using System;
using System.Runtime.Serialization;
using DaiDaKaram.Domain.Entities;

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
			if (p.Category != null)
			{
				CategoryId = p.Category.Id;
				CategoryName = p.Category.Name;
			}

		}
	}
}