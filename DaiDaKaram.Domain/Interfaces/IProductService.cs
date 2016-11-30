using System.Collections.Generic;
using DaiDaKaram.Domain.Entities;

namespace DaiDaKaram.Domain
{
	public interface IProductService
	{
		Product Create (string name, string sku, string description = null, List<Photo> photos = null, Thumbnail thumb = null);
		void RemovePhoto (int imageId);
		void AddPhoto (int productId, Photo photo);
		void SetThumbnail (int productId, Photo image);
		void ChangeThumbnail (int productId, int photoIndex);
		void Update (Product p);

		Product Get (int id);

		void ImportPhoto (string sku, byte [] photoBytes);
		void ImportProduct (string typeName, string groupName, string productName, string sku, string oem);
		IEnumerable<Product> GetPage (int pageNumber, int pageSize = 20);
	}
}