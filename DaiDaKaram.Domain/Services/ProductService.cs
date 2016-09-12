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

		IRepository<Category> _categoryRepository;


		public ProductService (IRepository<Product> productRepo, 
		                       IRepository<Thumbnail> thumbRepo,
		                       IRepository<Photo> photoRepo, 
		                       IImageManipulator imageManipulator,
		                       IRepository<Category> categoryRepo)
		{
			_productRepository = productRepo;
			_thumbnailRepository = thumbRepo;
			_photoRepository = photoRepo;
			_imageManipulator = imageManipulator;
			_categoryRepository = categoryRepo;
		}



		public Product Create (string name, string sku, string description = null, List<Photo> photos = null, Thumbnail thumb = null)
		{
			Contract.Ensures (Contract.Result<Product> () != null);
			var product = new Product () {
				Name=name,
				SKU=sku,
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

		public void ImportPhoto (string sku, byte [] photoBytes)
		{
			var photo = new Photo () { Bytes = photoBytes };
			var product = _productRepository.Get (p => p.SKU == sku);

			if (product == null)
			{
				product = new Product () { SKU = sku};
			}

			_imageManipulator.Watermark (ref photo);
			product.Photos.Add (photo);

			_productRepository.Insert (product);
			ChangeThumbnail (product.Id, 0);
		}

		public void ImportProduct (string typeName, string groupName, string productName, string sku, string oem)
		{
			var product = _productRepository.Get (p => p.SKU == sku) ??
			                                new Product () { SKU = sku, Name = productName};
			var category = _categoryRepository.Get (c => c.Name == groupName) ?? 
			                                  new Category () { Name = groupName };
			var topCategory = _categoryRepository.Get (c => c.Name == groupName) ??
											  new Category () { Name = typeName };
			product.Category = category;
			category.Products.Add (product);

			category.Parent = topCategory;
			topCategory.SubCategories.Add (category);

			_categoryRepository.Insert (topCategory);
		}
	}
}