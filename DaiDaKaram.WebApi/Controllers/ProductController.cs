﻿using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;

using System.Net.Http;
using System.Web;
using System.Linq;
using System.Web.Http;
using DaiDaKaram.Domain;
using DaiDaKaram.Domain.Entities;
using System.Collections.Generic;

namespace DaiDaKaram.Infrastructure.WebApi.Controllers
{
	public class ProductController : ApiController
	{
		IProductService _productService;

		public ProductController (IProductService productService)
		{
			_productService = productService;
		}

		[HttpPost]
		[Route("api/product")]
		public async Task<Product> CreateProduct ()
		{
			// Check if the request contains multipart/form-data.
			if (!Request.Content.IsMimeMultipartContent ()) {
				throw new HttpResponseException (HttpStatusCode.UnsupportedMediaType);
			}

			//string root = HttpContext.Current.Server.MapPath ("~/App_Data");
			string root = "/tmp/daidakaram";
			if (!Directory.Exists (root))
				Directory.CreateDirectory (root);

			var streamProvider = new MultipartFormDataStreamProvider (root);

			// Read the form data and return an async task.
			var resultTask = await Request.Content.ReadAsMultipartAsync (streamProvider) // read data from request
				.ContinueWith (streamReadingTask => {
					if (streamReadingTask.IsFaulted || streamReadingTask.IsCanceled) {
						Request.CreateErrorResponse (HttpStatusCode.InternalServerError, streamReadingTask.Exception);
					}

					List<Photo> photos = streamReadingTask.Result.FileData
												  .Where (f => f.Headers.ContentDisposition.Name.Replace (@"""", "").Replace (@"\", "") == "photos")
												  .Select (f => new Photo () { Bytes = File.ReadAllBytes (f.LocalFileName) }).ToList ();
					var name = streamReadingTask.Result.FormData ["name"];
					var description = streamReadingTask.Result.FormData ["description"];
				Thumbnail thumb = streamReadingTask.Result.FileData
												   .Where (f => f.Headers.ContentDisposition.FileName == streamReadingTask.Result.FormData ["thumbnail"])
												   .Select (f => new Thumbnail () { Bytes = File.ReadAllBytes (f.LocalFileName) })
												   .FirstOrDefault ();

					var resultProduct = _productService.Create (name, null, description, photos, thumb);

					return resultProduct;
				});

			return resultTask;
		}

		[HttpDelete]
		[Route ("api/product/deleteimage/{imageId}")]
		public IHttpActionResult DeleteImage (int imageId)
		{
			_productService.RemovePhoto (imageId);
			return Ok ();
		}

		[HttpPost]
		[Route ("api/product/{productId}/image")]
		public Task<List<int>> AddImagesToProduct (int productId, bool isThumbnail = false)
		{
			if (!Request.Content.IsMimeMultipartContent ()) {
				throw new HttpResponseException (HttpStatusCode.UnsupportedMediaType);
			}
			List<int> imageIds = new List<int> ();
			string root = "/tmp/daidakaram";
			if (!Directory.Exists (root)) Directory.CreateDirectory (root);

			var streamProvider = new MultipartFormDataStreamProvider (root);

			var task = Request.Content.ReadAsMultipartAsync (streamProvider).
			  ContinueWith (srTask => {
				  if (srTask.IsFaulted || srTask.IsCanceled) {
					  Request.CreateErrorResponse (HttpStatusCode.InternalServerError, srTask.Exception);
				  }

				  if (isThumbnail) {
					  _productService.SetThumbnail (productId, new Photo () {
						  Bytes = File.ReadAllBytes (srTask.Result.FileData.FirstOrDefault ().LocalFileName)
					  });
				  }

				  foreach (MultipartFileData f in srTask.Result.FileData) {
					var photo = new Photo () { Bytes = File.ReadAllBytes (f.LocalFileName) };
					  _productService.AddPhoto (productId, photo);
					  imageIds.Add (photo.Id);
				  }

				  return imageIds;
			  });

			return task;
		}

		[HttpPut]
		[Route ("api/product/{productId}/ChangeThumbnail/photoIndex")]
		public void ChangeThumbnail (int productId, int photoIndex)
		{
			_productService.ChangeThumbnail (productId, photoIndex);
		}

		[HttpPut]
		[Route ("api/product")]
		public void UpdateProduct (Product p)
		{
			_productService.Update (p);
		}
	}
}