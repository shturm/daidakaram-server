using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Linq;
using System.Web.Http;
using MonoWebApi.Domain;
using MonoWebApi.Domain.Entities;

namespace MonoWebApi.Infrastructure.WebApi.Controllers
{
	public class ProductController : ApiController
	{
		IProductService _productService;

		public ProductController (IProductService productService)
		{
			_productService = productService;
		}
		public async Task<Product> Post ()
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

					var photos = streamReadingTask.Result.FileData
												  .Where (f => f.Headers.ContentDisposition.Name == "photos")
												  .Select (f => new Image () { Bytes = File.ReadAllBytes (f.LocalFileName) }).ToList ();
					var name = streamReadingTask.Result.FormData ["name"];
					var description = streamReadingTask.Result.FormData ["description"];
					Image thumb = streamReadingTask.Result.FileData
												   .Where (f => f.Headers.ContentDisposition.FileName == streamReadingTask.Result.FormData ["thumbnail"])
												   .Select (f => new Image () { Bytes = File.ReadAllBytes (f.LocalFileName) })
												   .FirstOrDefault ();

					var resultProduct = _productService.Create (name, description, photos, thumb);

					return resultProduct;
				});

			return resultTask;
		}
	}
}