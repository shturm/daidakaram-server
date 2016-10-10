using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using DaiDaKaram.Domain.Entities;
using DaiDaKaram.Domain.Infrastructure;
using System.Text;

namespace DaiDaKaram.Domain
{
	public class ProductService : IProductService
	{
		IProductRepository _productRepository;
		IRepository<Thumbnail> _thumbnailRepository;
		IRepository<Photo> _photoRepository;

		IImageManipulator _imageManipulator;

		IRepository<Category> _categoryRepository;


		public ProductService (IProductRepository productRepo,
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
				Name = name,
				SKU = sku,
				Description = description,
				Photos = photos,
				Thumbnail = thumb
			};

			if (product.Photos != null && product.Thumbnail == null) {
				var thumbnail = _imageManipulator.ResizeToThumbnail (photos.First ());
				product.Thumbnail = thumbnail;
				thumbnail.Product = product;
			}

			if (product.Thumbnail != null) {
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

		public void ChangeThumbnail (int productId, int photoIndex)
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

		public Product Get(int id)
		{
			return _productRepository.Get (id);
		}

		public void ImportPhoto (string sku, byte [] photoBytes)
		{
			var photo = new Photo () { Bytes = photoBytes };
			var product = _productRepository.Get (p => p.SKU == sku);

			if (product == null) {
				product = new Product () { SKU = sku };
			}

			_imageManipulator.Watermark (ref photo);
			product.Photos.Add (photo);

			_productRepository.Insert (product);
			ChangeThumbnail (product.Id, 0);
		}

		public void ImportProduct (string typeName, string groupName, string productName, string sku, string oem)
		{
			var sbName = new StringBuilder ();
			if (!typeName.StartsWith ("^", StringComparison.InvariantCulture)) sbName.Append (typeName + " ");
			if (!groupName.StartsWith ("^", StringComparison.InvariantCulture)) sbName.Append (groupName + " ");
			sbName.Append (productName);
			var product = _productRepository.Get (p => p.SKU == sku) ??
											new Product () { SKU = sku, Name = sbName.ToString () };

			_productRepository.Update (product);
			//product.Category = category;
			//category.Products.Add (product);

			//category.Parent = topCategory;
			//topCategory.SubCategories.Add (category);

			//_categoryRepository.Insert (topCategory);
		}

		public IEnumerable<Product> GetPage (int pageNumber, int pageSize = 20)
		{
			//var query = from p in _productRepository.AsQueryable ()
			//			orderby p.SKU ascending, p.Name ascending, p.Price descending
			//			select p;
			//                   //select new Product () {
			//			//	Name = p.Name,
			//			//	Price = p.Price,
			//			//	SKU = p.SKU,
			//			//	Id = p.Id,
			//			//	Category = new Category () {
			//			//		Id = p.Category.Id,
			//			//		Name = p.Category.Name
			//			//	}
			//			//};

			//return query.Skip (pageNumber * pageSize)
			//			.Take (pageSize)
			//			.ToList ();

			//return _productRepository.AsQueryable ()
			//						 .OrderBy (p => p.SKU)
			//						 .ThenByDescending (p => p.Name)
			//						 .Skip (pageSize * pageNumber)
			//						 .Take (pageSize);

			return _productRepository.GetPage (pageNumber, pageSize);
		}
	}
}