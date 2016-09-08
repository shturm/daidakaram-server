using System.Collections.Generic;
using MonoWebApi.Domain.Entities;

namespace MonoWebApi.Domain
{
	public interface IProductService
	{
		Product Create (string name, string description = null, List<Photo> photos = null, Thumbnail thumb = null);
		void RemovePhoto (int imageId);
		void AddImage (int productId, Photo photo);
		void SetThumbnail (int productId, Photo image);
		void ChangeThumbnail (int productId, int photoIndex);
		void Update (Product p);
	}
}