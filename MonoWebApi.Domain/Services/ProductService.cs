using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using MonoWebApi.Domain.Entities;
using MonoWebApi.Domain.Infrastructure;

namespace MonoWebApi.Domain
{
	class ProductService : IProductService
	{
		IRepository<Product> _productRepository;
		IRepository<Image> _imageRepository;

		IImageService _imageService;

		public ProductService (IRepository<Product> productRepo, IRepository<Image> imageRepo, IImageService imageService)
		{
			_productRepository = productRepo;
			_imageRepository = imageRepo;
			_imageService = imageService;
		}

		public Product Create (string name, string description = null, List<Image> photos = null, Image thumb = null)
		{
			Contract.Ensures (Contract.Result<Product> () != null);
			var product = new Product () {
				Name=name,
				Description = description,
				Photos = photos,
				Thumbnail = thumb
			};

			if (product.Photos != null)
				_imageRepository.Insert (product.Photos);
			
			if (product.Photos != null && product.Thumbnail == null)
				product.Thumbnail = new Image () { Bytes = product.Photos [0].Bytes};
			
			if (product.Thumbnail != null)
			{
				product.Thumbnail = _imageService.ResizeToThumbnail (product.Thumbnail);
				_imageRepository.Insert (product.Thumbnail);
			}

			
			try {
				_productRepository.Insert (product);
			} catch (Exception ex) {
				_imageRepository.Delete (product.Thumbnail);
				_imageRepository.Delete (product.Photos);
			}

			return product;
		}

		public void RemoveImage (int imageId)
		{
			_imageRepository.Delete (new Image() {Id=imageId});
		}

		public void AddImage (int productId, Image image)
		{
			_imageRepository.Insert (image);
			var product = _productRepository.Get (p => p.Id == productId).FirstOrDefault ();
			product.Photos.Add (image);
			_productRepository.Update (product);
		}

		public void SetThumbnail (int productId, Image image)
		{
			var product = _productRepository.Get (p => p.Id == productId).FirstOrDefault ();
			if (product.Thumbnail != null)
				_imageRepository.Delete (product.Thumbnail);
			
			_imageRepository.Insert (image);
			product.Thumbnail = _imageService.ResizeToThumbnail (image);
			_imageRepository.Update (image);
			_productRepository.Update (product);
		}

		public void ChangeThumbnail(int productId, int photoIndex)
		{
			var product = _productRepository.Get (p => p.Id == productId).FirstOrDefault ();
			_imageRepository.Delete (product.Thumbnail);

			product.Thumbnail = _imageService.ResizeToThumbnail (product.Photos [photoIndex]);

			_imageRepository.Update (product.Thumbnail);
			_productRepository.Update (product);
		}

		public void Update (Product p)
		{
			_productRepository.Update (p);
		}
	}
}