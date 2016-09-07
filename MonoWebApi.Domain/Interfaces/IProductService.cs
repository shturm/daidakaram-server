using System.Collections.Generic;
using MonoWebApi.Domain.Entities;

namespace MonoWebApi.Domain
{
	public interface IProductService
	{
		Product Create (string name, string description = null, List<Image> photos = null, Image thumb = null);
		void RemoveImage (int imageId);
		void AddImage (int productId, Image image);
		void SetThumbnail (int productId, Image image);
		void ChangeThumbnail (int productId, int photoIndex);
		void Update (Product p);
	}
}