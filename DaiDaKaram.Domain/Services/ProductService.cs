using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using DaiDaKaram.Domain.Entities;
using DaiDaKaram.Domain.Infrastructure;

namespace DaiDaKaram.Domain
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
			var photo = _photoRepository.Get (imageId);
			_photoRepository.Delete (photo);
		}

		public void RemoveThumbnail (int thumbnailId)
		{
			_thumbnailRepository.Delete (new Thumbnail () { Id = thumbnailId });
		}

		public void AddPhoto (int productId, Photo photo)
		{
			_photoRepository.Insert (photo);
			var product = _productRepository.Get (p => p.Id == productId);
			product.Photos.Add (photo);
			_productRepository.Update (product);
		}

		public void SetThumbnail (int productId, Photo photo)
		{
			var product = _productRepository.Get (p => p.Id == productId);
			if (product.Thumbnail != null)
				_thumbnailRepository.Delete (product.Thumbnail);
			

			product.Thumbnail = _imageManipulator.ResizeToThumbnail (photo);
			_productRepository.Update (product);
		}

		public void ChangeThumbnail(int productId, int photoIndex)
		{
			var product = _productRepository.Get (p => p.Id == productId);
			_thumbnailRepository.Delete (product.Thumbnail);

			product.Thumbnail = _imageManipulator.ResizeToThumbnail (product.Photos [photoIndex]);
			product.Thumbnail.Product = product;
			       
			_productRepository.Update (product);
		}

		public void Update (Product p)
		{
			_productRepository.Update (p);
		}
	}
}