using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using MonoWebApi.Domain.Entities;
using MonoWebApi.Domain.Infrastructure;

namespace MonoWebApi.Domain
{
	public class ProductService : IProductService
	{
		IRepository<Product> _productRepository;
		IRepository<Thumbnail> _thumbnailRepository;
		IRepository<Photo> _photoRepository;

		IImageManipulator _imageManipulator;

		public ProductService (IRepository<Product> productRepo, IRepository<Thumbnail> thumbRepo, IRepository<Photo> photoRepo, IImageManipulator imageManipulator)
		{
			_productRepository = productRepo;
			_thumbnailRepository = thumbRepo;
			_photoRepository = photoRepo;
			_imageManipulator = imageManipulator;
		}

		public Product Create (string name, string description = null, List<Photo> photos = null, Thumbnail thumb = null)
		{
			Contract.Ensures (Contract.Result<Product> () != null);
			var product = new Product () {
				Name=name,
				Description = description,
				Photos = photos,
				Thumbnail = thumb
			};

			if (product.Photos != null && product.Thumbnail == null)
			{
				var thumbnail = _imageManipulator.ResizeToThumbnail (photos.First ());
				product.Thumbnail = thumbnail;
				thumbnail.Product = product;
			}

			if (product.Thumbnail != null)
			{
				product.Thumbnail = _imageManipulator.ResizeToThumbnail (product.Thumbnail);
			}

			_productRepository.Insert (product);

			return product;
		}

		public void RemovePhoto (int imageId)
		{
			_photoRepository.Delete (new Photo() {Id=imageId});
		}

		public void RemoveThumbnail (int thumbnailId)
		{
			_thumbnailRepository.Delete (new Thumbnail () { Id = thumbnailId });
		}

		public void AddImage (int productId, Photo photo)
		{
			_photoRepository.Insert (photo);
			var product = _productRepository.Get (p => p.Id == productId).FirstOrDefault ();
			product.Photos.Add (photo);
			_productRepository.Update (product);
		}

		public void SetThumbnail (int productId, Photo photo)
		{
			var product = _productRepository.Get (p => p.Id == productId).FirstOrDefault ();
			if (product.Thumbnail != null)
				_thumbnailRepository.Delete (product.Thumbnail);
			

			product.Thumbnail = _imageManipulator.ResizeToThumbnail (photo);
			_productRepository.Update (product);
		}

		public void ChangeThumbnail(int productId, int photoIndex)
		{
			var product = _productRepository.Get (p => p.Id == productId).FirstOrDefault ();
			_thumbnailRepository.Delete (product.Thumbnail);

			product.Thumbnail = _imageManipulator.ResizeToThumbnail (product.Photos [photoIndex]);

			_productRepository.Update (product);
		}

		public void Update (Product p)
		{
			_productRepository.Update (p);
		}
	}
}